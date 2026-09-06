using System.Text.Json;
using Airbnb.Application.Abstracts.Helpers;
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.External;
using Airbnb.Application.Options;
using Airbnb.Domain;
using Airbnb.Domain.Constants;
using Airbnb.Domain.Models;
using FluentValidation;
using MapsterMapper;
using Microsoft.Extensions.Options;

namespace Airbnb.Application.Helpers;

public class ExternalDataLoader : IExternalDataLoader
{
    private readonly IOptions<DefaultUserOptions> _defaultUserOptions;
    private readonly IValidator<ExternalHostDto> _validator;
    private readonly IUserRegistrationService _userRegistrationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExternalDataLoader(
        IOptions<DefaultUserOptions> defaultUserOptions,
        IValidator<ExternalHostDto> validator,
        IUserRegistrationService userRegistrationService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _defaultUserOptions = defaultUserOptions;
        _unitOfWork = unitOfWork;
        _userRegistrationService = userRegistrationService;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<string>> LoadDataFromJsonFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return new Result<string>(false, null, "File name is empty");
        }

        if (!File.Exists(filePath))
        {
            return new Result<string>(false, null, "File doesn't exist");
        }

        await _unitOfWork.StartTransactionAsync();

        try
        {
            await using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

            await foreach (ExternalHostDto? hostDto in JsonSerializer.DeserializeAsyncEnumerable<ExternalHostDto>(stream))
            {
                if (hostDto == null)
                    continue;

                var validationResult = await _validator.ValidateAsync(hostDto);
                if (!validationResult.IsValid)
                {
                    return await FailAsync(validationResult.Errors.First().ErrorMessage);
                }

                Result<User> hostResult = await UpsertHostAsync(hostDto);
                if (!hostResult.IsSuccessful)
                {
                    return await FailAsync(hostResult.Message);
                }

                foreach (ExternalApartmentDto apartmentDto in hostDto.Apartments)
                {
                    Result<Apartment> apartmentResult = await UpsertApartmentAsync(apartmentDto, hostResult.Value!.Id);
                    if (!apartmentResult.IsSuccessful)
                    {
                        return await FailAsync(apartmentResult.Message);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            return await FailAsync(ex.Message);
        }

        File.Delete(filePath);
        await _unitOfWork.CommitTransactionAsync();
        return new Result<string>(true, "Data loaded successfully", null);
    }

    private async Task<Result<string>> FailAsync(string? message)
    {
        await _unitOfWork.RollbackTransactionAsync();
        return new Result<string>(false, null, message);
    }

    private async Task<Result<User>> UpsertHostAsync(ExternalHostDto hostDto)
    {
        User? existingHost = await _userRegistrationService.FindUserByEmailAsync(hostDto.Email);
        if (existingHost is not null)
        {
            return existingHost;
        }

        User host = _mapper.Map<User>(hostDto);

        var createResult = await _userRegistrationService.CreateUserAsync(host, _defaultUserOptions.Value.DefaultPassword);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return $"Failed to create host '{host.Email}': {errors}";
        }

        var roleResult = await _userRegistrationService.AddUserToRoleAsync(host, Roles.Host);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            return $"Failed to assign role '{Roles.Host}' to host '{host.Email}': {errors}";
        }

        return host;
    }

    private async Task<Result<Apartment>> UpsertApartmentAsync(ExternalApartmentDto apartmentDto, string ownerId)
    {
        if (!Guid.TryParse(apartmentDto.ExternalId, out Guid externalId))
        {
            return $"Apartment has an invalid ExternalId: '{apartmentDto.ExternalId}'";
        }

        Apartment? existing = await FindApartmentByExternalIdAsync(externalId);

        if (existing is null)
        {
            Apartment apartment = _mapper.Map<Apartment>(apartmentDto);
            apartment.Id = Guid.NewGuid();
            apartment.CreatedAt = DateTime.UtcNow;
            apartment.OwnerId = ownerId;

            await _unitOfWork.Apartments.CreateAsync(apartment);
            return apartment;
        }

        _mapper.Map(apartmentDto, existing);
        existing.OwnerId = ownerId;

        await _unitOfWork.Apartments.UpdateAsync(existing);
        return existing;
    }

    private async Task<Apartment?> FindApartmentByExternalIdAsync(Guid externalId)
    {
        try
        {
            return await _unitOfWork.Apartments.GetByExternalIdAsync(externalId);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}

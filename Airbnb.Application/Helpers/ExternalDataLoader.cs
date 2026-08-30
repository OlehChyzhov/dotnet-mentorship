using System.Text.Json;
using Airbnb.Application.Abstracts.Helpers;
using Airbnb.Application.Abstracts.Repositories;
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
    private readonly IOptions<DataFileOptions> _fileOptions;
    private readonly IValidator<ExternalHostDto> _validator;
    private readonly IUserHelper _userHelper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public ExternalDataLoader(
        IOptions<DefaultUserOptions> defaultUserOptions,
        IOptions<DataFileOptions> fileOptions, 
        IValidator<ExternalHostDto> validator,
        IUserHelper userHelper,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _defaultUserOptions = defaultUserOptions;
        _fileOptions = fileOptions;
        _unitOfWork = unitOfWork;
        _userHelper = userHelper;
        _validator = validator;
        _mapper = mapper;
    }
    
    public async Task<Result<string>> LoadDataFromJsonFileAsync()
    {
        if (string.IsNullOrEmpty(_fileOptions.Value.FileName))
        {
            return new Result<string>(false, null, "File name is empty");
        }
        
        string filePath = Path.Combine(AppContext.BaseDirectory, _fileOptions.Value.FileName);
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
                
                await ValidateExternalHostDtoAsync(hostDto);
                
                (User host, List<Apartment> apartments) = MapHostAndApartments(hostDto);

                await TryCreateHostAsync(host);
                
                foreach (var apartment in apartments)
                {
                    await _unitOfWork.Apartments.CreateAsync(apartment);
                }

                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return new Result<string>(false, null, ex.Message);
        }

        await _unitOfWork.CommitTransactionAsync();
        return  new Result<string>(true, $"{_fileOptions.Value.FileName} loaded successfully", null);
    }

    private async Task TryCreateHostAsync(User host)
    {
        var createResult = await _userHelper.CreateUserAsync(host, _defaultUserOptions.Value.DefaultPassword);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            var message = $"Failed to create host '{host.Email}': {errors}";
            
            throw new InvalidOperationException(message);
        }
                
        var roleResult = await _userHelper.AddUserToRoleAsync(host, Roles.Host);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            var message = $"Failed to create role '{Roles.Host}': {errors}";
            
            throw new InvalidOperationException(message);
        }
    }

    private (User, List<Apartment>) MapHostAndApartments(ExternalHostDto hostDto)
    {
        User user = _mapper.Map<User>(hostDto);
        List<Apartment> apartments = _mapper.Map<List<Apartment>>(hostDto.Apartments);
        
        apartments.ForEach(apartment => apartment.OwnerId = user.Id);
        return (user, apartments);
    } 
    
    private async Task ValidateExternalHostDtoAsync(ExternalHostDto hostDto)
    {
        var result =  await _validator.ValidateAsync(hostDto);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}
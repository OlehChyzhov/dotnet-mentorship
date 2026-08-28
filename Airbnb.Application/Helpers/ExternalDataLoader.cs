using System.Text.Json;
using Airbnb.Application.Abstracts.Helpers;
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs.External;
using Airbnb.Application.Options;
using Airbnb.Domain;
using Microsoft.Extensions.Options;

namespace Airbnb.Application.Helpers;

public class ExternalDataLoader : IExternalDataLoader
{
    private readonly IOptions<DataFileOptions> _fileOptions;
    private readonly IUnitOfWork _unitOfWork;
    
    public ExternalDataLoader(IOptions<DataFileOptions> fileOptions, IUnitOfWork unitOfWork)
    {
        _fileOptions = fileOptions;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<string>> LoadDataFromJsonFileAsync()
    {
        if (string.IsNullOrEmpty(_fileOptions.Value.FileName))
        {
            return new Result<string>(false, null, "File name is empty");
        }
        
        string filePath = Path.Combine(AppContext.BaseDirectory, _fileOptions.Value.FileName);
        if (!File.Exists(_fileOptions.Value.FileName))
        {
            return new Result<string>(false, null, "File doesn't exist");
        }

        await _unitOfWork.StartTransactionAsync();
        
        try
        {
            await using FileStream stream = File.Open(filePath, FileMode.Open);
            
            await foreach (ExternalHostDto? hostDto in JsonSerializer.DeserializeAsyncEnumerable<ExternalHostDto>(stream))
            {
                if (hostDto != null)
                {
                    // Processing logic
                }
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
}
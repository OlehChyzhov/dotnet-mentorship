using Airbnb.Application.Abstracts.Helpers;
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.Options;
using Airbnb.Domain;
using Microsoft.Extensions.Options;

namespace Airbnb.Application.Helpers;

public class DataLoader : IDataLoader
{
    private readonly IOptions<DataFileOptions> _fileOptions;
    private readonly IUnitOfWork _unitOfWork;
    
    public DataLoader(IOptions<DataFileOptions> fileOptions, IUnitOfWork unitOfWork)
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
        if (File.Exists(_fileOptions.Value.FileName))
        {
            return new Result<string>(false, null, "File doesn't exist");
        }

        await _unitOfWork.StartTransactionAsync();
        
        try
        {
            
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
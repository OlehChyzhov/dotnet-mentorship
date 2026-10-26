using Airbnb.Shared.HTTP;

namespace Airbnb.Application.Abstractions.Services;

public interface IExternalDataLoader
{
    Task<Result<string>> LoadDataFromJsonFileAsync(string filePath);
}
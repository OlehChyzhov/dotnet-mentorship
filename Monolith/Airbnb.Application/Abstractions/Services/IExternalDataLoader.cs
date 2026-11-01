using Airbnb.Shared.Common;

namespace Airbnb.Application.Abstractions.Services;

public interface IExternalDataLoader
{
    Task<Result<string>> LoadDataFromJsonFileAsync(string filePath);
}
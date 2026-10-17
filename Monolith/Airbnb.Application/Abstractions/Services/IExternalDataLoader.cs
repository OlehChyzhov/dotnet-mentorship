using Airbnb.Contracts;
using Airbnb.Contracts.HTTP;
using Airbnb.Domain;

namespace Airbnb.Application.Abstracts.Services;

public interface IExternalDataLoader
{
    Task<Result<string>> LoadDataFromJsonFileAsync(string filePath);
}
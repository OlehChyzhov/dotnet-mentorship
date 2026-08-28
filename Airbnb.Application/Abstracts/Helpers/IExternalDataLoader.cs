using Airbnb.Domain;

namespace Airbnb.Application.Abstracts.Helpers;

public interface IExternalDataLoader
{
    Task<Result<string>> LoadDataFromJsonFileAsync();
}
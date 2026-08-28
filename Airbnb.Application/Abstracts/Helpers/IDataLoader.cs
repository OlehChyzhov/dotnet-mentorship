using Airbnb.Domain;

namespace Airbnb.Application.Abstracts.Helpers;

public interface IDataLoader
{
    Task<Result<string>> LoadDataFromJsonFileAsync();
}
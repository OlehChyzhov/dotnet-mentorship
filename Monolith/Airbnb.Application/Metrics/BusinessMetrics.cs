using System.Diagnostics.Metrics;

namespace Airbnb.Application.Metrics;

public sealed class BusinessMetrics
{
    public const string MeterName = "Airbnb.Business";

    private readonly Counter<long> _usersCreated;

    public BusinessMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);

        _usersCreated = meter.CreateCounter<long>(
            "airbnb_business_recently_created_users",
            unit: "{user}",
            description: "Number of users created");
    }

    public void UserCreated(string emailInbox) =>
        _usersCreated.Add(1, new KeyValuePair<string, object?>("user_email_inbox", emailInbox));
}

using Ardalis.GuardClauses;

namespace WebShop.Api.Config
{
    public class ConnectionString : IConnectionString
    {
        public ConnectionString(string value)
        {
            Guard.Against.NullOrEmpty(value, nameof(value));
            Value = value;
        }

        public string Value { get; }
    }

    public interface IConnectionString
    {
        string Value { get; }
    }
}

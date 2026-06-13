namespace Identity.Application.Interfaces;

public interface IHmacService
{
    string Sign(string payload);
    bool Verify(string payload, string signature);
}

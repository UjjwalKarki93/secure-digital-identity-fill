namespace Banking.Domain.ValueObjects;

public class QrPayload
{
    public Guid RequestId { get; set; }
    public string BankId { get; set; } = string.Empty;
    public string ExpiryIso { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;

    public static string BuildCanonical(Guid requestId, string bankId, string expiryIso) =>
        $"{requestId}|{bankId}|{expiryIso}";

    public string ToCanonicalString() =>
        BuildCanonical(RequestId, BankId, ExpiryIso);
}

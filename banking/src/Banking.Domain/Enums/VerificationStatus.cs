namespace Banking.Domain.Enums;

public enum VerificationStatus
{
    Pending = 0,
    QrGenerated = 1,
    Scanned = 2,
    Completed = 3,
    Expired = 4,
    Failed = 5
}

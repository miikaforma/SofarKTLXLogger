namespace SofarKTLXLogger.Helpers;

public static class CommonExtensions
{
    public static CancellationTokenSource ExtendWithDelayedToken(this CancellationToken cancellationToken, TimeSpan delay)
    {
        return CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,
            new CancellationTokenSource(delay).Token);
    }
}
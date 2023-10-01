namespace SofarKTLXLogger.Daytime;

public interface IDaytimeService
{
    Task<bool> IsDaytime();
}
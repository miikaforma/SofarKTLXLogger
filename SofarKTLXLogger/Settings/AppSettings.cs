﻿namespace SofarKTLXLogger.Settings;

public class AppSettings
{
    public const string SectionName = "App";
    
    public int Timeout { get; set; } = 25_000; // 25 seconds by default
    public int LoggingInterval { get; set; } = 30_000;
    public bool EnableDaytimeChecking { get; set; }
    public required string TimeZone { get; set; }
    public required string Longitude { get; set; }
    public required string Latitude { get; set; }
    public bool OfflineMode { get; set; }
}
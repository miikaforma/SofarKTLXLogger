﻿// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using Npgsql;

namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

public enum CountryCode
{
    Germany = 0,
    Internal = 1,
    Australia = 2,
    Spain = 3,
    Turkey = 4,
    Denmark = 5,
    Greece = 6,
    Netherland = 7,
    Belgium = 8,
    UK = 9,
    China = 10,
    France = 11,
    Poland = 12,
    Austria = 13,
    Japan = 14,
     // = 15,
     // = 16,
     // = 17,
    EU = 18,
    IEC = 19,
    Korea = 20,
    Sweden = 21,
    EuropeGeneral = 22,
     // = 23,
    Cyprus = 24,
    India = 25,
    Philippines = 26,
    NewZealand = 27,
    Brazil = 28,
    Slovakia = 29,
     // = 30,
     // = 31,
     // = 32,
    Ukraine = 33,
    Norway = 34,
    Mexico = 35,
    //  = 36,
    //  = 37,
    WideRange60Hz = 38,
    Ireland = 39,
    Thailand = 40,
     // = 41,
    LVRange50Hz = 42,
    SouthAfrica = 44,
     // = 45,
    Dubai = 46,
    Croatia = 107,
    Lithuania = 108,
}

public static class CountryCodeExtensions
{
 public static void AddMetric(this CountryCode countryCode, NpgsqlParameterCollection parameterCollection)
 {
  parameterCollection.AddWithValue("CountryCode", (int)countryCode);
 }
}

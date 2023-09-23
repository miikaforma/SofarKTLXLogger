namespace SofarKTLXLogger.ModbusRTU.RealtimeData;

public enum Unit
{
    None,
    Watt, // Power unit - The watt is a unit of power. It is the power required to do one joule of work per second.
    WattHour, // Energy unit - A watt-hour is the amount of energy equivalent to a power of one watt operating for one hour.
    Kilowatt, // Power unit - A kilowatt is a unit of power equal to 1,000 watts.
    KilowattHour, // Energy unit - A kilowatt-hour is a unit of energy equivalent to one kilowatt of power expended for one hour of time.
    Ampere, // Electric current unit - The ampere is the base unit of electric current in the International System of Units, equal to one coulomb per second.
    Volt, // Electric voltage unit - The volt is the derived unit for electric potential across a conductor when a current of one ampere dissipates one watt of power.
    Ohm, // Electric resistance unit - The ohm is the unit of electrical resistance in the International System of Units.
    KiloOhm, // Electric resistance unit: The kiloohm is a unit of electrical resistance equal to 1000 ohms.
    Celsius, // Temperature unit - The Celsius scale is based on the properties of water. The freezing point is 0 degrees Celsius and the boiling point is 100 degrees Celsius.
    Fahrenheit, // Temperature unit - In Fahrenheit scale, the freezing point of water is 32 degrees and the boiling point is 212 degrees.
    Second, // Time unit - The second is the smallest standard unit of time in the International System of Units.
    Minute, // Time unit - The minute is a unit of time equal to 60 seconds.
    Hour, // Time unit - The hour is a unit of time equal to 60 minutes.
    Day, // Time unit - The day is a unit of time equal to 24 hours.
    KilovoltAmpereReactive, // Reactive Power unit - The kilovolt-ampere reactive (kVAR) is a unit of reactive power in an alternating current electrical system.
    Hertz // Frequency unit - The hertz is a unit of frequency defined as one cycle per second of a periodic phenomenon.
}
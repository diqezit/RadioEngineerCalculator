using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RadioEngineerCalculator.Services
{
    public static class ComboBoxService
    {
        #region Приватные поля

        private static readonly Dictionary<string, ObservableCollection<string>> units = new()
        {
            { "Current", new() { "pA", "nA", "μA", "mA", "A" } },
            { "Resistance", new() { "μΩ", "mΩ", "Ω", "kΩ", "MΩ" } },
            { "Voltage", new() { "nV", "μV", "mV", "V", "kV" } },
            { "Power", new() { "μW", "mW", "W", "kW", "MW" } },
            { "Length", new() { "nm", "μm", "mm", "cm", "m", "km" } },
            { "Frequency", new() { "Hz", "kHz", "MHz", "GHz", "THz" } },
            { "Capacitance", new() { "pF", "nF", "μF", "mF", "F" } },
            { "Inductance", new() { "pH", "nH", "μH", "mH", "H" } },
            { "Time", new() { "ps", "ns", "μs", "ms", "s" } },
            { "Temperature", new() { "°C", "K", "°F" } },
            { "Angle", new() { "°", "rad", "grad" } },
            { "Pressure", new() { "Pa", "kPa", "MPa", "bar", "mbar", "atm" } },
            { "Force", new() { "N", "kN", "MN", "lbf" } },
            { "Energy", new() { "eV", "cal", "J", "kJ", "MJ" } },
            { "Speed", new() { "m/s", "km/h", "mph", "knot" } },
            { "Area", new() { "cm²", "m²", "ha", "acre", "km²" } },
            { "Volume", new() { "mL", "L", "cm³", "m³", "gal" } },
            { "Mass", new() { "μg", "mg", "g", "kg", "t" } },
            { "Density", new() { "kg/L", "g/cm³", "kg/m³" } },
            { "Flow", new() { "L/min", "gal/min", "L/s", "m³/s" } },
            { "Conductance", new() { "pS", "nS", "μS", "mS", "S" } },
            { "Resistivity", new() { "Ω·cm", "Ω·mm²/m", "Ω·m" } },
            { "Permittivity", new() { "pF/m", "nF/m", "F/m" } },
            { "Permeability", new() { "nH/m", "μH/m", "H/m" } },
            { "MagneticField", new() { "μT", "mT", "T", "G" } },
            { "MagneticFlux", new() { "μWb", "mWb", "Wb" } },
            { "LuminousFlux", new() { "lm", "klm" } },
            { "Illuminance", new() { "lx", "klx" } },
            { "SoundPressure", new() { "μPa", "Pa", "dB" } },
            { "SoundIntensity", new() { "μW/m²", "mW/m²", "W/m²" } },
            { "SoundPower", new() { "μW", "mW", "W" } },
            { "SoundLevel", new() { "dB", "dB(A)", "dB(B)", "dB(C)" } },
        };

        #endregion

        #region Публичные методы

        public static ObservableCollection<string> GetUnits(string quantity) =>
            units.TryGetValue(quantity, out var unitCollection) ? unitCollection : new ObservableCollection<string>();

        #endregion
    }
}

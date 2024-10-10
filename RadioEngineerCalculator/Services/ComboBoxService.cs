using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RadioEngineerCalculator.Services
{
    public static class ComboBoxService
    {
        private static readonly Dictionary<string, ObservableCollection<string>> units = new Dictionary<string, ObservableCollection<string>>
        {
            { "Current", new ObservableCollection<string> { "A", "mA", "μA", "nA", "pA" } },
            { "Resistance", new ObservableCollection<string> { "Ω", "kΩ", "MΩ", "mΩ", "μΩ" } },
            { "Voltage", new ObservableCollection<string> { "V", "mV", "kV", "μV", "nV" } },
            { "Power", new ObservableCollection<string> { "W", "kW", "MW", "mW", "μW" } },
            { "Length", new ObservableCollection<string> { "mm", "cm", "m", "km", "μm", "nm" } },
            { "Frequency", new ObservableCollection<string> { "Hz", "kHz", "MHz", "GHz", "THz" } },
            { "Capacitance", new ObservableCollection<string> { "F", "mF", "μF", "nF", "pF" } },
            { "Inductance", new ObservableCollection<string> { "H", "mH", "μH", "nH", "pH" } },
            { "Time", new ObservableCollection<string> { "s", "ms", "μs", "ns", "ps" } },
            { "Temperature", new ObservableCollection<string> { "°C", "K", "°F" } },
            { "Angle", new ObservableCollection<string> { "°", "rad", "grad" } },
            { "Pressure", new ObservableCollection<string> { "Pa", "kPa", "MPa", "bar", "mbar", "atm" } },
            { "Force", new ObservableCollection<string> { "N", "kN", "MN", "lbf" } },
            { "Energy", new ObservableCollection<string> { "J", "kJ", "MJ", "eV", "cal" } },
            { "Speed", new ObservableCollection<string> { "m/s", "km/h", "mph", "knot" } },
            { "Area", new ObservableCollection<string> { "m²", "cm²", "km²", "ha", "acre" } },
            { "Volume", new ObservableCollection<string> { "m³", "cm³", "L", "mL", "gal" } },
            { "Mass", new ObservableCollection<string> { "kg", "g", "mg", "μg", "t" } },
            { "Density", new ObservableCollection<string> { "kg/m³", "g/cm³", "kg/L" } },
            { "Flow", new ObservableCollection<string> { "m³/s", "L/s", "L/min", "gal/min" } },
            { "Conductance", new ObservableCollection<string> { "S", "mS", "μS", "nS", "pS" } },
            { "Resistivity", new ObservableCollection<string> { "Ω·m", "Ω·cm", "Ω·mm²/m" } },
            { "Permittivity", new ObservableCollection<string> { "F/m", "pF/m", "nF/m" } },
            { "Permeability", new ObservableCollection<string> { "H/m", "μH/m", "nH/m" } },
            { "MagneticField", new ObservableCollection<string> { "T", "mT", "μT", "G" } },
            { "MagneticFlux", new ObservableCollection<string> { "Wb", "mWb", "μWb" } },
            { "LuminousFlux", new ObservableCollection<string> { "lm", "klm" } },
            { "Illuminance", new ObservableCollection<string> { "lx", "klx" } },
            { "SoundPressure", new ObservableCollection<string> { "Pa", "μPa", "dB" } },
            { "SoundIntensity", new ObservableCollection<string> { "W/m²", "mW/m²", "μW/m²" } },
            { "SoundPower", new ObservableCollection<string> { "W", "mW", "μW" } },
            { "SoundLevel", new ObservableCollection<string> { "dB", "dB(A)", "dB(B)", "dB(C)" } },
        };

        public static ObservableCollection<string> GetUnits(string quantity)
        {
            if (units.TryGetValue(quantity, out var unitCollection))
            {
                return unitCollection;
            }
            return new ObservableCollection<string>();
        }
    }
}

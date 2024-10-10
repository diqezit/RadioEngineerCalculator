using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RadioEngineerCalculator.Services
{
    public static class ComboBoxService
    {
        private static readonly Dictionary<string, ObservableCollection<string>> units = new Dictionary<string, ObservableCollection<string>>
        {
            { "Current", new ObservableCollection<string> { "pA", "nA", "μA", "mA", "A" } },
            { "Resistance", new ObservableCollection<string> { "μΩ", "mΩ", "Ω", "kΩ", "MΩ" } },
            { "Voltage", new ObservableCollection<string> { "nV", "μV", "mV", "V", "kV" } },
            { "Power", new ObservableCollection<string> { "μW", "mW", "W", "kW", "MW" } },
            { "Length", new ObservableCollection<string> { "nm", "μm", "mm", "cm", "m", "km" } },
            { "Frequency", new ObservableCollection<string> { "Hz", "kHz", "MHz", "GHz", "THz" } },
            { "Capacitance", new ObservableCollection<string> { "pF", "nF", "μF", "mF", "F" } },
            { "Inductance", new ObservableCollection<string> { "pH", "nH", "μH", "mH", "H" } },
            { "Time", new ObservableCollection<string> { "ps", "ns", "μs", "ms", "s" } },
            { "Temperature", new ObservableCollection<string> { "°C", "K", "°F" } },
            { "Angle", new ObservableCollection<string> { "°", "rad", "grad" } },
            { "Pressure", new ObservableCollection<string> { "Pa", "kPa", "MPa", "bar", "mbar", "atm" } },
            { "Force", new ObservableCollection<string> { "N", "kN", "MN", "lbf" } },
            { "Energy", new ObservableCollection<string> { "eV", "cal", "J", "kJ", "MJ" } },
            { "Speed", new ObservableCollection<string> { "m/s", "km/h", "mph", "knot" } },
            { "Area", new ObservableCollection<string> { "cm²", "m²", "ha", "acre", "km²" } },
            { "Volume", new ObservableCollection<string> { "mL", "L", "cm³", "m³", "gal" } },
            { "Mass", new ObservableCollection<string> { "μg", "mg", "g", "kg", "t" } },
            { "Density", new ObservableCollection<string> { "kg/L", "g/cm³", "kg/m³" } },
            { "Flow", new ObservableCollection<string> { "L/min", "gal/min", "L/s", "m³/s" } },
            { "Conductance", new ObservableCollection<string> { "pS", "nS", "μS", "mS", "S" } },
            { "Resistivity", new ObservableCollection<string> { "Ω·cm", "Ω·mm²/m", "Ω·m" } },
            { "Permittivity", new ObservableCollection<string> { "pF/m", "nF/m", "F/m" } },
            { "Permeability", new ObservableCollection<string> { "nH/m", "μH/m", "H/m" } },
            { "MagneticField", new ObservableCollection<string> { "μT", "mT", "T", "G" } },
            { "MagneticFlux", new ObservableCollection<string> { "μWb", "mWb", "Wb" } },
            { "LuminousFlux", new ObservableCollection<string> { "lm", "klm" } },
            { "Illuminance", new ObservableCollection<string> { "lx", "klx" } },
            { "SoundPressure", new ObservableCollection<string> { "μPa", "Pa", "dB" } },
            { "SoundIntensity", new ObservableCollection<string> { "μW/m²", "mW/m²", "W/m²" } },
            { "SoundPower", new ObservableCollection<string> { "μW", "mW", "W" } },
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
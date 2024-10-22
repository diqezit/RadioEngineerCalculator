using System;
using System.Collections.Generic;
using System.Linq;

namespace RadioEngineerCalculator.Services
{
    public static class UnitC
    {
        #region Перечисления

        public enum PhysicalQuantity
        {
            Frequency, Resistance, Capacitance, Voltage, Current, Inductance, Length, Power,
            Pressure, Time, ReactivePower, Temperature, Angle, Force, Energy, Speed, Area,
            Volume, Mass, Density, Flow, Conductance, Resistivity, Permittivity, Permeability,
            MagneticField, MagneticFlux, LuminousFlux, Illuminance, SoundPressure,
            SoundIntensity, SoundPower, SoundLevel, Attenuation
        }

        #endregion

        #region Факторы единиц измерения

        public static readonly Dictionary<PhysicalQuantity, IReadOnlyDictionary<string, double>> UnitFactors =
            new Dictionary<PhysicalQuantity, IReadOnlyDictionary<string, double>>
            {
                [PhysicalQuantity.Frequency] = new Dictionary<string, double>
                {
                    ["Hz"] = 1,
                    ["kHz"] = 1e3,
                    ["MHz"] = 1e6,
                    ["GHz"] = 1e9,
                    ["THz"] = 1e12
                },
                [PhysicalQuantity.Resistance] = new Dictionary<string, double>
                {
                    ["Ω"] = 1,
                    ["kΩ"] = 1e3,
                    ["MΩ"] = 1e6,
                    ["mΩ"] = 1e-3,
                    ["μΩ"] = 1e-6
                },
                [PhysicalQuantity.Capacitance] = new Dictionary<string, double>
                {
                    ["F"] = 1,
                    ["mF"] = 1e-3,
                    ["μF"] = 1e-6,
                    ["nF"] = 1e-9,
                    ["pF"] = 1e-12
                },
                [PhysicalQuantity.Voltage] = new Dictionary<string, double>
                {
                    ["V"] = 1,
                    ["mV"] = 1e-3,
                    ["kV"] = 1e3,
                    ["μV"] = 1e-6,
                    ["nV"] = 1e-9
                },
                [PhysicalQuantity.Current] = new Dictionary<string, double>
                {
                    ["A"] = 1,
                    ["mA"] = 1e-3,
                    ["μA"] = 1e-6,
                    ["nA"] = 1e-9,
                    ["pA"] = 1e-12
                },
                [PhysicalQuantity.Inductance] = new Dictionary<string, double>
                {
                    ["H"] = 1,
                    ["mH"] = 1e-3,
                    ["μH"] = 1e-6,
                    ["nH"] = 1e-9,
                    ["pH"] = 1e-12
                },
                [PhysicalQuantity.Length] = new Dictionary<string, double>
                {
                    ["m"] = 1,
                    ["cm"] = 1e-2,
                    ["mm"] = 1e-3,
                    ["km"] = 1e3,
                    ["μm"] = 1e-6,
                    ["nm"] = 1e-9
                },
                [PhysicalQuantity.Power] = new Dictionary<string, double>
                {
                    ["W"] = 1,
                    ["kW"] = 1e3,
                    ["MW"] = 1e6,
                    ["mW"] = 1e-3,
                    ["μW"] = 1e-6
                },
                [PhysicalQuantity.Pressure] = new Dictionary<string, double>
                {
                    ["Pa"] = 1,
                    ["kPa"] = 1e3,
                    ["MPa"] = 1e6,
                    ["bar"] = 1e5,
                    ["mbar"] = 1e2,
                    ["atm"] = 101325
                },
                [PhysicalQuantity.Time] = new Dictionary<string, double>
                {
                    ["s"] = 1,
                    ["ms"] = 1e-3,
                    ["μs"] = 1e-6,
                    ["ns"] = 1e-9,
                    ["ps"] = 1e-12,
                    ["min"] = 60,
                    ["h"] = 3600
                },

                [PhysicalQuantity.ReactivePower] = new Dictionary<string, double>
                {
                    ["VAr"] = 1,
                    ["kVAr"] = 1e3,
                    ["mVAr"] = 1e-3
                },
                [PhysicalQuantity.Temperature] = new Dictionary<string, double>
                {
                    ["°C"] = 1,
                    ["K"] = 1,
                    ["°F"] = 1
                },
                [PhysicalQuantity.Angle] = new Dictionary<string, double>
                {
                    ["°"] = 1,
                    ["rad"] = Math.PI / 180,
                    ["grad"] = 0.9
                },
                [PhysicalQuantity.Force] = new Dictionary<string, double>
                {
                    ["N"] = 1,
                    ["kN"] = 1e3,
                    ["MN"] = 1e6,
                    ["lbf"] = 4.44822
                },
                [PhysicalQuantity.Energy] = new Dictionary<string, double>
                {
                    ["J"] = 1,
                    ["kJ"] = 1e3,
                    ["MJ"] = 1e6,
                    ["eV"] = 1.60218e-19,
                    ["cal"] = 4.184
                },
                [PhysicalQuantity.Speed] = new Dictionary<string, double>
                {
                    ["m/s"] = 1,
                    ["km/h"] = 1 / 3.6,
                    ["mph"] = 0.44704,
                    ["knot"] = 0.514444
                },
                [PhysicalQuantity.Area] = new Dictionary<string, double>
                {
                    ["m²"] = 1,
                    ["cm²"] = 1e-4,
                    ["km²"] = 1e6,
                    ["ha"] = 1e4,
                    ["acre"] = 4046.86
                },
                [PhysicalQuantity.Volume] = new Dictionary<string, double>
                {
                    ["m³"] = 1,
                    ["cm³"] = 1e-6,
                    ["L"] = 1e-3,
                    ["mL"] = 1e-6,
                    ["gal"] = 3.78541e-3
                },
                [PhysicalQuantity.Mass] = new Dictionary<string, double>
                {
                    ["kg"] = 1,
                    ["g"] = 1e-3,
                    ["mg"] = 1e-6,
                    ["μg"] = 1e-9,
                    ["t"] = 1e3
                },
                [PhysicalQuantity.Density] = new Dictionary<string, double>
                {
                    ["kg/m³"] = 1,
                    ["g/cm³"] = 1e3,
                    ["kg/L"] = 1e3
                },
                [PhysicalQuantity.Flow] = new Dictionary<string, double>
                {
                    ["m³/s"] = 1,
                    ["L/s"] = 1e-3,
                    ["L/min"] = 1.66667e-5,
                    ["gal/min"] = 6.30902e-5
                },
                [PhysicalQuantity.Conductance] = new Dictionary<string, double>
                {
                    ["S"] = 1,
                    ["mS"] = 1e-3,
                    ["μS"] = 1e-6,
                    ["nS"] = 1e-9,
                    ["pS"] = 1e-12
                },
                [PhysicalQuantity.Resistivity] = new Dictionary<string, double>
                {
                    ["Ω·m"] = 1,
                    ["Ω·cm"] = 1e-2,
                    ["Ω·mm²/m"] = 1e-6
                },
                [PhysicalQuantity.Permittivity] = new Dictionary<string, double>
                {
                    ["F/m"] = 1,
                    ["pF/m"] = 1e-12,
                    ["nF/m"] = 1e-9
                },
                [PhysicalQuantity.Permeability] = new Dictionary<string, double>
                {
                    ["H/m"] = 1,
                    ["μH/m"] = 1e-6,
                    ["nH/m"] = 1e-9
                },
                [PhysicalQuantity.MagneticField] = new Dictionary<string, double>
                {
                    ["T"] = 1,
                    ["mT"] = 1e-3,
                    ["μT"] = 1e-6,
                    ["G"] = 1e-4
                },
                [PhysicalQuantity.MagneticFlux] = new Dictionary<string, double>
                {
                    ["Wb"] = 1,
                    ["mWb"] = 1e-3,
                    ["μWb"] = 1e-6
                },
                [PhysicalQuantity.LuminousFlux] = new Dictionary<string, double>
                {
                    ["lm"] = 1,
                    ["klm"] = 1e3
                },
                [PhysicalQuantity.Illuminance] = new Dictionary<string, double>
                {
                    ["lx"] = 1,
                    ["klx"] = 1e3
                },
                [PhysicalQuantity.SoundPressure] = new Dictionary<string, double>
                {
                    ["Pa"] = 1,
                    ["μPa"] = 1e-6,
                    ["dB"] = 1
                },
                [PhysicalQuantity.SoundIntensity] = new Dictionary<string, double>
                {
                    ["W/m²"] = 1,
                    ["mW/m²"] = 1e-3,
                    ["μW/m²"] = 1e-6
                },
                [PhysicalQuantity.SoundPower] = new Dictionary<string, double>
                {
                    ["W"] = 1,
                    ["mW"] = 1e-3,
                    ["μW"] = 1e-6
                },
                [PhysicalQuantity.SoundLevel] = new Dictionary<string, double>
                {
                    ["dB"] = 1,
                    ["dB(A)"] = 1,
                    ["dB(B)"] = 1,
                    ["dB(C)"] = 1
                },
                [PhysicalQuantity.Attenuation] = new Dictionary<string, double>
                {
                    ["dB"] = 1,
                    ["dBm"] = 1,
                    ["Np"] = 8.685889638
                }
            };

        private static IReadOnlyDictionary<string, double> GetUnitFactors(PhysicalQuantity quantity)
        {
            if (!UnitFactors.TryGetValue(quantity, out var factors))
            {
                throw new ArgumentException($"Недопустимая физическая величина: {quantity}");
            }
            return factors;
        }

        #endregion

        #region Методы конвертации

        public static double Convert(double value, string fromUnit, string toUnit, PhysicalQuantity quantity)
        {
            var factors = GetUnitFactors(quantity);

            var conversionFactors = factors
                .Where(f => f.Key == fromUnit || f.Key == toUnit)
                .ToList();

            if (conversionFactors.Count != 2)
            {
                throw new ArgumentException($"Недопустимые единицы измерения для {quantity}");
            }

            var fromFactor = conversionFactors.First(f => f.Key == fromUnit).Value;
            var toFactor = conversionFactors.First(f => f.Key == toUnit).Value;

            return value * fromFactor / toFactor;
        }

        #endregion

        #region Автоматический выбор единиц измерения

        public static string AutoFormat(double value, PhysicalQuantity quantity)
        {
            var factors = GetUnitFactors(quantity);

            var bestUnit = factors
                .Where(f => Math.Abs(value) >= f.Value)
                .OrderByDescending(f => f.Value)
                .FirstOrDefault();

            if (bestUnit.Key == null)
            {
                return $"{value:F2} undefined";
            }

            var convertedValue = value / bestUnit.Value;
            return $"{convertedValue:F2} {bestUnit.Key}";
        }

        #endregion

        #region Вспомогательные методы форматирования

        public static class Form
        {
            private static string FormatValue(double value, string unit, string largerUnit, string smallerUnit,
                double largerThreshold, double smallerThreshold)
            {
                if (!string.IsNullOrEmpty(largerUnit) && Math.Abs(value) >= largerThreshold)
                {
                    return $"{value / largerThreshold:F2} {largerUnit}";
                }
                if (!string.IsNullOrEmpty(smallerUnit) && Math.Abs(value) >= smallerThreshold)
                {
                    return $"{value / smallerThreshold:F2} {smallerUnit}";
                }
                return $"{value:F2} {unit}";
            }

            public static string Format(double value, string baseUnit, string largerUnit = null,
                string smallerUnit = null, double largerThreshold = 0, double smallerThreshold = 0)
            {
                return FormatValue(value, baseUnit, largerUnit, smallerUnit, largerThreshold, smallerThreshold);
            }

            public static string Resistance(double value) { return Format(value, "Ω", "kΩ", "MΩ", 1e3, 1e6); }
            public static string Capacitance(double value) { return Format(value, "F", "μF", "nF", 1e-6, 1e-9); }
            public static string Frequency(double value) { return Format(value, "Hz", "kHz", "MHz", 1e3, 1e6); }
            public static string Voltage(double value) { return Format(value, "V", "kV", "mV", 1e3, 1e-3); }
            public static string Current(double value) { return Format(value, "A", "mA", "μA", 1e-3, 1e-6); }
            public static string Inductance(double value) { return Format(value, "H", "mH", "μH", 1e-3, 1e-6); }
            public static string Length(double value) { return Format(value, "m", "km", "mm", 1e3, 1e-3); }
            public static string Power(double value) { return Format(value, "W", "kW", "mW", 1e3, 1e-3); }
            public static string Pressure(double value) { return Format(value, "Pa", "kPa", "MPa", 1e3, 1e6); }
            public static string Time(double value) { return Format(value, "s", "ms", "μs", 1e-3, 1e-6); }
            public static string Force(double value) { return Format(value, "N", "kN", "MN", 1e3, 1e6); }
            public static string Energy(double value) { return Format(value, "J", "kJ", "MJ", 1e3, 1e6); }
            public static string Speed(double value) { return Format(value, "m/s", "km/h", "mph", 3.6, 2.23694); }
            public static string Area(double value) { return Format(value, "m²", "km²", "cm²", 1e6, 1e-4); }
            public static string Volume(double value) { return Format(value, "m³", "L", "mL", 1e-3, 1e-6); }
            public static string Mass(double value) { return Format(value, "kg", "t", "g", 1e3, 1e-3); }
            public static string Density(double value) { return Format(value, "kg/m³", "g/cm³", "kg/L", 1e3, 1); }
            public static string Flow(double value) { return Format(value, "m³/s", "L/s", "L/min", 1e-3, 60e-3); }
            public static string Conductance(double value) { return Format(value, "S", "mS", "μS", 1e-3, 1e-6); }
            public static string Resistivity(double value) { return Format(value, "Ω·m", "Ω·cm", "Ω·mm²/m", 1e-2, 1e-6); }
            public static string Permittivity(double value) { return Format(value, "F/m", "pF/m", "nF/m", 1e-12, 1e-9); }
            public static string Permeability(double value) { return Format(value, "H/m", "μH/m", "nH/m", 1e-6, 1e-9); }
            public static string MagneticField(double value) { return Format(value, "T", "mT", "μT", 1e-3, 1e-6); }
            public static string MagneticFlux(double value) { return Format(value, "Wb", "mWb", "μWb", 1e-3, 1e-6); }
            public static string LuminousFlux(double value) { return Format(value, "lm", "klm", null, 1e3, 0); }
            public static string Illuminance(double value) { return Format(value, "lx", "klx", null, 1e3, 0); }
            public static string SoundPressure(double value) { return Format(value, "Pa", "μPa", "dB", 1e-6, 1); }
            public static string SoundIntensity(double value) { return Format(value, "W/m²", "mW/m²", "μW/m²", 1e-3, 1e-6); }
            public static string SoundPower(double value) { return Format(value, "W", "mW", "μW", 1e-3, 1e-6); }
            public static string SoundLevel(double value) { return Format(value, "dB", null, null, 0, 0); }
            public static string Attenuation(double value) { return Format(value, "dB", "dBm", "Np", 1, 1); }
        }

        #endregion
    }
}
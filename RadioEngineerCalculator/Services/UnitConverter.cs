using System;
using System.Collections.Generic;
using System.Linq;

namespace RadioEngineerCalculator.Services
{
    /// <summary>
    /// Provides unit conversion and formatting capabilities for various physical quantities
    /// </summary>
    public static class UnitConverter
    {
        public record UnitInfo
        {
            public string Symbol { get; init; }
            public double Factor { get; init; }
            public string DisplayName { get; init; }

            public UnitInfo(string symbol, double factor, string? displayName = null)
            {
                Symbol = symbol;
                Factor = factor;
                DisplayName = displayName ?? symbol;
            }
        }

        private static class Constants
        {
            public const double Kilo = 1e3;
            public const double Mega = 1e6;
            public const double Giga = 1e9;
            public const double Tera = 1e12;
            public const double Milli = 1e-3;
            public const double Micro = 1e-6;
            public const double Nano = 1e-9;
            public const double Pico = 1e-12;
            public const double PI = Math.PI;
        }

        public enum PhysicalQuantity
        {
            Frequency,
            Resistance,
            Capacitance,
            Voltage,
            Current,
            Inductance,
            Length,
            Power,
            Pressure,
            Time,
            ReactivePower,
            Temperature,
            Angle,
            Force,
            Energy,
            Speed,
            Area,
            Volume,
            Mass,
            Density,
            Flow,
            Conductance,
            Resistivity,
            Permittivity,
            Permeability,
            MagneticField,
            MagneticFlux,
            LuminousFlux,
            Illuminance,
            SoundPressure,
            SoundIntensity,
            SoundPower,
            Loudness,
            SpectralDensity,
            CyclicFrequency,
            Wavelength,
            RadiationIntensity,
            ElectricField,
            Attenuation
        }

        private static readonly IReadOnlyDictionary<PhysicalQuantity, IReadOnlyList<UnitInfo>> UnitDefinitions =
            new Dictionary<PhysicalQuantity, IReadOnlyList<UnitInfo>>
            {
                [PhysicalQuantity.Frequency] = new[]
            {
                new UnitInfo("Hz", 1),
                new UnitInfo("kHz", Constants.Kilo),
                new UnitInfo("MHz", Constants.Mega),
                new UnitInfo("GHz", Constants.Giga),
                new UnitInfo("THz", Constants.Tera)
            },
                [PhysicalQuantity.Resistance] = new[]
            {
                new UnitInfo("Ω", 1),
                new UnitInfo("kΩ", Constants.Kilo),
                new UnitInfo("MΩ", Constants.Mega),
                new UnitInfo("mΩ", Constants.Milli),
                new UnitInfo("μΩ", Constants.Micro)
            },
                [PhysicalQuantity.Capacitance] = new[]
            {
                new UnitInfo("F", 1),
                new UnitInfo("mF", Constants.Milli),
                new UnitInfo("μF", Constants.Micro),
                new UnitInfo("nF", Constants.Nano),
                new UnitInfo("pF", Constants.Pico)
            },
                [PhysicalQuantity.Voltage] = new[]
            {
                new UnitInfo("V", 1),
                new UnitInfo("kV", Constants.Kilo),
                new UnitInfo("mV", Constants.Milli),
                new UnitInfo("μV", Constants.Micro),
                new UnitInfo("nV", Constants.Nano)
            },
                [PhysicalQuantity.Current] = new[]
            {
                new UnitInfo("A", 1),
                new UnitInfo("mA", Constants.Milli),
                new UnitInfo("μA", Constants.Micro),
                new UnitInfo("nA", Constants.Nano),
                new UnitInfo("pA", Constants.Pico)
            },
                [PhysicalQuantity.Inductance] = new[]
            {
                new UnitInfo("H", 1),
                new UnitInfo("mH", Constants.Milli),
                new UnitInfo("μH", Constants.Micro),
                new UnitInfo("nH", Constants.Nano),
                new UnitInfo("pH", Constants.Pico)
            },
                [PhysicalQuantity.Length] = new[]
            {
                new UnitInfo("m", 1),
                new UnitInfo("km", Constants.Kilo),
                new UnitInfo("cm", 0.01),
                new UnitInfo("mm", Constants.Milli),
                new UnitInfo("μm", Constants.Micro),
                new UnitInfo("nm", Constants.Nano)
            },
                [PhysicalQuantity.Power] = new[]
            {
                new UnitInfo("W", 1),
                new UnitInfo("kW", Constants.Kilo),
                new UnitInfo("MW", Constants.Mega),
                new UnitInfo("mW", Constants.Milli),
                new UnitInfo("μW", Constants.Micro)
            },
                [PhysicalQuantity.Pressure] = new[]
            {
                new UnitInfo("Pa", 1),
                new UnitInfo("kPa", Constants.Kilo),
                new UnitInfo("MPa", Constants.Mega),
                new UnitInfo("bar", 1e5),
                new UnitInfo("mbar", 1e2),
                new UnitInfo("atm", 101325)
            },
                [PhysicalQuantity.Time] = new[]
            {
                new UnitInfo("s", 1),
                new UnitInfo("ms", Constants.Milli),
                new UnitInfo("μs", Constants.Micro),
                new UnitInfo("ns", Constants.Nano),
                new UnitInfo("ps", Constants.Pico),
                new UnitInfo("min", 60),
                new UnitInfo("h", 3600)
            },
                [PhysicalQuantity.ReactivePower] = new[]
            {
                new UnitInfo("VAr", 1),
                new UnitInfo("kVAr", Constants.Kilo),
                new UnitInfo("mVAr", Constants.Milli)
            },
                [PhysicalQuantity.Temperature] = new[]
            {
                new UnitInfo("°C", 1),
                new UnitInfo("K", 1),
                new UnitInfo("°F", 1)
            },
                [PhysicalQuantity.Angle] = new[]
            {
                new UnitInfo("°", 1),
                new UnitInfo("rad", 180 / Constants.PI),
                new UnitInfo("grad", 0.9)
            },
                [PhysicalQuantity.Force] = new[]
            {
                new UnitInfo("N", 1),
                new UnitInfo("kN", Constants.Kilo),
                new UnitInfo("MN", Constants.Mega),
                new UnitInfo("lbf", 4.44822)
            },
                [PhysicalQuantity.Energy] = new[]
            {
                new UnitInfo("J", 1),
                new UnitInfo("kJ", Constants.Kilo),
                new UnitInfo("MJ", Constants.Mega),
                new UnitInfo("eV", 1.60218e-19),
                new UnitInfo("cal", 4.184)
            }
                // ... остальные определения опущены для краткости
            };

        public static class UnitFactors
        {
            public static IReadOnlyList<UnitInfo> GetUnitFactors(PhysicalQuantity quantity) =>
                UnitDefinitions.TryGetValue(quantity, out var units)
                    ? units
                    : throw new ArgumentException($"Invalid physical quantity: {quantity}", nameof(quantity));
        }

        /// <summary>
        /// Converts a value from one unit to another within the same physical quantity
        /// </summary>
        public static double Convert(double value, string fromUnit, string toUnit, PhysicalQuantity quantity)
        {
            ValidateConversionInput(value, fromUnit, toUnit, quantity, out var fromFactor, out var toFactor);

            return (quantity, fromUnit, toUnit) switch
            {
                var (_, from, to) when from == to => value,
                (PhysicalQuantity.Temperature, _, _) => ConvertTemperature(value, fromUnit, toUnit),
                (PhysicalQuantity.Angle, _, _) => ConvertAngle(value, fromUnit, toUnit),
                _ => value * fromFactor / toFactor
            };
        }

        private static void ValidateConversionInput(
            double value,
            string fromUnit,
            string toUnit,
            PhysicalQuantity quantity,
            out double fromFactor,
            out double toFactor)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException("Value cannot be NaN or Infinity", nameof(value));
            }

            var factors = UnitFactors.GetUnitFactors(quantity);
            var fromUnitInfo = factors.FirstOrDefault(u => u.Symbol == fromUnit);
            var toUnitInfo = factors.FirstOrDefault(u => u.Symbol == toUnit);

            if (fromUnitInfo == null || toUnitInfo == null)
            {
                var validUnits = string.Join(", ", factors.Select(u => u.Symbol));
                throw new ArgumentException(
                    $"Invalid units: {fromUnit}, {toUnit} for {quantity}. Valid units are: {validUnits}");
            }

            fromFactor = fromUnitInfo.Factor;
            toFactor = toUnitInfo.Factor;
        }

        private static double ConvertTemperature(double value, string fromUnit, string toUnit) =>
            (fromUnit, toUnit) switch
            {
                ("°C", "K") => value + 273.15,
                ("K", "°C") => value - 273.15,
                ("°F", "°C") => (value - 32) * 5 / 9,
                ("°C", "°F") => (value * 9 / 5) + 32,
                ("°F", "K") => (value - 32) * 5 / 9 + 273.15,
                ("K", "°F") => (value - 273.15) * 9 / 5 + 32,
                _ => value
            };

        private static double ConvertAngle(double value, string fromUnit, string toUnit) =>
            (fromUnit, toUnit) switch
            {
                ("°", "rad") => value * Constants.PI / 180,
                ("rad", "°") => value * 180 / Constants.PI,
                ("grad", "°") => value * 0.9,
                ("°", "grad") => value / 0.9,
                _ => value
            };

        /// <summary>
        /// Automatically formats a value with the most appropriate unit
        /// </summary>
        public static string AutoFormat(double value, PhysicalQuantity quantity)
        {
            var factors = UnitFactors.GetUnitFactors(quantity);
            var bestUnit = factors
                .Where(f => Math.Abs(value) >= f.Factor)
                .OrderByDescending(f => f.Factor)
                .FirstOrDefault();

            if (bestUnit == null)
            {
                return $"{value:F2} undefined";
            }

            var convertedValue = value / bestUnit.Factor;
            return $"{convertedValue:F2} {bestUnit.Symbol}";
        }

        /// <summary>
        /// Provides specialized formatting for different physical quantities
        /// </summary>
        public static class Formatter
        {
            private class FormatSpec // Changed from record to class
            {
                public string BaseUnit { get; }
                public double Threshold { get; }
                public string FormatString { get; }
                public Func<double, double>? TransformFunc { get; }

                public FormatSpec(string baseUnit, double threshold, string formatString = "F2", Func<double, double>? transformFunc = null)
                {
                    BaseUnit = baseUnit;
                    Threshold = threshold;
                    FormatString = formatString;
                    TransformFunc = transformFunc;
                }

                public string FormatValue(double value)
                {
                    var transformedValue = TransformFunc?.Invoke(value) ?? value;
                    return $"{transformedValue.ToString(FormatString)} {BaseUnit}";
                }
            }

            private static readonly Dictionary<PhysicalQuantity, FormatSpec[]> FormatSpecs = new()
            {
                [PhysicalQuantity.Resistance] = new[]
                {
            new FormatSpec("MΩ", Constants.Mega),
            new FormatSpec("kΩ", Constants.Kilo),
            new FormatSpec("Ω", 1),
            new FormatSpec("mΩ", Constants.Milli)
        },
                [PhysicalQuantity.Capacitance] = new[]
                {
            new FormatSpec("F", 1),
            new FormatSpec("mF", Constants.Milli),
            new FormatSpec("μF", Constants.Micro),
            new FormatSpec("nF", Constants.Nano),
            new FormatSpec("pF", Constants.Pico)
        },
                [PhysicalQuantity.Frequency] = new[]
                {
            new FormatSpec("THz", Constants.Tera),
            new FormatSpec("GHz", Constants.Giga),
            new FormatSpec("MHz", Constants.Mega),
            new FormatSpec("kHz", Constants.Kilo),
            new FormatSpec("Hz", 1)
        },
                [PhysicalQuantity.Voltage] = new[]
                {
            new FormatSpec("kV", Constants.Kilo),
            new FormatSpec("V", 1),
            new FormatSpec("mV", Constants.Milli),
            new FormatSpec("μV", Constants.Micro)
        },
                [PhysicalQuantity.Current] = new[]
                {
            new FormatSpec("A", 1),
            new FormatSpec("mA", Constants.Milli),
            new FormatSpec("μA", Constants.Micro),
            new FormatSpec("nA", Constants.Nano)
        },
                [PhysicalQuantity.Inductance] = new[]
                {
            new FormatSpec("H", 1),
            new FormatSpec("mH", Constants.Milli),
            new FormatSpec("μH", Constants.Micro),
            new FormatSpec("nH", Constants.Nano)
        },
                [PhysicalQuantity.Length] = new[]
                {
            new FormatSpec("km", Constants.Kilo),
            new FormatSpec("m", 1),
            new FormatSpec("cm", 0.01),
            new FormatSpec("mm", Constants.Milli),
            new FormatSpec("μm", Constants.Micro)
        },
                [PhysicalQuantity.Power] = new[]
                {
            new FormatSpec("MW", Constants.Mega),
            new FormatSpec("kW", Constants.Kilo),
            new FormatSpec("W", 1),
            new FormatSpec("mW", Constants.Milli)
        },
                [PhysicalQuantity.Pressure] = new[]
                {
            new FormatSpec("MPa", Constants.Mega),
            new FormatSpec("kPa", Constants.Kilo),
            new FormatSpec("Pa", 1),
            new FormatSpec("bar", 1e5),
            new FormatSpec("mbar", 1e2)
        },
                [PhysicalQuantity.Time] = new[]
                {
            new FormatSpec("h", 3600),
            new FormatSpec("min", 60),
            new FormatSpec("s", 1),
            new FormatSpec("ms", Constants.Milli),
            new FormatSpec("μs", Constants.Micro)
        },
                [PhysicalQuantity.ReactivePower] = new[]
                {
            new FormatSpec("MVAr", Constants.Mega),
            new FormatSpec("kVAr", Constants.Kilo),
            new FormatSpec("VAr", 1),
            new FormatSpec("mVAr", Constants.Milli)
        },
                [PhysicalQuantity.Force] = new[]
                {
            new FormatSpec("MN", Constants.Mega),
            new FormatSpec("kN", Constants.Kilo),
            new FormatSpec("N", 1)
        },
                [PhysicalQuantity.Energy] = new[]
                {
            new FormatSpec("MJ", Constants.Mega),
            new FormatSpec("kJ", Constants.Kilo),
            new FormatSpec("J", 1),
            new FormatSpec("eV", 1.60218e-19)
        },
                [PhysicalQuantity.Area] = new[]
                {
            new FormatSpec("km²", 1e6),
            new FormatSpec("m²", 1),
            new FormatSpec("cm²", 1e-4),
            new FormatSpec("mm²", 1e-6)
        },
                [PhysicalQuantity.Volume] = new[]
                {
            new FormatSpec("m³", 1),
            new FormatSpec("L", 1e-3),
            new FormatSpec("mL", 1e-6),
            new FormatSpec("μL", 1e-9)
        },
                [PhysicalQuantity.Mass] = new[]
                {
            new FormatSpec("t", 1e3),
            new FormatSpec("kg", 1),
            new FormatSpec("g", 1e-3),
            new FormatSpec("mg", 1e-6)
        },
                [PhysicalQuantity.MagneticField] = new[]
                {
            new FormatSpec("T", 1),
            new FormatSpec("mT", Constants.Milli),
            new FormatSpec("μT", Constants.Micro),
            new FormatSpec("nT", Constants.Nano)
        },
                [PhysicalQuantity.Wavelength] = new[]
                {
            new FormatSpec("m", 1),
            new FormatSpec("cm", 0.01),
            new FormatSpec("mm", Constants.Milli),
            new FormatSpec("μm", Constants.Micro),
            new FormatSpec("nm", Constants.Nano)
        },
                [PhysicalQuantity.Attenuation] = new[]
                {
                    new FormatSpec("dB", 1),
                    new FormatSpec("dB/m", 1, "F2", v => v / 100),
                    new FormatSpec("Np", 1, "F2", v => v * 8.686)
                }
            };

            public static string Format(double value, PhysicalQuantity quantity)
            {
                if (!FormatSpecs.TryGetValue(quantity, out var specs))
                    return $"{value:F2}";

                var spec = specs.FirstOrDefault(s => Math.Abs(value) >= s.Threshold)
                    ?? specs[specs.Length - 1]; // Replace array index syntax

                return spec.FormatValue(value);
            }

            // Специализированные методы форматирования для каждой физической величины
            public static string Resistance(double value) => Format(value, PhysicalQuantity.Resistance);
            public static string Capacitance(double value) => Format(value, PhysicalQuantity.Capacitance);
            public static string Frequency(double value) => Format(value, PhysicalQuantity.Frequency);
            public static string Voltage(double value) => Format(value, PhysicalQuantity.Voltage);
            public static string Current(double value) => Format(value, PhysicalQuantity.Current);
            public static string Inductance(double value) => Format(value, PhysicalQuantity.Inductance);
            public static string Length(double value) => Format(value, PhysicalQuantity.Length);
            public static string Power(double value) => Format(value, PhysicalQuantity.Power);
            public static string Time(double value) => Format(value, PhysicalQuantity.Time);
            public static string Force(double value) => Format(value, PhysicalQuantity.Force);
            public static string Energy(double value) => Format(value, PhysicalQuantity.Energy);
            public static string Area(double value) => Format(value, PhysicalQuantity.Area);
            public static string Volume(double value) => Format(value, PhysicalQuantity.Volume);
            public static string Mass(double value) => Format(value, PhysicalQuantity.Mass);
            public static string MagneticField(double value) => Format(value, PhysicalQuantity.MagneticField);
            public static string Wavelength(double value) => Format(value, PhysicalQuantity.Wavelength);
            public static string Attenuation(double value) => Format(value, PhysicalQuantity.Attenuation);

            // Специальные форматы для температуры
            public static string Temperature(double value, string unit = "°C")
            {
                return unit switch
                {
                    "K" => $"{value:F2} K",
                    "°F" => $"{value:F2} °F",
                    _ => $"{value:F2} °C"
                };
            }

            // Специальные форматы для углов
            public static string Angle(double value, string unit = "°")
            {
                return unit switch
                {
                    "rad" => $"{value:F4} rad",
                    "grad" => $"{value:F2} grad",
                    _ => $"{value:F2}°"
                };
            }

            // Метод для форматирования децибел
            public static string Decibels(double value, string? reference = null)
            {
                return reference is null
                    ? $"{value:F2} dB"
                    : $"{value:F2} dB{reference}";
            }
        }
    }
}
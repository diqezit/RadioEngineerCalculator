using System;
using System.Collections.Generic;
using System.Linq;

namespace RadioEngineerCalculator.Services
{
    /// <summary>
    /// Предоставляет возможности конвертации и форматирования единиц измерения для различных физических величин.
    /// </summary>
    public static class UnitConverter
    {
        /// <summary>
        /// Информация о единице измерения, включая её символ, коэффициент и отображаемое имя.
        /// </summary>
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

        /// <summary>
        /// Константы, используемые для преобразования единиц измерения.
        /// </summary>
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

        /// <summary>
        /// Перечисление физических величин, для которых возможна конвертация.
        /// </summary>
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

        /// <summary>
        /// Словарь, содержащий определения единиц измерения для каждой физической величины.
        /// </summary>
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

        /// <summary>
        /// Возвращает коэффициенты для единиц измерения заданной физической величины.
        /// </summary>
        public static class UnitFactors
        {
            public static IReadOnlyList<UnitInfo> GetUnitFactors(PhysicalQuantity quantity) =>
                UnitDefinitions.TryGetValue(quantity, out var units)
                    ? units
                    : throw new ArgumentException($"Invalid physical quantity: {quantity}", nameof(quantity));
        }

        /// <summary>
        /// Преобразует значение из одной единицы измерения в другую в рамках одной физической величины.
        /// </summary>
        /// <param name="value">Значение для преобразования.</param>
        /// <param name="fromUnit">Исходная единица измерения.</param>
        /// <param name="toUnit">Целевая единица измерения.</param>
        /// <param name="quantity">Физическая величина.</param>
        /// <returns>Преобразованное значение.</returns>
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

        /// <summary>
        /// Проверяет входные данные для конвертации и выбрасывает исключение, если данные некорректны.
        /// </summary>
        /// <param name="value">Значение для преобразования.</param>
        /// <param name="fromUnit">Исходная единица измерения.</param>
        /// <param name="toUnit">Целевая единица измерения.</param>
        /// <param name="quantity">Физическая величина.</param>
        /// <param name="fromFactor">Выходной параметр, содержащий коэффициент исходной единицы.</param>
        /// <param name="toFactor">Выходной параметр, содержащий коэффициент целевой единицы.</param>
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

        /// <summary>
        /// Преобразует температуру из одной единицы измерения в другую.
        /// </summary>
        /// <param name="value">Значение температуры.</param>
        /// <param name="fromUnit">Исходная единица измерения.</param>
        /// <param name="toUnit">Целевая единица измерения.</param>
        /// <returns>Преобразованное значение температуры.</returns>
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

        /// <summary>
        /// Преобразует угол из одной единицы измерения в другую.
        /// </summary>
        /// <param name="value">Значение угла.</param>
        /// <param name="fromUnit">Исходная единица измерения.</param>
        /// <param name="toUnit">Целевая единица измерения.</param>
        /// <returns>Преобразованное значение угла.</returns>
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
        /// Автоматически форматирует значение с наиболее подходящей единицей измерения.
        /// </summary>
        /// <param name="value">Значение для форматирования.</param>
        /// <param name="quantity">Физическая величина.</param>
        /// <returns>Отформатированная строка.</returns>
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
        /// Предоставляет специализированное форматирование для отоборажения результатов различных физических величин.
        /// </summary>
        public static class Formatter
        {
            private static readonly Dictionary<PhysicalQuantity, Func<double, string>> Formatters = new()
            {
                [PhysicalQuantity.Resistance] = value => Format(value, "Ω", (Constants.Mega, "MΩ"), (Constants.Kilo, "kΩ"), (1, "Ω"), (Constants.Milli, "mΩ")),
                [PhysicalQuantity.Capacitance] = value => Format(value, "F", (1, "F"), (Constants.Milli, "mF"), (Constants.Micro, "μF"), (Constants.Nano, "nF"), (Constants.Pico, "pF")),
                [PhysicalQuantity.Frequency] = value => Format(value, "Hz", (Constants.Tera, "THz"), (Constants.Giga, "GHz"), (Constants.Mega, "MHz"), (Constants.Kilo, "kHz"), (1, "Hz")),
                [PhysicalQuantity.Voltage] = value => Format(value, "V", (Constants.Kilo, "kV"), (1, "V"), (Constants.Milli, "mV"), (Constants.Micro, "μV")),
                [PhysicalQuantity.Current] = value => Format(value, "A", (1, "A"), (Constants.Milli, "mA"), (Constants.Micro, "μA"), (Constants.Nano, "nA")),
                [PhysicalQuantity.Inductance] = value => Format(value, "H", (1, "H"), (Constants.Milli, "mH"), (Constants.Micro, "μH"), (Constants.Nano, "nH")),
                [PhysicalQuantity.Length] = value => Format(value, "m", (Constants.Kilo, "km"), (1, "m"), (0.01, "cm"), (Constants.Milli, "mm"), (Constants.Micro, "μm")),
                [PhysicalQuantity.Power] = value => Format(value, "W", (Constants.Mega, "MW"), (Constants.Kilo, "kW"), (1, "W"), (Constants.Milli, "mW")),
                [PhysicalQuantity.Pressure] = value => Format(value, "Pa", (Constants.Mega, "MPa"), (Constants.Kilo, "kPa"), (1, "Pa"), (1e5, "bar"), (1e2, "mbar")),
                [PhysicalQuantity.Time] = value => Format(value, "s", (3600, "h"), (60, "min"), (1, "s"), (Constants.Milli, "ms"), (Constants.Micro, "μs")),
                [PhysicalQuantity.ReactivePower] = value => Format(value, "VAr", (Constants.Mega, "MVAr"), (Constants.Kilo, "kVAr"), (1, "VAr"), (Constants.Milli, "mVAr")),
                [PhysicalQuantity.Force] = value => Format(value, "N", (Constants.Mega, "MN"), (Constants.Kilo, "kN"), (1, "N")),
                [PhysicalQuantity.Energy] = value => Format(value, "J", (Constants.Mega, "MJ"), (Constants.Kilo, "kJ"), (1, "J"), (1.60218e-19, "eV")),
                [PhysicalQuantity.Area] = value => Format(value, "m²", (1e6, "km²"), (1, "m²"), (1e-4, "cm²"), (1e-6, "mm²")),
                [PhysicalQuantity.Volume] = value => Format(value, "m³", (1, "m³"), (1e-3, "L"), (1e-6, "mL"), (1e-9, "μL")),
                [PhysicalQuantity.Mass] = value => Format(value, "kg", (1e3, "t"), (1, "kg"), (1e-3, "g"), (1e-6, "mg")),
                [PhysicalQuantity.MagneticField] = value => Format(value, "T", (1, "T"), (Constants.Milli, "mT"), (Constants.Micro, "μT"), (Constants.Nano, "nT")),
                [PhysicalQuantity.Wavelength] = value => Format(value, "m", (1, "m"), (0.01, "cm"), (Constants.Milli, "mm"), (Constants.Micro, "μm"), (Constants.Nano, "nm")),
                [PhysicalQuantity.Attenuation] = value => Format(value, "dB", (1, "dB"), (100.0, "dB/m"), (8.686, "Np")),
            };

            /// <summary>
            /// Форматирует значение с учетом физической величины.
            /// </summary>
            /// <param name="value">Значение для форматирования.</param>
            /// <param name="quantity">Физическая величина.</param>
            /// <returns>Отформатированная строка.</returns>
            public static string Format(double value, PhysicalQuantity quantity)
            {
                if (Formatters.TryGetValue(quantity, out var formatter))
                    return formatter(value);
                return $"{value:F2}"; // Default formatting
            }

            /// <summary>
            /// Форматирует значение с учетом заданных порогов и единиц измерения.
            /// </summary>
            /// <param name="value">Значение для форматирования.</param>
            /// <param name="baseUnit">Базовая единица измерения.</param>
            /// <param name="formats">Кортежи, содержащие пороги и соответствующие единицы измерения.</param>
            /// <returns>Отформатированная строка.</returns>
            private static string Format(double value, string baseUnit, params (double threshold, string unit)[] formats)
            {
                foreach (var (threshold, unit) in formats)
                {
                    if (Math.Abs(value) >= threshold)
                        return $"{(value / threshold):F2} {unit}";
                }
                return $"{value:F2} {baseUnit}";
            }

            /// <summary>
            /// Специальные форматы для температуры.
            /// </summary>
            /// <param name="value">Значение температуры.</param>
            /// <param name="unit">Единица измерения (по умолчанию "°C").</param>
            /// <returns>Отформатированная строка.</returns>
            public static string Temperature(double value, string unit = "°C")
            {
                return unit switch
                {
                    "K" => $"{value:F2} K",
                    "°F" => $"{value:F2} °F",
                    _ => $"{value:F2} °C"
                };
            }

            /// <summary>
            /// Специальные форматы для углов.
            /// </summary>
            /// <param name="value">Значение угла.</param>
            /// <param name="unit">Единица измерения (по умолчанию "°").</param>
            /// <returns>Отформатированная строка.</returns>
            public static string Angle(double value, string unit = "°")
            {
                return unit switch
                {
                    "rad" => $"{value:F4} rad",
                    "grad" => $"{value:F2} grad",
                    _ => $"{value:F2}°"
                };
            }

            /// <summary>
            /// Метод для форматирования децибел.
            /// </summary>
            /// <param name="value">Значение в децибелах.</param>
            /// <param name="reference">Опциональное значение, указывающее на ссылку (например, "dBm").</param>
            /// <returns>Отформатированная строка.</returns>
            public static string Decibels(double value, string? reference = null)
            {
                return reference is null
                    ? $"{value:F2} dB"
                    : $"{value:F2} dB{reference}";
            }
        }
    }
}
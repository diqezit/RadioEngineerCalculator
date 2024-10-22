using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using static RadioEngineerCalculator.Services.UnitC;

namespace RadioEngineerCalculator.Services
{
    public class FiltersCalculationService
    {
        public enum FilterType
        {
            LowPass,
            HighPass,
            BandPass,
            BandStop
        }

        public FilterResults CalculateFilterResults(FilterInputValues inputValues)
        {
            try
            {
                ValidateInputValues(inputValues);

                // Конвертируем все значения в базовые единицы измерения
                double capacitanceInFarads = Convert(inputValues.Capacitance, inputValues.CapacitanceUnit, "F", PhysicalQuantity.Capacitance);
                double inductanceInHenries = Convert(inputValues.Inductance, inputValues.InductanceUnit, "H", PhysicalQuantity.Inductance);
                double resistanceInOhms = Convert(inputValues.Resistance, inputValues.ResistanceUnit, "Ω", PhysicalQuantity.Resistance);
                double frequencyInHz = Convert(inputValues.Frequency, inputValues.FrequencyUnit, "Hz", PhysicalQuantity.Frequency);

                var cutoffFrequency = CalculateCutoffFrequency(inductanceInHenries, capacitanceInFarads);
                var qualityFactor = CalculateQualityFactor(resistanceInOhms, inductanceInHenries, capacitanceInFarads);
                var bandwidth = CalculateBandwidth(cutoffFrequency, qualityFactor);

                double safeFrequency = EnsureSafeFrequency(frequencyInHz, cutoffFrequency);

                var impedance = CalculateImpedance(safeFrequency, resistanceInOhms, inductanceInHenries, capacitanceInFarads);
                var phaseShift = CalculatePhaseShift(inputValues.FilterType, safeFrequency, cutoffFrequency, bandwidth);
                var groupDelay = CalculateGroupDelay(inputValues.FilterType, safeFrequency, cutoffFrequency, bandwidth);
                var attenuation = CalculateAttenuation(inputValues.FilterType, safeFrequency, cutoffFrequency, bandwidth);

                return new FilterResults
                {
                    FilterType = inputValues.FilterType,
                    CutoffFrequency = Convert(cutoffFrequency, "Hz", inputValues.FrequencyUnit, PhysicalQuantity.Frequency),
                    QualityFactor = qualityFactor,
                    Bandwidth = Convert(bandwidth, "Hz", inputValues.FrequencyUnit, PhysicalQuantity.Frequency),
                    Impedance = impedance.Magnitude,
                    PhaseShift = phaseShift,
                    GroupDelay = groupDelay,
                    Attenuation = attenuation,
                    FrequencyResponse = CalculateFrequencyResponse(inputValues.FilterType, safeFrequency, cutoffFrequency, bandwidth, inputValues.FrequencyUnit)
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при расчете параметров фильтра: {ex.Message}", ex);
            }
        }

        private double ConvertFrequencyToHz(double frequency, string unit)
        {
            if (string.IsNullOrEmpty(unit))
            {
                throw new ArgumentException("Единица измерения частоты не указана");
            }

            try
            {
                return Convert(frequency, unit, "Hz", PhysicalQuantity.Frequency);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"Недопустимая единица измерения для частоты: {unit}");
            }
        }


        private double EnsureSafeFrequency(double frequency, double cutoffFrequency)
        {
            const double MIN_RATIO = 1e-6;
            const double MAX_RATIO = 1e6;

            if (frequency < cutoffFrequency * MIN_RATIO)
                return cutoffFrequency * MIN_RATIO;
            if (frequency > cutoffFrequency * MAX_RATIO)
                return cutoffFrequency * MAX_RATIO;

            return frequency;
        }

        public double CalculateMagnitudeResponse(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth)
        {
            if (frequency <= 0 || cutoffFrequency <= 0 || bandwidth <= 0)
            {
                throw new ArgumentException("Частота, частота среза и полоса пропускания должны быть положительными числами");
            }

            double normalizedFrequency = frequency / cutoffFrequency;
            double response;

            switch (filterType)
            {
                case FilterType.LowPass:
                    response = 1 / Math.Sqrt(1 + Math.Pow(normalizedFrequency, 2));
                    break;
                case FilterType.HighPass:
                    response = normalizedFrequency / Math.Sqrt(1 + Math.Pow(normalizedFrequency, 2));
                    break;
                case FilterType.BandPass:
                    response = CalculateBandPassMagnitude(normalizedFrequency, cutoffFrequency, bandwidth);
                    break;
                case FilterType.BandStop:
                    response = CalculateBandStopMagnitude(normalizedFrequency, cutoffFrequency, bandwidth);
                    break;
                default:
                    throw new ArgumentException("Неподдерживаемый тип фильтра", nameof(filterType));
            }

            return response;
        }

        public double CalculatePhaseShift(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth)
        {
            double normalizedFrequency = frequency / cutoffFrequency;
            double phase;

            switch (filterType)
            {
                case FilterType.LowPass:
                    phase = -Math.Atan(normalizedFrequency);
                    break;
                case FilterType.HighPass:
                    phase = Math.PI / 2 - Math.Atan(1 / normalizedFrequency);
                    break;
                case FilterType.BandPass:
                    phase = CalculateBandPassPhase(normalizedFrequency, cutoffFrequency, bandwidth);
                    break;
                case FilterType.BandStop:
                    phase = CalculateBandStopPhase(normalizedFrequency, cutoffFrequency, bandwidth);
                    break;
                default:
                    throw new ArgumentException("Неподдерживаемый тип фильтра", nameof(filterType));
            }

            return phase;
        }

        public double CalculateGroupDelay(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth)
        {
            double normalizedFrequency = frequency / cutoffFrequency;
            double delay;

            switch (filterType)
            {
                case FilterType.LowPass:
                    delay = 1 / (2 * Math.PI * frequency * (1 + Math.Pow(normalizedFrequency, 2)));
                    break;
                case FilterType.HighPass:
                    delay = 1 / (2 * Math.PI * frequency * (1 + Math.Pow(1 / normalizedFrequency, 2)));
                    break;
                case FilterType.BandPass:
                case FilterType.BandStop:
                    delay = CalculateBandGroupDelay(normalizedFrequency, cutoffFrequency, bandwidth);
                    break;
                default:
                    throw new ArgumentException("Неподдерживаемый тип фильтра", nameof(filterType));
            }

            return delay;
        }

        public double CalculateAttenuation(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth)
        {
            double magnitudeResponse = CalculateMagnitudeResponse(filterType, frequency, cutoffFrequency, bandwidth);
            return -20 * Math.Log10(magnitudeResponse);
        }

        private static double CalculateCutoffFrequency(double inductance, double capacitance)
        {
            if (inductance <= 0 || capacitance <= 0)
            {
                throw new ArgumentException("Индуктивность и емкость должны быть положительными числами");
            }
            return 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));
        }

        private static double CalculateQualityFactor(double resistance, double inductance, double capacitance)
        {
            if (resistance <= 0 || inductance <= 0 || capacitance <= 0)
            {
                throw new ArgumentException("Сопротивление, индуктивность и емкость должны быть положительными числами");
            }
            return (1 / resistance) * Math.Sqrt(inductance / capacitance);
        }

        private static double CalculateBandwidth(double cutoffFrequency, double qualityFactor)
        {
            return cutoffFrequency / qualityFactor;
        }

        private static Complex CalculateImpedance(double frequency, double resistance, double inductance, double capacitance)
        {
            double omega = 2 * Math.PI * frequency;
            return new Complex(resistance, omega * inductance - 1 / (omega * capacitance));
        }

        private List<Point> CalculateFrequencyResponse(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth, string frequencyUnit)
        {
            const int pointCount = 1000;
            double startFreq = Math.Max(frequency / 100, 1e-6 * cutoffFrequency);
            double endFreq = Math.Min(frequency * 100, 1e6 * cutoffFrequency);
            var response = new List<Point>(pointCount);

            for (int i = 0; i < pointCount; i++)
            {
                double freq = Math.Pow(10, Math.Log10(startFreq) + (Math.Log10(endFreq) - Math.Log10(startFreq)) * i / (pointCount - 1));
                double gain = CalculateMagnitudeResponse(filterType, freq, cutoffFrequency, bandwidth);
                response.Add(new Point
                {
                    X = Convert(freq, "Hz", frequencyUnit, PhysicalQuantity.Frequency),
                    Y = 20 * Math.Log10(gain)
                });
            }

            return response;
        }

        private static void ValidateUnit(string unit, PhysicalQuantity quantity)
        {
            if (!UnitFactors[quantity].ContainsKey(unit))
                throw new ArgumentException($"Недопустимая единица измерения для {quantity}: {unit}");
        }

        private static double CalculateBandPassMagnitude(double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            return 1 / Math.Sqrt(1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2));
        }

        private static double CalculateBandStopMagnitude(double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            return Math.Abs(q * (normalizedFrequency - 1 / normalizedFrequency)) / Math.Sqrt(1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2));
        }

        private static double CalculateBandPassPhase(double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            return Math.Atan(q * (normalizedFrequency - 1 / normalizedFrequency));
        }

        private static double CalculateBandStopPhase(double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            return -Math.Atan(q * (normalizedFrequency - 1 / normalizedFrequency));
        }

        private static double CalculateBandGroupDelay(double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            return q / (Math.PI * cutoffFrequency * (1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2)));
        }

        private static void ValidateInputValues(FilterInputValues inputValues)
        {
            if (inputValues == null)
                throw new ArgumentNullException(nameof(inputValues));

            if (inputValues.Capacitance <= 0 || inputValues.Inductance <= 0 || inputValues.Resistance <= 0 || inputValues.Frequency <= 0)
                throw new ArgumentException("Все значения должны быть положительными.");

            if (string.IsNullOrEmpty(inputValues.FrequencyUnit))
                throw new ArgumentException("Единица измерения частоты не указана.");

            // Проверка допустимости единиц измерения
            ValidateUnit(inputValues.CapacitanceUnit, PhysicalQuantity.Capacitance);
            ValidateUnit(inputValues.InductanceUnit, PhysicalQuantity.Inductance);
            ValidateUnit(inputValues.ResistanceUnit, PhysicalQuantity.Resistance);
            ValidateUnit(inputValues.FrequencyUnit, PhysicalQuantity.Frequency);
        }
    }

    public class FilterInputValues
    {
        public FiltersCalculationService.FilterType FilterType { get; set; }
        public double Frequency { get; set; }
        public double Capacitance { get; set; }
        public double Inductance { get; set; }
        public double Resistance { get; set; }
        public string ResistanceUnit { get; set; }
        public string InductanceUnit { get; set; }
        public string FrequencyUnit { get; set; }
        public string CapacitanceUnit { get; set; }
    }

    public class FilterResults
    {
        public FiltersCalculationService.FilterType FilterType { get; set; }
        public double CutoffFrequency { get; set; }
        public double QualityFactor { get; set; }
        public double Bandwidth { get; set; }
        public double Impedance { get; set; }
        public double PhaseShift { get; set; }
        public double GroupDelay { get; set; }
        public double Attenuation { get; set; }
        public List<Point> FrequencyResponse { get; set; }
    }

    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
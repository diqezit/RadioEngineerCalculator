using System;
using System.Numerics;
using static RadioEngineerCalculator.Services.FiltersCalculationService;

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
            var cutoffFrequency = CalculateFilterCutoffFrequency(inputValues);
            var qualityFactor = CalculateQualityFactor(inputValues);
            var bandwidth = CalculateBandwidth(cutoffFrequency, qualityFactor);
            var impedance = CalculateFilterImpedance(inputValues);
            var phaseShift = CalculateFilterPhaseResponse(inputValues.FilterType, inputValues.Frequency, cutoffFrequency, bandwidth);
            var groupDelay = CalculateGroupDelay(inputValues.FilterType, inputValues.Frequency, cutoffFrequency, bandwidth);
            var attenuation = CalculateFilterAttenuation(inputValues.FilterType, inputValues.Frequency, cutoffFrequency, bandwidth);

            return new FilterResults
            {
                FilterType = inputValues.FilterType,
                CutoffFrequency = cutoffFrequency,
                QualityFactor = qualityFactor,
                Bandwidth = bandwidth,
                Impedance = impedance.Magnitude,
                PhaseShift = phaseShift,
                GroupDelay = groupDelay,
                Attenuation = attenuation
            };
        }

        public double CalculateFilterMagnitudeResponse(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth)
        {
            double normalizedFrequency = frequency / cutoffFrequency;
            switch (filterType)
            {
                case FilterType.LowPass:
                    return 1 / Math.Sqrt(1 + Math.Pow(normalizedFrequency, 2));
                case FilterType.HighPass:
                    return normalizedFrequency / Math.Sqrt(1 + Math.Pow(normalizedFrequency, 2));
                case FilterType.BandPass:
                    double q = cutoffFrequency / bandwidth;
                    return 1 / Math.Sqrt(1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2));
                case FilterType.BandStop:
                    q = cutoffFrequency / bandwidth;
                    return Math.Abs(q * (normalizedFrequency - 1 / normalizedFrequency)) / Math.Sqrt(1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2));
                default:
                    throw new ArgumentException("Unsupported filter type");
            }
        }

        private double CalculateFilterCutoffFrequency(FilterInputValues inputValues)
        {
            switch (inputValues.FilterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                    return 1 / (2 * Math.PI * Math.Sqrt(inputValues.Inductance * inputValues.Capacitance));
                case FilterType.BandPass:
                case FilterType.BandStop:
                    return 1 / (2 * Math.PI * Math.Sqrt(inputValues.Inductance * inputValues.Capacitance));
                default:
                    throw new ArgumentException("Unsupported filter type");
            }
        }

        private double CalculateQualityFactor(FilterInputValues inputValues)
        {
            return (1 / inputValues.Resistance) * Math.Sqrt(inputValues.Inductance / inputValues.Capacitance);
        }

        private double CalculateBandwidth(double cutoffFrequency, double qualityFactor)
        {
            return cutoffFrequency / qualityFactor;
        }

        private Complex CalculateFilterImpedance(FilterInputValues inputValues)
        {
            double omega = 2 * Math.PI * inputValues.Frequency;
            return new Complex(inputValues.Resistance, omega * inputValues.Inductance - 1 / (omega * inputValues.Capacitance));
        }

        public double CalculateFilterPhaseResponse(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth)
        {
            double normalizedFrequency = frequency / cutoffFrequency;
            switch (filterType)
            {
                case FilterType.LowPass:
                    return -Math.Atan(normalizedFrequency);
                case FilterType.HighPass:
                    return Math.PI / 2 - Math.Atan(1 / normalizedFrequency);
                case FilterType.BandPass:
                    double q = cutoffFrequency / bandwidth;
                    return Math.Atan(q * (normalizedFrequency - 1 / normalizedFrequency));
                case FilterType.BandStop:
                    q = cutoffFrequency / bandwidth;
                    return -Math.Atan(q * (normalizedFrequency - 1 / normalizedFrequency));
                default:
                    throw new ArgumentException("Unsupported filter type");
            }
        }

        public double CalculateGroupDelay(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth)
        {
            double normalizedFrequency = frequency / cutoffFrequency;
            switch (filterType)
            {
                case FilterType.LowPass:
                    return 1 / (2 * Math.PI * frequency * (1 + Math.Pow(normalizedFrequency, 2)));
                case FilterType.HighPass:
                    return 1 / (2 * Math.PI * frequency * (1 + Math.Pow(1 / normalizedFrequency, 2)));
                case FilterType.BandPass:
                case FilterType.BandStop:
                    double q = cutoffFrequency / bandwidth;
                    return q / (Math.PI * cutoffFrequency * (1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2)));
                default:
                    throw new ArgumentException("Unsupported filter type");
            }
        }

        public double CalculateFilterAttenuation(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth)
        {
            double normalizedFrequency = frequency / cutoffFrequency;
            switch (filterType)
            {
                case FilterType.LowPass:
                    return -20 * Math.Log10(1 / Math.Sqrt(1 + Math.Pow(normalizedFrequency, 2)));
                case FilterType.HighPass:
                    return -20 * Math.Log10(normalizedFrequency / Math.Sqrt(1 + Math.Pow(normalizedFrequency, 2)));
                case FilterType.BandPass:
                    double q = cutoffFrequency / bandwidth;
                    return -20 * Math.Log10(1 / Math.Sqrt(1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2)));
                case FilterType.BandStop:
                    q = cutoffFrequency / bandwidth;
                    return -20 * Math.Log10(Math.Abs(q * (normalizedFrequency - 1 / normalizedFrequency)) / Math.Sqrt(1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2)));
                default:
                    throw new ArgumentException("Unsupported filter type");
            }
        }
    }

    public class FilterInputValues
    {
        public FilterType FilterType { get; set; }
        public double Frequency { get; set; }
        public double Capacitance { get; set; }
        public double Inductance { get; set; }
        public double Resistance { get; set; }
        public double PassbandRipple { get; set; }
        public double StopbandAttenuation { get; set; }
        public double StopbandFrequency { get; set; }
        public string StopbandFrequencyUnit { get; set; }
        public string ResistanceUnit { get; set; }
        public string InductanceUnit { get; set; }
        public string FrequencyUnit { get; set; }
        public string CapacitanceUnit { get; set; }
    }

    public class FilterResults
    {
        public FilterType FilterType { get; set; }
        public double CutoffFrequency { get; set; }
        public double QualityFactor { get; set; }
        public double Bandwidth { get; set; }
        public double Impedance { get; set; }
        public double PhaseShift { get; set; }
        public double GroupDelay { get; set; }
        public double Attenuation { get; set; }
        public int FilterOrder { get; set; }
    }
}

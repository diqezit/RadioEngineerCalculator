using System;
using static RadioEngineerCalculator.Services.FiltersCalculationService;

namespace RadioEngineerCalculator.Services
{
    public class FiltersCalculationService
    {
        #region Enums

        public enum FilterType
        {
            LowPass,
            HighPass,
            BandPass,
            BandStop,
            RC,
            RL,
            Pi,
            Trap,
            Quartz,
            PassiveBandPass,
            PassiveBandStop
        }

        #endregion

        #region Public Methods

        public double CalculateFilterCutoffFrequency(FilterType filterType, double resistance, double inductance, double capacitance)
        {
            switch (filterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                    return 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));
                case FilterType.BandPass:
                case FilterType.BandStop:
                    return 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));
                case FilterType.RC:
                    return 1 / (2 * Math.PI * resistance * capacitance);
                case FilterType.RL:
                    return resistance / (2 * Math.PI * inductance);
                case FilterType.Quartz:
                    return 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));
                default:
                    throw new ArgumentException("Unsupported filter type");
            }
        }

        public double CalculateLowPassFilterCutoffFrequency(double resistance, double capacitance)
            => 1 / (2 * Math.PI * resistance * capacitance);

        public double CalculateHighPassFilterCutoffFrequency(double resistance, double inductance)
            => resistance / (2 * Math.PI * inductance);

        public double CalculateBandPassFilterCenterFrequency(double inductance, double capacitance)
            => 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));

        public double CalculateBandStopFilterCenterFrequency(double inductance, double capacitance)
            => 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));

        public double CalculatePiFilterCutoffFrequency(double inductance, double capacitance)
            => 1 / (Math.PI * Math.Sqrt(inductance * capacitance));

        public double CalculateTrapFilterCutoffFrequency(double inductance, double capacitance)
            => 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));

        public double CalculateQuartzFilterResonantFrequency(double inductance, double capacitance)
            => 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));

        public double CalculateQualityFactor(FilterType filterType, double inductance, double capacitance, double resistance)
        {
            switch (filterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                case FilterType.BandPass:
                case FilterType.BandStop:
                    return (1 / resistance) * Math.Sqrt(inductance / capacitance);
                case FilterType.RC:
                    return 1 / (2 * Math.PI * resistance * capacitance);
                case FilterType.RL:
                    return (2 * Math.PI * inductance) / resistance;
                case FilterType.Quartz:
                    // For quartz crystals, Q factor is typically very high, often in the range of 10^4 to 10^6
                    return 100000; // This is a simplified approximation
                default:
                    throw new ArgumentException("Unsupported filter type");
            }
        }

        public double CalculateFilterImpedance(FilterType filterType, double resistance, double inductance, double capacitance, double frequency)
        {
            double omega = 2 * Math.PI * frequency;
            switch (filterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                    return Math.Sqrt(Math.Pow(resistance, 2) + Math.Pow(omega * inductance - 1 / (omega * capacitance), 2));
                case FilterType.BandPass:
                case FilterType.BandStop:
                    return Math.Sqrt(Math.Pow(resistance, 2) + Math.Pow(omega * inductance - 1 / (omega * capacitance), 2));
                case FilterType.RC:
                    return Math.Sqrt(Math.Pow(resistance, 2) + Math.Pow(1 / (omega * capacitance), 2));
                case FilterType.RL:
                    return Math.Sqrt(Math.Pow(resistance, 2) + Math.Pow(omega * inductance, 2));
                case FilterType.Quartz:
                    // For quartz crystals, impedance calculation is more complex and depends on the specific crystal model
                    throw new NotImplementedException("Quartz crystal impedance calculation not implemented");
                default:
                    throw new ArgumentException("Unsupported filter type");
            }
        }

        public double CalculateBandwidth(double cutoffFrequency, double qualityFactor)
        {
            return cutoffFrequency / qualityFactor;
        }

        public int CalculateFilterOrder(FilterType filterType, double attenuationAtStopband, double passbandRipple, double cutoffFrequency, double stopbandFrequency)
        {
            // Примерная реализация для фильтра Баттерворта
            return (int)Math.Ceiling((Math.Log10(Math.Pow(10, attenuationAtStopband / 10) - 1) /
                                     (2 * Math.Log10(cutoffFrequency / stopbandFrequency))));
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
                    throw new ArgumentException("Unsupported filter type for magnitude response calculation");
            }
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
                    throw new ArgumentException("Unsupported filter type for phase response calculation");
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
                    throw new ArgumentException("Unsupported filter type for attenuation calculation");
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
                    throw new ArgumentException("Unsupported filter type for group delay calculation");
            }
        }

        #endregion

        #region Private Methods

        private double CalculateLowPassMagnitudeResponse(double frequency, double cutoffFrequency)
            => 1 / Math.Sqrt(1 + Math.Pow(frequency / cutoffFrequency, 2));

        private double CalculateHighPassMagnitudeResponse(double frequency, double cutoffFrequency)
            => 1 / Math.Sqrt(1 + Math.Pow(cutoffFrequency / frequency, 2));

        private double CalculateBandPassMagnitudeResponse(double frequency, double centerFrequency, double bandwidth)
        {
            double normalizedFreq = frequency / centerFrequency;
            double normalizedBandwidth = bandwidth / centerFrequency;
            return 1 / Math.Sqrt(1 + Math.Pow((normalizedFreq - 1 / normalizedFreq) / normalizedBandwidth, 2));
        }

        private double CalculateBandStopMagnitudeResponse(double frequency, double centerFrequency, double bandwidth)
        {
            double normalizedFreq = frequency / centerFrequency;
            double normalizedBandwidth = bandwidth / centerFrequency;
            return Math.Sqrt(1 - 1 / (1 + Math.Pow((normalizedFreq - 1 / normalizedFreq) / normalizedBandwidth, 2)));
        }

        private double CalculateLowPassPhaseResponse(double frequency, double cutoffFrequency)
            => -Math.Atan(frequency / cutoffFrequency);

        private double CalculateHighPassPhaseResponse(double frequency, double cutoffFrequency)
            => Math.PI / 2 - Math.Atan(frequency / cutoffFrequency);

        private double CalculateBandPassPhaseResponse(double frequency, double centerFrequency, double bandwidth)
        {
            double normalizedFreq = frequency / centerFrequency;
            double normalizedBandwidth = bandwidth / centerFrequency;
            return Math.Atan((normalizedFreq - 1 / normalizedFreq) / normalizedBandwidth);
        }

        private double CalculateBandStopPhaseResponse(double frequency, double centerFrequency, double bandwidth)
        {
            double normalizedFreq = frequency / centerFrequency;
            double normalizedBandwidth = bandwidth / centerFrequency;
            return -Math.Atan((normalizedFreq - 1 / normalizedFreq) / normalizedBandwidth);
        }

        #endregion
    }

    #region Data Classes

    public class FilterInputValues
    {
        public FilterType FilterType { get; set; }
        public double Frequency { get; set; }
        public string FrequencyUnit { get; set; }
        public double Capacitance { get; set; }
        public string CapacitanceUnit { get; set; }
        public double Inductance { get; set; }
        public string InductanceUnit { get; set; }
        public double Resistance { get; set; }
        public string ResistanceUnit { get; set; }
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
    }

    #endregion
}
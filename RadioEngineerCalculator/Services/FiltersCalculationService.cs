using RadioEngineerCalculator.Infos;
using System;
using System.Numerics;
using static RadioEngineerCalculator.Services.FiltersCalculationService;
using RadioEngineerCalculator.ViewModel;

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

        public double RecalculateCapacitance(FilterInputValues inputValues)
        {
            switch (inputValues.FilterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                case FilterType.BandPass:
                case FilterType.BandStop:
                    return 1 / (Math.Pow(2 * Math.PI * inputValues.Frequency, 2) * inputValues.Inductance);
                case FilterType.RC:
                    return 1 / (2 * Math.PI * inputValues.Frequency * inputValues.Resistance);
                case FilterType.Pi:
                case FilterType.Trap:
                    return 1 / (Math.Pow(2 * Math.PI * inputValues.Frequency, 2) * inputValues.Inductance * 2);
                case FilterType.Quartz:
                    return 1 / (4 * Math.Pow(Math.PI * inputValues.Frequency, 2) * inputValues.Inductance);
                default:
                    throw new ArgumentException("Unsupported filter type for capacitance recalculation");
            }
        }

        public double RecalculateInductance(FilterInputValues inputValues)
        {
            switch (inputValues.FilterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                case FilterType.BandPass:
                case FilterType.BandStop:
                    return 1 / (Math.Pow(2 * Math.PI * inputValues.Frequency, 2) * inputValues.Capacitance);
                case FilterType.RL:
                    return inputValues.Resistance / (2 * Math.PI * inputValues.Frequency);
                case FilterType.Pi:
                case FilterType.Trap:
                    return 1 / (2 * Math.Pow(Math.PI * inputValues.Frequency, 2) * inputValues.Capacitance);
                case FilterType.Quartz:
                    return 1 / (4 * Math.Pow(Math.PI * inputValues.Frequency, 2) * inputValues.Capacitance);
                default:
                    throw new ArgumentException("Unsupported filter type for inductance recalculation");
            }
        }


        public double RecalculateResistance(FilterInputValues inputValues)
        {
            switch (inputValues.FilterType)
            {
                case FilterType.RC:
                    return 1 / (2 * Math.PI * inputValues.Frequency * inputValues.Capacitance);
                case FilterType.RL:
                    return 2 * Math.PI * inputValues.Frequency * inputValues.Inductance;
                case FilterType.LowPass:
                case FilterType.HighPass:
                case FilterType.BandPass:
                case FilterType.BandStop:
                case FilterType.Pi:
                case FilterType.Trap:
                    return Math.Sqrt(inputValues.Inductance / inputValues.Capacitance);
                case FilterType.Quartz:
                    return 2 * Math.PI * inputValues.Frequency * inputValues.Inductance / inputValues.QualityFactor;
                default:
                    throw new ArgumentException("Unsupported filter type for resistance recalculation");
            }
        }

        public double CalculateFilterCutoffFrequency(FilterType filterType, double resistance, double inductance, double capacitance)
        {
            switch (filterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                case FilterType.BandPass:
                case FilterType.BandStop:
                case FilterType.Quartz:
                    return 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));
                case FilterType.RC:
                    return 1 / (2 * Math.PI * resistance * capacitance);
                case FilterType.RL:
                    return resistance / (2 * Math.PI * inductance);
                case FilterType.Pi:
                    return 1 / (Math.PI * Math.Sqrt(inductance * capacitance));
                case FilterType.Trap:
                    return 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));
                default:
                    throw new ArgumentException("Unsupported filter type");
            }
        }

        private double CalculateQualityFactor(FilterInputValues inputValues)
        {
            switch (inputValues.FilterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                case FilterType.BandPass:
                case FilterType.BandStop:
                    return (1 / inputValues.Resistance) * Math.Sqrt(inputValues.Inductance / inputValues.Capacitance);
                case FilterType.RC:
                    return 1 / (2 * Math.PI * inputValues.Resistance * inputValues.Capacitance * inputValues.Frequency);
                case FilterType.RL:
                    return (2 * Math.PI * inputValues.Frequency * inputValues.Inductance) / inputValues.Resistance;
                case FilterType.Quartz:
                    return CalculateQuartzQualityFactor(inputValues.Inductance, inputValues.Capacitance, inputValues.Resistance);
                case FilterType.Pi:
                case FilterType.Trap:
                    return Math.Sqrt(inputValues.Inductance / inputValues.Capacitance) / inputValues.Resistance;
                default:
                    throw new ArgumentException("Unsupported filter type");
            }
        }

        private double CalculateQuartzQualityFactor(double inductance, double capacitance, double resistance)
        {
            double resonantFrequency = 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));
            return (2 * Math.PI * resonantFrequency * inductance) / resistance;
        }

        public Complex CalculateFilterImpedance(FilterType filterType, double resistance, double inductance, double capacitance, double frequency)
        {
            double omega = 2 * Math.PI * frequency;
            Complex impedance;

            switch (filterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                case FilterType.BandPass:
                case FilterType.BandStop:
                    impedance = new Complex(resistance, omega * inductance - 1 / (omega * capacitance));
                    break;
                case FilterType.RC:
                    impedance = new Complex(resistance, -1 / (omega * capacitance));
                    break;
                case FilterType.RL:
                    impedance = new Complex(resistance, omega * inductance);
                    break;
                case FilterType.Pi:
                    impedance = CalculatePiFilterImpedance(resistance, inductance, capacitance, omega);
                    break;
                case FilterType.Trap:
                    impedance = CalculateTrapFilterImpedance(resistance, inductance, capacitance, omega);
                    break;
                case FilterType.Quartz:
                    impedance = CalculateQuartzImpedance(resistance, inductance, capacitance, omega);
                    break;
                default:
                    throw new ArgumentException("Unsupported filter type");
            }

            return impedance;
        }

        private Complex CalculatePiFilterImpedance(double resistance, double inductance, double capacitance, double omega)
        {
            Complex zL = new Complex(0, omega * inductance);
            Complex zC = new Complex(0, -1 / (omega * capacitance));
            return 1 / (1 / zL + 1 / (resistance + zC) + 1 / zC);
        }

        private Complex CalculateTrapFilterImpedance(double resistance, double inductance, double capacitance, double omega)
        {
            Complex zL = new Complex(0, omega * inductance);
            Complex zC = new Complex(0, -1 / (omega * capacitance));
            return zL + 1 / (1 / resistance + 1 / zC);
        }

        private Complex CalculateQuartzImpedance(double resistance, double inductance, double capacitance, double omega)
        {
            Complex zL = new Complex(0, omega * inductance);
            Complex zC = new Complex(0, -1 / (omega * capacitance));
            return resistance + zL + zC;
        }

        public FilterResults CalculateFilterResults(FilterInputValues inputValues)
        {
            InputValidator.ValidateInputValues(inputValues);

            var results = new FilterResults
            {
                FilterType = inputValues.FilterType,
                CutoffFrequency = CalculateFilterCutoffFrequency(inputValues.FilterType, inputValues.Resistance, inputValues.Inductance, inputValues.Capacitance),
                QualityFactor = CalculateQualityFactor(inputValues),
                Bandwidth = CalculateBandwidth(CalculateFilterCutoffFrequency(inputValues.FilterType, inputValues.Resistance, inputValues.Inductance, inputValues.Capacitance), CalculateQualityFactor(inputValues)),
                Impedance = CalculateFilterImpedance(inputValues.FilterType, inputValues.Resistance, inputValues.Inductance, inputValues.Capacitance, inputValues.Frequency).Magnitude,
                PhaseShift = CalculateFilterPhaseResponse(inputValues.FilterType, inputValues.Frequency, CalculateFilterCutoffFrequency(inputValues.FilterType, inputValues.Resistance, inputValues.Inductance, inputValues.Capacitance), CalculateBandwidth(CalculateFilterCutoffFrequency(inputValues.FilterType, inputValues.Resistance, inputValues.Inductance, inputValues.Capacitance), CalculateQualityFactor(inputValues))),
                GroupDelay = CalculateGroupDelay(inputValues.FilterType, inputValues.Frequency, CalculateFilterCutoffFrequency(inputValues.FilterType, inputValues.Resistance, inputValues.Inductance, inputValues.Capacitance), CalculateBandwidth(CalculateFilterCutoffFrequency(inputValues.FilterType, inputValues.Resistance, inputValues.Inductance, inputValues.Capacitance), CalculateQualityFactor(inputValues))),
                Attenuation = CalculateFilterAttenuation(inputValues.FilterType, inputValues.Frequency, CalculateFilterCutoffFrequency(inputValues.FilterType, inputValues.Resistance, inputValues.Inductance, inputValues.Capacitance), CalculateBandwidth(CalculateFilterCutoffFrequency(inputValues.FilterType, inputValues.Resistance, inputValues.Inductance, inputValues.Capacitance), CalculateQualityFactor(inputValues))),
                FilterOrder = CalculateFilterOrder(inputValues.FilterType, inputValues.PassbandRipple, inputValues.StopbandAttenuation, inputValues.PassbandFrequency, inputValues.StopbandFrequency)
            };

            return results;
        }


        public double RecalculateFrequency(FilterInputValues inputValues)
        {
            switch (inputValues.FilterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                case FilterType.BandPass:
                case FilterType.BandStop:
                    return 1 / (2 * Math.PI * Math.Sqrt(inputValues.Inductance * inputValues.Capacitance));
                case FilterType.RC:
                    return 1 / (2 * Math.PI * inputValues.Resistance * inputValues.Capacitance);
                case FilterType.RL:
                    return inputValues.Resistance / (2 * Math.PI * inputValues.Inductance);
                // ... (добавьте другие случаи по необходимости)
                default:
                    throw new ArgumentException("Unsupported filter type for frequency recalculation");
            }
        }

        private double CalculateFilterCutoffFrequency(FilterInputValues inputValues)
        {
            switch (inputValues.FilterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                case FilterType.BandPass:
                case FilterType.BandStop:
                case FilterType.Quartz:
                    return 1 / (2 * Math.PI * Math.Sqrt(inputValues.Inductance * inputValues.Capacitance));
                case FilterType.RC:
                    return 1 / (2 * Math.PI * inputValues.Resistance * inputValues.Capacitance);
                case FilterType.RL:
                    return inputValues.Resistance / (2 * Math.PI * inputValues.Inductance);
                case FilterType.Pi:
                    return 1 / (Math.PI * Math.Sqrt(inputValues.Inductance * inputValues.Capacitance));
                case FilterType.Trap:
                    return 1 / (2 * Math.PI * Math.Sqrt(inputValues.Inductance * inputValues.Capacitance));
                default:
                    throw new ArgumentException("Unsupported filter type");
            }
        }


        public double CalculateBandwidth(double cutoffFrequency, double qualityFactor)
        {
            return cutoffFrequency / qualityFactor;
        }

        public int CalculateFilterOrder(FilterType filterType, double passbandRipple, double stopbandAttenuation, double passbandFrequency, double stopbandFrequency)
        {
            double normalizedTransition = stopbandFrequency / passbandFrequency;

            switch (filterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                    return CalculateLowHighPassOrder(passbandRipple, stopbandAttenuation, normalizedTransition);
                case FilterType.BandPass:
                case FilterType.BandStop:
                    return CalculateBandPassStopOrder(passbandRipple, stopbandAttenuation, normalizedTransition);
                default:
                    throw new ArgumentException("Unsupported filter type for order calculation");
            }
        }

        private int CalculateBandPassStopOrder(double passbandRipple, double stopbandAttenuation, double normalizedTransition)
        {
            // For band-pass and band-stop filters, the order is typically doubled
            return 2 * CalculateLowHighPassOrder(passbandRipple, stopbandAttenuation, normalizedTransition);
        }

        private int CalculateLowHighPassOrder(double passbandRipple, double stopbandAttenuation, double normalizedTransition)
        {
            double a1 = Math.Pow(10, stopbandAttenuation / 10) - 1;
            double a2 = Math.Pow(10, passbandRipple / 10) - 1;

            return (int)Math.Ceiling(Math.Log10(Math.Sqrt(a1 / a2)) / (2 * Math.Log10(normalizedTransition)));
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
                case FilterType.BandStop:
                    return CalculateBandResponse(filterType, normalizedFrequency, cutoffFrequency, bandwidth);
                case FilterType.RC:
                case FilterType.RL:
                    return 1 / Math.Sqrt(1 + Math.Pow(normalizedFrequency, 2));
                case FilterType.Pi:
                case FilterType.Trap:
                    return CalculatePiTrapResponse(filterType, normalizedFrequency);
                case FilterType.Quartz:
                    return CalculateQuartzResponse(normalizedFrequency, cutoffFrequency, bandwidth);
                default:
                    throw new ArgumentException("Unsupported filter type for magnitude response calculation");
            }
        }

        private double CalculateBandResponse(FilterType filterType, double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            double response = 1 / Math.Sqrt(1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2));
            return filterType == FilterType.BandStop ? 1 - response : response;
        }

        private double CalculatePiTrapResponse(FilterType filterType, double normalizedFrequency)
        {
            return filterType == FilterType.Pi
                ? 1 / (1 + Math.Pow(normalizedFrequency - 1 / normalizedFrequency, 2))
                : Math.Pow(normalizedFrequency - 1 / normalizedFrequency, 2) / (1 + Math.Pow(normalizedFrequency - 1 / normalizedFrequency, 2));
        }

        private double CalculateQuartzResponse(double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            return Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2) / (1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2));
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

        #region  Methods

        public double CalculateLowPassMagnitudeResponse(double frequency, double cutoffFrequency)
        {
            return 1 / Math.Sqrt(1 + Math.Pow(frequency / cutoffFrequency, 2));
        }

        public double CalculateHighPassMagnitudeResponse(double frequency, double cutoffFrequency)
        {
            return 1 / Math.Sqrt(1 + Math.Pow(cutoffFrequency / frequency, 2));
        }

        public double CalculateBandPassMagnitudeResponse(double frequency, double centerFrequency, double bandwidth)
        {
            double normalizedFreq = frequency / centerFrequency;
            double normalizedBandwidth = bandwidth / centerFrequency;
            return 1 / Math.Sqrt(1 + Math.Pow((normalizedFreq - 1 / normalizedFreq) / normalizedBandwidth, 2));
        }

        public double CalculateBandStopMagnitudeResponse(double frequency, double centerFrequency, double bandwidth)
        {
            double normalizedFreq = frequency / centerFrequency;
            double normalizedBandwidth = bandwidth / centerFrequency;
            return Math.Sqrt(1 - 1 / (1 + Math.Pow((normalizedFreq - 1 / normalizedFreq) / normalizedBandwidth, 2)));
        }

        public double CalculateLowPassPhaseResponse(double frequency, double cutoffFrequency)
        {
            return -Math.Atan(frequency / cutoffFrequency);
        }

        public double CalculateHighPassPhaseResponse(double frequency, double cutoffFrequency)
        {
            return Math.PI / 2 - Math.Atan(frequency / cutoffFrequency);
        }

        public double CalculateBandPassPhaseResponse(double frequency, double centerFrequency, double bandwidth)
        {
            double normalizedFreq = frequency / centerFrequency;
            double normalizedBandwidth = bandwidth / centerFrequency;
            return Math.Atan((normalizedFreq - 1 / normalizedFreq) / normalizedBandwidth);
        }

        public double CalculateBandStopPhaseResponse(double frequency, double centerFrequency, double bandwidth)
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
        public double PassbandRipple { get; set; }
        public double StopbandAttenuation { get; set; }
        public double StopbandFrequency { get; set; }
        public string StopbandFrequencyUnit { get; set; }
        public double QualityFactor { get; set; }
        public double PassbandFrequency { get; set; }

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

    #endregion
}
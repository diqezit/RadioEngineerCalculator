using System;

namespace RadioEngineerCalculator.Services
{
    public class CalculationService
    {
        // Константы
        private const double SpeedOfLight = 299_792_458; // м/с
        private const double Mu0 = 4 * Math.PI * 1e-7; // магнитная постоянная

        #region Основные электрические расчеты

        public double CalculateVoltage(double resistance, double current) => resistance * current;
        public double CalculateCurrent(double voltage, double resistance) => voltage / resistance;
        public double CalculateResistance(double voltage, double current) => voltage / current;
        public double CalculateParallelResistance(double r1, double r2) => 1 / (1 / r1 + 1 / r2);
        public double CalculateSeriesResistance(double r1, double r2) => r1 + r2;

        public double CalculatePower(double current, double resistance)
        {
            if (current <= 0 || resistance <= 0)
                throw new ArgumentException("Current and resistance must be greater than zero.");
            return Math.Pow(current, 2) * resistance;
        }

        public double CalculatePowerVI(double voltage, double current)
        {
            if (voltage <= 0 || current <= 0)
                throw new ArgumentException("Voltage and current must be greater than zero.");
            return voltage * current;
        }

        #endregion

        #region Расчеты переменного тока и мощности

        public double CalculateCapacitiveReactance(double capacitance, double frequency)
            => 1 / (2 * Math.PI * frequency * capacitance);

        public double CalculateInductiveReactance(double inductance, double frequency)
            => 2 * Math.PI * frequency * inductance;

        public double CalculateReactance(double value, double frequency, bool isCapacitive)
        {
            if (isCapacitive)
            {
                return CalculateCapacitiveReactance(value, frequency);
            }
            else
            {
                return CalculateInductiveReactance(value, frequency);
            }
        }

        public double CalculatePowerFactor(double realPower, double apparentPower)
        {
            if (realPower <= 0 || apparentPower <= 0)
                throw new ArgumentException("Real power and apparent power must be greater than zero.");
            if (realPower > apparentPower)
                throw new ArgumentException("Real power cannot be greater than apparent power.");
            return realPower / apparentPower;
        }

        public double CalculateReactivePower(double apparentPower, double realPower)
        {
            if (apparentPower <= 0 || realPower <= 0)
                throw new ArgumentException("Apparent power and real power must be greater than zero.");
            if (realPower > apparentPower)
                throw new ArgumentException("Real power cannot be greater than apparent power.");
            return Math.Sqrt(Math.Pow(apparentPower, 2) - Math.Pow(realPower, 2));
        }

        #endregion

        #region Расчеты для радиочастотных цепей

        public double CalculateGain(double powerIn, double powerOut)
            => 10 * Math.Log10(powerOut / powerIn);

        public double CalculateResonanceFrequency(double inductance, double capacitance)
            => 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));

        public double CalculateWavelength(double frequency)
            => SpeedOfLight / frequency;

        public double CalculateVSWR(double forwardPower, double reflectedPower)
        {
            var sqrtRatio = Math.Sqrt(reflectedPower / forwardPower);
            return (1 + sqrtRatio) / (1 - sqrtRatio);
        }

        public double CalculateQFactor(double resonantFrequency, double bandwidth)
            => resonantFrequency / bandwidth;

        public double CalculateNoiseFigure(double noiseFactor)
            => 10 * Math.Log10(noiseFactor);

        public double CalculateSkinDepth(double frequency, double resistivity, double relativePermeability)
            => Math.Sqrt(resistivity / (Math.PI * frequency * Mu0 * relativePermeability));

        public double CalculateImpedanceMatching(double sourceImpedance, double loadImpedance)
            => Math.Sqrt(sourceImpedance * loadImpedance);

        public double CalculateAttenuator(double inputVoltage, double outputVoltage)
            => 20 * Math.Log10(inputVoltage / outputVoltage);

        public double CalculateCoaxialCable(double innerDiameter, double outerDiameter)
            => 138 * Math.Log10(outerDiameter / innerDiameter);

        #endregion

        #region Расчеты для усилителей

        public double CalculateAmplifierEfficiency(double outputPower, double inputDCPower)
        {
            if (outputPower < 0 || inputDCPower <= 0)
                throw new ArgumentException("Output power must be non-negative and input DC power must be positive.");
            return (outputPower / inputDCPower) * 100;
        }

        public double Calculate1dBCompressionPoint(double inputPower, double outputPower, double smallSignalGain)
        {
            if (outputPower <= inputPower + smallSignalGain - 1)
                throw new ArgumentException("The given output power does not represent a 1dB compression point.");
            return inputPower;
        }

        public double CalculateIP3(double fundamentalPower, double thirdOrderPower)
        {
            if (fundamentalPower <= thirdOrderPower)
                throw new ArgumentException("Fundamental power must be greater than third-order product power.");
            return fundamentalPower + (fundamentalPower - thirdOrderPower) / 2;
        }

        public double CalculateTransconductance(double drainCurrent, double gateVoltage)
            => drainCurrent / gateVoltage;

        public double CalculateVoltageGain(double transconductance, double loadResistance)
            => transconductance * loadResistance;

        #endregion

        #region Расчеты для модуляции

        public double CalculateAMIndex(double carrierAmplitude, string carrierAmplitudeUnit, double modulatingAmplitude, string modulatingAmplitudeUnit)
        {
            var carrierAmplitudeV = UnitC.Conv.Voltage(carrierAmplitude, carrierAmplitudeUnit, "V");
            var modulatingAmplitudeV = UnitC.Conv.Voltage(modulatingAmplitude, modulatingAmplitudeUnit, "V");

            if (carrierAmplitudeV <= 0 || modulatingAmplitudeV <= 0)
                throw new ArgumentException("Амплитуды должны быть больше нуля.");

            return modulatingAmplitudeV / carrierAmplitudeV;
        }

        public double CalculateFMIndex(double carrierFrequency, string carrierFrequencyUnit, double frequencyDeviation, string frequencyDeviationUnit)
        {
            var carrierFrequencyHz = UnitC.Conv.Frequency(carrierFrequency, carrierFrequencyUnit, "Hz");
            var frequencyDeviationHz = UnitC.Conv.Frequency(frequencyDeviation, frequencyDeviationUnit, "Hz");

            if (carrierFrequencyHz <= 0 || frequencyDeviationHz <= 0)
                throw new ArgumentException("Частоты должны быть больше нуля.");

            return frequencyDeviationHz / carrierFrequencyHz;
        }

        public double CalculatePMIndex(double carrierPhase, string carrierPhaseUnit, double phaseDeviation, string phaseDeviationUnit)
        {
            var carrierPhaseRad = UnitC.Conv.Angle(carrierPhase, carrierPhaseUnit, "rad");
            var phaseDeviationRad = UnitC.Conv.Angle(phaseDeviation, phaseDeviationUnit, "rad");

            if (carrierPhaseRad <= 0 || phaseDeviationRad <= 0)
                throw new ArgumentException("Фазы должны быть больше нуля.");

            return phaseDeviationRad / carrierPhaseRad;
        }


        public double CalculateAMBandwidth(double modulationFrequency)
            => 2 * modulationFrequency;

        public double CalculateFMBandwidth(double modulationIndex, double modulationFrequency)
            => 2 * (modulationIndex + 1) * modulationFrequency;

        #endregion

        #region Расчеты для длинных линий

        public double CalculateCharacteristicImpedance(double inductancePerUnit, double capacitancePerUnit)
            => Math.Sqrt(inductancePerUnit / capacitancePerUnit);

        // Метод CalculatePropagationConstant закомментирован из-за использования комплексных чисел
        // public double CalculatePropagationConstant(double resistance, double inductance, double conductance, double capacitance, double frequency)
        // {
        //     var omega = 2 * Math.PI * frequency;
        //     return Math.Sqrt((resistance + 1i * omega * inductance) * (conductance + 1i * omega * capacitance));
        // }

        #endregion

        #region Расчеты для микрополосковых линий

        public double CalculateMicrostripImpedance(double width, double height, double dielectricConstant)
        {
            var effectiveWidth = width + (1.25 * height / Math.PI) * (1 + Math.Log(4 * Math.PI * width / height));
            var effectiveDielectricConstant = (dielectricConstant + 1) / 2 + (dielectricConstant - 1) / (2 * Math.Sqrt(1 + 12 * height / width));
            return (60 / Math.Sqrt(effectiveDielectricConstant)) * Math.Log((8 * height / effectiveWidth) + (effectiveWidth / (4 * height)));
        }

        #endregion

        #region Расчеты для антенн

        public double CalculateAntennaGain(double efficiency, double directivity)
            => efficiency * directivity;

        public double CalculateAntennaEffectiveArea(double gain, double wavelength)
            => (gain * Math.Pow(wavelength, 2)) / (4 * Math.PI);

        #endregion

        #region Расчеты для цифровой обработки сигналов

        public double CalculateNyquistRate(double maxFrequency)
            => 2 * maxFrequency;

        public double CalculateQuantizationNoise(int bitsPerSample)
            => Math.Pow(2, -bitsPerSample) / Math.Sqrt(12);

        #endregion

        #region Расчеты для смесителей (Mixers)

        public double CalculateConversionGain(double outputPower, double inputPower)
            => 10 * Math.Log10(outputPower / inputPower);

        public double CalculateImageRejectionRatio(double desiredSignalPower, double imageSignalPower)
            => 10 * Math.Log10(desiredSignalPower / imageSignalPower);

        #endregion

        #region Расчеты для систем связи (Communication Systems)

        public double CalculateBitErrorRate(double energyPerBit, double noisePowerDensity)
        {
            var snr = energyPerBit / noisePowerDensity;
            return 0.5 * Erfc(Math.Sqrt(snr / 2));
        }

        private double Erfc(double x)
        {
            return 1 - Erf(x);
        }

        private double Erf(double x)
        {
            // Аппроксимация функции ошибок
            const double a1 = 0.254829592;
            const double a2 = -0.284496736;
            const double a3 = 1.421413741;
            const double a4 = -1.453152027;
            const double a5 = 1.061405429;
            const double p = 0.3275911;

            int sign = (x < 0) ? -1 : 1;
            x = Math.Abs(x);

            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        }

        #endregion
    }
}
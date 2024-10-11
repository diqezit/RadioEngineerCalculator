using System;
using System.Numerics;

namespace RadioEngineerCalculator.Services
{
    public class CalculationService
    {
        // Константы
        private const double SpeedOfLight = 299_792_458; // м/с
        private const double Mu0 = 4 * Math.PI * 1e-7; // магнитная постоянная

        #region Основные электрические расчеты

        public double CalculateVoltage(double resistance, double current)
        {
            Validate.EnsurePositive(resistance, nameof(resistance));
            Validate.EnsurePositive(current, nameof(current));
            return resistance * current;
        }

        public double CalculateCurrent(double voltage, double resistance)
        {
            Validate.EnsurePositive(resistance, nameof(resistance));
            return voltage / resistance;
        }

        public double CalculateResistance(double voltage, double current)
        {
            Validate.EnsurePositive(current, nameof(current));
            return voltage / current;
        }

        public double CalculateParallelResistance(double r1, double r2)
        {
            Validate.EnsurePositive(r1, nameof(r1));
            Validate.EnsurePositive(r2, nameof(r2));
            return 1 / (1 / r1 + 1 / r2);
        }

        public double CalculateSeriesResistance(double r1, double r2)
        {
            Validate.EnsurePositive(r1, nameof(r1));
            Validate.EnsurePositive(r2, nameof(r2));
            return r1 + r2;
        }

        public double CalculatePower(double current, double resistance)
        {
            Validate.EnsurePositive(current, nameof(current));
            Validate.EnsurePositive(resistance, nameof(resistance));
            return Math.Pow(current, 2) * resistance;
        }

        public double CalculatePowerVI(double voltage, double current)
        {
            Validate.EnsurePositive(voltage, nameof(voltage));
            Validate.EnsurePositive(current, nameof(current));
            return voltage * current;
        }

        #endregion

        #region Расчеты переменного тока и мощности

        public double CalculateCapacitiveReactance(double capacitance, double frequency)
        {
            Validate.EnsurePositive(capacitance, nameof(capacitance));
            Validate.EnsurePositive(frequency, nameof(frequency));
            return 1 / (2 * Math.PI * frequency * capacitance);
        }

        public double CalculateInductiveReactance(double inductance, double frequency)
        {
            Validate.EnsurePositive(inductance, nameof(inductance));
            Validate.EnsurePositive(frequency, nameof(frequency));
            return 2 * Math.PI * frequency * inductance;
        }

        public double CalculateReactance(double value, double frequency, bool isCapacitive)
        {
            Validate.EnsurePositive(value, nameof(value));
            Validate.EnsurePositive(frequency, nameof(frequency));
            return isCapacitive ? CalculateCapacitiveReactance(value, frequency) : CalculateInductiveReactance(value, frequency);
        }

        public double CalculatePowerFactor(double realPower, double apparentPower)
        {
            Validate.EnsurePositive(realPower, nameof(realPower));
            Validate.EnsurePositive(apparentPower, nameof(apparentPower));
            if (realPower > apparentPower)
                throw new ArgumentException("Real power cannot be greater than apparent power.");
            return realPower / apparentPower;
        }

        public double CalculateReactivePower(double apparentPower, double realPower)
        {
            Validate.EnsurePositive(apparentPower, nameof(apparentPower));
            Validate.EnsurePositive(realPower, nameof(realPower));
            if (realPower > apparentPower)
                throw new ArgumentException("Real power cannot be greater than apparent power.");
            return Math.Sqrt(Math.Pow(apparentPower, 2) - Math.Pow(realPower, 2));
        }

        #endregion

        #region Расчеты для радиочастотных цепей

        public double CalculateGain(double powerIn, double powerOut)
        {
            Validate.EnsurePositive(powerIn, nameof(powerIn));
            Validate.EnsurePositive(powerOut, nameof(powerOut));
            return 10 * Math.Log10(powerOut / powerIn);
        }

        public double CalculateWavelength(double frequency)
        {
            Validate.EnsurePositive(frequency, nameof(frequency));
            return SpeedOfLight / frequency;
        }

        public double CalculateVSWR(double forwardPower, double reflectedPower)
        {
            Validate.EnsurePositive(forwardPower, nameof(forwardPower));
            Validate.EnsurePositive(reflectedPower, nameof(reflectedPower));
            var sqrtRatio = Math.Sqrt(reflectedPower / forwardPower);
            return (1 + sqrtRatio) / (1 - sqrtRatio);
        }

        public double CalculateQFactor(double resonantFrequency, double bandwidth)
        {
            Validate.EnsurePositive(resonantFrequency, nameof(resonantFrequency));
            Validate.EnsurePositive(bandwidth, nameof(bandwidth));
            return resonantFrequency / bandwidth;
        }

        public double CalculateNoiseFigure(double noiseFactor)
        {
            Validate.EnsurePositive(noiseFactor, nameof(noiseFactor));
            return 10 * Math.Log10(noiseFactor);
        }

        public double CalculateSkinDepth(double frequency, double resistivity, double relativePermeability)
        {
            Validate.EnsurePositive(frequency, nameof(frequency));
            Validate.EnsurePositive(resistivity, nameof(resistivity));
            Validate.EnsurePositive(relativePermeability, nameof(relativePermeability));
            return Math.Sqrt(resistivity / (Math.PI * frequency * Mu0 * relativePermeability));
        }

        public double CalculateImpedanceMatching(double sourceImpedance, double loadImpedance)
        {
            Validate.EnsurePositive(sourceImpedance, nameof(sourceImpedance));
            Validate.EnsurePositive(loadImpedance, nameof(loadImpedance));
            return Math.Sqrt(sourceImpedance * loadImpedance);
        }

        public double CalculateAttenuator(double inputVoltage, double outputVoltage)
        {
            Validate.EnsurePositive(inputVoltage, nameof(inputVoltage));
            Validate.EnsurePositive(outputVoltage, nameof(outputVoltage));
            return 20 * Math.Log10(inputVoltage / outputVoltage);
        }

        public double CalculateCoaxialCable(double innerDiameter, double outerDiameter)
        {
            Validate.EnsurePositive(innerDiameter, nameof(innerDiameter));
            Validate.EnsurePositive(outerDiameter, nameof(outerDiameter));
            return 138 * Math.Log10(outerDiameter / innerDiameter);
        }

        #endregion

        #region Расчеты для усилителей

        public double CalculateAmplifierEfficiency(double outputPower, double inputDCPower)
        {
            Validate.EnsurePositive(outputPower, nameof(outputPower));
            Validate.EnsurePositive(inputDCPower, nameof(inputDCPower));
            return (outputPower / inputDCPower) * 100;
        }

        public double Calculate1dBCompressionPoint(double inputPower, double outputPower, double smallSignalGain)
        {
            Validate.EnsurePositive(inputPower, nameof(inputPower));
            Validate.EnsurePositive(outputPower, nameof(outputPower));
            Validate.EnsurePositive(smallSignalGain, nameof(smallSignalGain));
            if (outputPower <= inputPower + smallSignalGain - 1)
                throw new ArgumentException("The given output power does not represent a 1dB compression point.");
            return inputPower;
        }

        public double CalculateIP3(double fundamentalPower, double thirdOrderPower)
        {
            Validate.EnsurePositive(fundamentalPower, nameof(fundamentalPower));
            Validate.EnsurePositive(thirdOrderPower, nameof(thirdOrderPower));
            if (fundamentalPower <= thirdOrderPower)
                throw new ArgumentException("Fundamental power must be greater than third-order product power.");
            return fundamentalPower + (fundamentalPower - thirdOrderPower) / 2;
        }

        public double CalculateTransconductance(double drainCurrent, double gateVoltage)
        {
            Validate.EnsurePositive(drainCurrent, nameof(drainCurrent));
            Validate.EnsurePositive(gateVoltage, nameof(gateVoltage));
            return drainCurrent / gateVoltage;
        }

        public double CalculateVoltageGain(double transconductance, double loadResistance)
        {
            Validate.EnsurePositive(transconductance, nameof(transconductance));
            Validate.EnsurePositive(loadResistance, nameof(loadResistance));
            return transconductance * loadResistance;
        }

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

            // Упрощенное для чтения выражение
            double y = 1.0 - (
                (
                    (
                        (
                            (a5 * t + a4) * t + a3
                        ) * t + a2
                    ) * t + a1
                ) * t * Math.Exp(-x * x)
            );

            return sign * y;
        }


        #endregion

        #region Расчеты для колебательных контуров (Oscillators)

        // Расчет резонансной частоты с улучшенной валидацией
        public double CalculateResonanceFrequency(double inductance, double capacitance)
        {
            if (inductance <= 0 || capacitance <= 0)
                throw new ArgumentException("Индуктивность и ёмкость должны быть положительными.");
            return 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));
        }

        // Добавление более явной обработки ошибок для импеданса
        public Complex CalculateSeriesImpedance(double resistance, double inductance, double capacitance, double frequency)
        {
            if (resistance < 0 || inductance <= 0 || capacitance <= 0 || frequency <= 0)
                throw new ArgumentException("Все параметры должны быть положительными, а индуктивность, ёмкость и частота должны быть строго больше нуля.");

            var inductiveReactance = 2 * Math.PI * frequency * inductance;
            var capacitiveReactance = 1 / (2 * Math.PI * frequency * capacitance);

            return new Complex(resistance, inductiveReactance - capacitiveReactance);
        }

        // Расчет индекса паралельного контура
        public Complex CalculateParallelImpedance(double resistance, double inductance, double capacitance, double frequency)
        {
            if (resistance <= 0 || inductance <= 0 || capacitance <= 0 || frequency <= 0)
                throw new ArgumentException("Все параметры должны быть строго больше нуля.");

            // Индуктивное и емкостное сопротивление
            var inductiveReactance = 2 * Math.PI * frequency * inductance;  // X_L = ωL
            var capacitiveReactance = 1 / (2 * Math.PI * frequency * capacitance);  // X_C = 1 / (ωC)

            // Преобразуем в комплексные числа для импеданса
            Complex Z_R = new Complex(resistance, 0);  // Сопротивление
            Complex Z_L = new Complex(0, inductiveReactance);  // Индуктивное сопротивление (чисто мнимая часть)
            Complex Z_C = new Complex(0, -capacitiveReactance);  // Емкостное сопротивление (чисто мнимая часть)

            // Полный импеданс для параллельного контура: Z_parallel = 1 / (1/Z_R + 1/Z_L + 1/Z_C)
            return 1 / (1 / Z_R + 1 / Z_L + 1 / Z_C);
        }

        // Расчет добротности для последовательного контура
        public double CalculateSeriesQFactor(double inductance, double resistance, double frequency)
        {
            if (inductance <= 0 || resistance <= 0 || frequency <= 0)
                throw new ArgumentException("Parameters must be greater than zero.");

            // Добротность Q = ωL / R
            var inductiveReactance = 2 * Math.PI * frequency * inductance;  // X_L = ωL
            return inductiveReactance / resistance;
        }

        // Расчет добротности для параллельного контура
        public double CalculateParallelQFactor(double inductance, double resistance, double frequency)
        {
            if (inductance <= 0 || resistance <= 0 || frequency <= 0)
                throw new ArgumentException("Parameters must be greater than zero.");

            // Добротность Q = R / ωL
            var inductiveReactance = 2 * Math.PI * frequency * inductance;  // X_L = ωL
            return resistance / inductiveReactance;
        }

        #endregion
    }
}
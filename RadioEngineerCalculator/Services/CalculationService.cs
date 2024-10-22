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
            ValidatePositive(resistance, nameof(resistance));
            ValidatePositive(current, nameof(current));
            return resistance * current;
        }

        public double CalculateCurrent(double voltage, double resistance)
        {
            ValidatePositive(resistance, nameof(resistance));
            return voltage / resistance;
        }

        public double CalculateResistance(double voltage, double current)
        {
            ValidatePositive(current, nameof(current));
            return voltage / current;
        }

        public double CalculateParallelResistance(double r1, double r2)
        {
            ValidatePositive(r1, nameof(r1));
            ValidatePositive(r2, nameof(r2));
            return 1 / (1 / r1 + 1 / r2);
        }

        public double CalculateSeriesResistance(double r1, double r2)
        {
            ValidatePositive(r1, nameof(r1));
            ValidatePositive(r2, nameof(r2));
            return r1 + r2;
        }

        public double CalculatePower(double current, double resistance)
        {
            ValidatePositive(current, nameof(current));
            ValidatePositive(resistance, nameof(resistance));
            return Math.Pow(current, 2) * resistance;
        }

        public double CalculatePowerVI(double voltage, double current)
        {
            ValidatePositive(voltage, nameof(voltage));
            ValidatePositive(current, nameof(current));
            return voltage * current;
        }

        #endregion

        #region Расчеты переменного тока и мощности

        /// Рассчитывает ёмкостное реактивное сопротивление.

        public double CalculateCapacitiveReactance(double capacitance, double frequency)
        {
            EnsurePositive(capacitance, nameof(capacitance));
            EnsurePositive(frequency, nameof(frequency));
            return 1 / (2 * Math.PI * frequency * capacitance);
        }

        /// Рассчитывает индуктивное реактивное сопротивление.

        public double CalculateInductiveReactance(double inductance, double frequency)
        {
            EnsurePositive(inductance, nameof(inductance));
            EnsurePositive(frequency, nameof(frequency));
            return 2 * Math.PI * frequency * inductance;
        }

        /// Рассчитывает реактивное сопротивление.

        public double CalculateReactance(double value, double frequency, bool isCapacitive)
        {
            EnsurePositive(value, nameof(value));
            EnsurePositive(frequency, nameof(frequency));
            return isCapacitive
                ? CalculateCapacitiveReactance(value, frequency)
                : CalculateInductiveReactance(value, frequency);
        }

        /// Рассчитывает коэффициент мощности.

        public double CalculatePowerFactor(double realPower, double apparentPower)
        {
            EnsurePositive(realPower, nameof(realPower));
            EnsurePositive(apparentPower, nameof(apparentPower));
            if (realPower > apparentPower)
                throw new ArgumentException("Реальная мощность не может быть больше кажущейся мощности.");
            return realPower / apparentPower;
        }

        /// Рассчитывает реактивную мощность.

        public double CalculateReactivePower(double apparentPower, double realPower)
        {
            EnsurePositive(apparentPower, nameof(apparentPower));
            EnsurePositive(realPower, nameof(realPower));
            if (realPower > apparentPower)
                throw new ArgumentException("Реальная мощность не может быть больше кажущейся мощности.");
            return Math.Sqrt(Math.Pow(apparentPower, 2) - Math.Pow(realPower, 2));
        }

        #endregion

        #region Расчеты для радиочастотных цепей

        public double CalculateGain(double powerIn, double powerOut)
        {
            ValidatePositive(powerIn);
            ValidatePositive(powerOut);
            return 10 * Math.Log10(powerOut / powerIn);
        }

        public double CalculateWavelength(double frequency)
        {
            ValidatePositive(frequency);
            return SpeedOfLight / frequency;
        }

        public double CalculateQFactor(double resonantFrequency, double bandwidth)
        {
            ValidatePositive(resonantFrequency);
            ValidatePositive(bandwidth);
            return resonantFrequency / bandwidth;
        }

        public double CalculateNoiseFigure(double noiseFactor)
        {
            ValidatePositive(noiseFactor);
            return 10 * Math.Log10(noiseFactor);
        }

        public double CalculateSkinDepth(double frequency, double resistivity, double relativePermeability)
        {
            ValidatePositive(frequency);
            ValidatePositive(resistivity);
            ValidatePositive(relativePermeability);
            return Math.Sqrt(resistivity / (Math.PI * frequency * Mu0 * relativePermeability));
        }

        public double CalculateReflectionCoefficient(double sourceImpedance, double loadImpedance)
        {
            ValidatePositive(sourceImpedance);
            ValidatePositive(loadImpedance);
            return Math.Abs((loadImpedance - sourceImpedance) / (loadImpedance + sourceImpedance));
        }

        public double CalculateVSWR(double sourceImpedance, double loadImpedance)
        {
            var reflectionCoefficient = CalculateReflectionCoefficient(sourceImpedance, loadImpedance);
            return (1 + reflectionCoefficient) / (1 - reflectionCoefficient);
        }

        public double CalculateImpedanceMatching(double sourceImpedance, double loadImpedance)
        {
            ValidatePositive(sourceImpedance);
            ValidatePositive(loadImpedance);
            return Math.Sqrt(sourceImpedance * loadImpedance);
        }

        public double CalculateAttenuator(double inputVoltage, double outputVoltage)
        {
            ValidatePositive(inputVoltage);
            ValidatePositive(outputVoltage);
            return 20 * Math.Log10(inputVoltage / outputVoltage);
        }

        public double CalculateCoaxialCable(double innerDiameter, double outerDiameter)
        {
            ValidatePositive(innerDiameter);
            ValidatePositive(outerDiameter);
            return 138 * Math.Log10(outerDiameter / innerDiameter);
        }

        #endregion

        #region Расчеты для усилителей

        // Расчет эффективности усилителя
        public double CalculateAmplifierEfficiency(double outputPower, double inputDCPower)
        {
            EnsurePositive(outputPower, nameof(outputPower));
            EnsurePositive(inputDCPower, nameof(inputDCPower));
            return (outputPower / inputDCPower) * 100;
        }

        // Расчет точки сжатия на 1 дБ
        public double Calculate1dBCompressionPoint(double inputPower, double outputPower, double smallSignalGain)
        {
            EnsurePositive(inputPower, nameof(inputPower));
            EnsurePositive(outputPower, nameof(outputPower));
            EnsurePositive(smallSignalGain, nameof(smallSignalGain));
            if (outputPower <= inputPower + smallSignalGain - 1)
                throw new ArgumentException("The given output power does not represent a 1dB compression point.");
            return inputPower;
        }

        // Расчет третьего интермодуляционного пересечения (IP3)
        public double CalculateIP3(double fundamentalPower, double thirdOrderPower)
        {
            EnsurePositive(fundamentalPower, nameof(fundamentalPower));
            EnsurePositive(thirdOrderPower, nameof(thirdOrderPower));
            if (fundamentalPower <= thirdOrderPower)
                throw new ArgumentException("Fundamental power must be greater than third-order product power.");
            return fundamentalPower + (fundamentalPower - thirdOrderPower) / 2;
        }

        // Расчет трансадмиттанса (transconductance)
        public double CalculateTransconductance(double drainCurrent, double gateVoltage)
        {
            EnsurePositive(drainCurrent, nameof(drainCurrent));
            EnsurePositive(gateVoltage, nameof(gateVoltage));
            return drainCurrent / gateVoltage;
        }

        // Расчет усиления по напряжению
        public double CalculateVoltageGain(double transconductance, double loadResistance)
        {
            EnsurePositive(transconductance, nameof(transconductance));
            EnsurePositive(loadResistance, nameof(loadResistance));
            return transconductance * loadResistance;
        }

        #endregion

        #region Расчеты для модуляции

        // Расчет индекса амплитудной модуляции (AM)
        public double CalculateAMIndex(double carrierAmplitude, double modulatingAmplitude)
        {
            if (carrierAmplitude <= 0 || modulatingAmplitude <= 0)
            {
                throw new ArgumentException("Амплитуды должны быть больше нуля.");
            }
            return modulatingAmplitude / carrierAmplitude;
        }

        // Расчет индекса частотной модуляции (FM)
        public double CalculateFMIndex(double carrierFrequency, double frequencyDeviation)
        {
            if (carrierFrequency <= 0 || frequencyDeviation <= 0)
            {
                throw new ArgumentException("Частоты должны быть больше нуля.");
            }
            return frequencyDeviation / carrierFrequency;
        }

        // Расчет индекса фазовой модуляции (PM)
        public double CalculatePMIndex(double carrierPhase, double phaseDeviation)
        {
            if (carrierPhase <= 0 || phaseDeviation <= 0)
            {
                throw new ArgumentException("Фазы должны быть больше нуля.");
            }
            return phaseDeviation / carrierPhase;
        }

        // Расчет ширины полосы для амплитудной модуляции (AM)
        public double CalculateAMBandwidth(double modulationFrequency)
        {
            if (modulationFrequency <= 0)
            {
                throw new ArgumentException("Частота модуляции должна быть больше нуля.");
            }
            return 2 * modulationFrequency;
        }

        // Расчет ширины полосы для частотной модуляции (FM)
        public double CalculateFMBandwidth(double modulationIndex, double modulationFrequency)
        {
            if (modulationIndex <= 0 || modulationFrequency <= 0)
            {
                throw new ArgumentException("Индекс модуляции и частота модуляции должны быть больше нуля.");
            }
            return 2 * (modulationIndex + 1) * modulationFrequency;
        }

        #endregion

        #region Расчеты для длинных линий

        // Расчет характеристического импеданса длинной линии с проверкой входных параметров
        public double CalculateCharacteristicImpedance(double inductancePerUnit, double capacitancePerUnit)
        {
            if (inductancePerUnit <= 0 || capacitancePerUnit <= 0)
                throw new ArgumentException("Индуктивность и ёмкость на единицу длины должны быть положительными.");

            // Расчет характеристического импеданса
            return Math.Sqrt(inductancePerUnit / capacitancePerUnit);
        }

        #endregion

        #region Расчеты для микрополосковых линий

        // Расчет волнового сопротивления микрополосковой линии с проверкой входных параметров
        public double CalculateMicrostripImpedance(double width, double height, double dielectricConstant)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Ширина и высота должны быть положительными.");
            if (dielectricConstant <= 1)
                throw new ArgumentException("Диэлектрическая проницаемость должна быть больше 1.");

            // Эффективная ширина полоски
            var effectiveWidth = width + (1.25 * height / Math.PI) * (1 + Math.Log(4 * Math.PI * width / height));

            // Эффективная диэлектрическая проницаемость
            var effectiveDielectricConstant = (dielectricConstant + 1) / 2 + (dielectricConstant - 1) / (2 * Math.Sqrt(1 + 12 * height / width));

            // Расчет импеданса микрополосковой линии
            return (60 / Math.Sqrt(effectiveDielectricConstant)) * Math.Log((8 * height / effectiveWidth) + (effectiveWidth / (4 * height)));
        }

        #endregion

        #region Расчеты для антенн

        // Расчет коэффициента усиления антенны с проверкой эффективности и направленности
        public double CalculateAntennaGain(double efficiency, double directivity)
        {
            if (efficiency <= 0 || efficiency > 1)
                throw new ArgumentException("Эффективность должна быть в пределах от 0 до 1.");
            if (directivity <= 0)
                throw new ArgumentException("Направленность должна быть положительной.");

            return efficiency * directivity;
        }

        // Расчет эффективной площади антенны с проверкой коэффициента усиления и длины волны
        public double CalculateAntennaEffectiveArea(double gain, double wavelength)
        {
            if (gain <= 0)
                throw new ArgumentException("Коэффициент усиления должен быть положительным.");
            if (wavelength <= 0)
                throw new ArgumentException("Длина волны должна быть положительной.");

            return (gain * Math.Pow(wavelength, 2)) / (4 * Math.PI);
        }

        #endregion

        #region Расчеты для цифровой обработки сигналов

        // Расчет частоты Найквиста с валидацией максимальной частоты
        public double CalculateNyquistRate(double maxFrequency)
        {
            if (maxFrequency <= 0)
                throw new ArgumentException("Максимальная частота должна быть положительной.");

            return 2 * maxFrequency;
        }

        // Расчет шума квантования с проверкой количества бит на отсчет
        public double CalculateQuantizationNoise(int bitsPerSample)
        {
            if (bitsPerSample <= 0)
                throw new ArgumentException("Количество бит на отсчет должно быть положительным.");

            return Math.Pow(2, -bitsPerSample) / Math.Sqrt(12);
        }

        #endregion

        #region Расчеты для смесителей (Mixers)

        // Расчет коэффициента преобразования с валидацией входных данных
        public double CalculateConversionGain(double outputPower, double inputPower)
        {
            if (outputPower <= 0 || inputPower <= 0)
                throw new ArgumentException("Выходная и входная мощность должны быть положительными.");

            return 10 * Math.Log10(outputPower / inputPower);
        }

        // Расчет коэффициента подавления зеркальной частоты с валидацией входных данных
        public double CalculateImageRejectionRatio(double desiredSignalPower, double imageSignalPower)
        {
            if (desiredSignalPower <= 0 || imageSignalPower <= 0)
                throw new ArgumentException("Мощности желаемого и зеркального сигналов должны быть положительными.");

            return 10 * Math.Log10(desiredSignalPower / imageSignalPower);
        }

        #endregion

        #region Расчеты для систем связи (Communication Systems)

        // Расчет битовой ошибки с валидацией
        public double CalculateBitErrorRate(double energyPerBit, double noisePowerDensity)
        {
            if (energyPerBit <= 0 || noisePowerDensity <= 0)
                throw new ArgumentException("Энергия на бит и мощность шума должны быть положительными.");

            var snr = energyPerBit / noisePowerDensity;
            return 0.5 * Erfc(Math.Sqrt(snr / 2));
        }

        // Вычисление комплементарной функции ошибок с использованием expression-bodied метода
        private double Erfc(double x) => 1 - Erf(x);

        // Аппроксимация функции ошибок с валидацией параметров
        private double Erf(double x)
        {
            const double a1 = 0.254829592;
            const double a2 = -0.284496736;
            const double a3 = 1.421413741;
            const double a4 = -1.453152027;
            const double a5 = 1.061405429;
            const double p = 0.3275911;

            if (double.IsNaN(x) || double.IsInfinity(x))
                throw new ArgumentException("Недопустимое значение для функции ошибок.");

            int sign = (x < 0) ? -1 : 1;
            x = Math.Abs(x);

            double t = 1.0 / (1.0 + p * x);

            // Более читаемая запись аппроксимации
            double y = 1.0 - (((((a5 * t + a4) * t + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x));

            return sign * y;
        }

        #endregion

        #region Расчеты для колебательных контуров (Oscillators)

        // Расчет резонансной частоты с улучшенной валидацией и expression-bodied методом
        public double CalculateResonanceFrequency(double inductance, double capacitance) =>
            (inductance, capacitance) switch
            {
                ( > 0, > 0) => 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance)),
                _ => throw new ArgumentException("Индуктивность и ёмкость должны быть положительными.")
            };

        // Добавление более явной обработки ошибок с использованием switch expression
        public Complex CalculateSeriesImpedance(double resistance, double inductance, double capacitance, double frequency)
        {
            EnsurePositiveValues(new (double, string)[]
            {
        (resistance, nameof(resistance)),
        (inductance, nameof(inductance)),
        (capacitance, nameof(capacitance)),
        (frequency, nameof(frequency))
            });

            var inductiveReactance = 2 * Math.PI * frequency * inductance;
            var capacitiveReactance = 1 / (2 * Math.PI * frequency * capacitance);

            return new Complex(resistance, inductiveReactance - capacitiveReactance);
        }

        // Расчет импеданса для параллельного контура
        public Complex CalculateParallelImpedance(double resistance, double inductance, double capacitance, double frequency)
        {
            EnsurePositiveValues(new (double, string)[]
            {
        (resistance, nameof(resistance)),
        (inductance, nameof(inductance)),
        (capacitance, nameof(capacitance)),
        (frequency, nameof(frequency))
            });

            var inductiveReactance = 2 * Math.PI * frequency * inductance;
            var capacitiveReactance = 1 / (2 * Math.PI * frequency * capacitance);

            Complex Z_R = new Complex(resistance, 0);
            Complex Z_L = new Complex(0, inductiveReactance);
            Complex Z_C = new Complex(0, -capacitiveReactance);

            return 1 / (1 / Z_R + 1 / Z_L + 1 / Z_C);
        }

        // Расчет добротности для последовательного контура
        public double CalculateSeriesQFactor(double inductance, double resistance, double frequency) =>
            (inductance, resistance, frequency) switch
            {
                ( > 0, > 0, > 0) => (2 * Math.PI * frequency * inductance) / resistance,
                _ => throw new ArgumentException("Parameters must be greater than zero.")
            };

        // Расчет добротности для параллельного контура
        public double CalculateParallelQFactor(double inductance, double resistance, double frequency) =>
            (inductance, resistance, frequency) switch
            {
                ( > 0, > 0, > 0) => resistance / (2 * Math.PI * frequency * inductance),
                _ => throw new ArgumentException("Parameters must be greater than zero.")
            };

        // Метод для проверки положительных значений
        private void EnsurePositiveValues((double value, string name)[] values)
        {
            foreach (var (value, name) in values)
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"{name} должно быть положительным.");
                }
            }
        }
        #endregion

        #region Расчеты для коаксиального кабеля

        /// <summary>
        /// Рассчитывает волновое сопротивление коаксиального кабеля
        /// </summary>
        /// <param name="innerDiameter">Внутренний диаметр в метрах</param>
        /// <param name="outerDiameter">Внешний диаметр в метрах</param>
        /// <param name="dielectricConstant">Диэлектрическая постоянная</param>
        /// <returns>Волновое сопротивление в Омах</returns>
        public double CalculateCoaxialCableImpedance(double innerDiameter, double outerDiameter, double dielectricConstant)
        {
            EnsureValidParameters();

            return (60 / Math.Sqrt(dielectricConstant)) * Math.Log10(outerDiameter / innerDiameter);

            // Локальная функция для проверки параметров
            void EnsureValidParameters()
            {
                EnsurePositive(innerDiameter, nameof(innerDiameter));
                EnsurePositive(outerDiameter, nameof(outerDiameter));
                EnsurePositive(dielectricConstant, nameof(dielectricConstant));

                if (innerDiameter >= outerDiameter)
                {
                    throw new ArgumentException("Внутренний диаметр должен быть меньше внешнего диаметра");
                }
            }
        }

        /// <summary>
        /// Рассчитывает затухание в коаксиальном кабеле
        /// </summary>
        /// <param name="innerDiameter">Внутренний диаметр в метрах</param>
        /// <param name="outerDiameter">Внешний диаметр в метрах</param>
        /// <param name="frequency">Частота в Герцах</param>
        /// <param name="dielectricConstant">Диэлектрическая постоянная</param>
        /// <param name="length">Длина кабеля в метрах</param>
        /// <returns>Затухание в дБ</returns>
        public double CalculateCoaxialCableAttenuation(double innerDiameter, double outerDiameter, double frequency, double dielectricConstant, double length)
        {
            EnsureValidParameters();

            double impedance = CalculateCoaxialCableImpedance(innerDiameter, outerDiameter, dielectricConstant);
            double conductorLoss = (1 / innerDiameter + 1 / outerDiameter) * Math.Sqrt(Math.PI * frequency * 4e-7 / (2 * 5.8e7));
            double dielectricLoss = (Math.PI / 3e8) * frequency * Math.Sqrt(dielectricConstant) * Math.Tan(0.001); // Тангенс угла потерь 0.001

            return (conductorLoss + dielectricLoss) * impedance * length * 8.686; // 8.686 для перевода из Np в дБ

            void EnsureValidParameters()
            {
                EnsurePositive(innerDiameter, nameof(innerDiameter));
                EnsurePositive(outerDiameter, nameof(outerDiameter));
                EnsurePositive(frequency, nameof(frequency));
                EnsurePositive(dielectricConstant, nameof(dielectricConstant));
                EnsurePositive(length, nameof(length));

                if (innerDiameter >= outerDiameter)
                {
                    throw new ArgumentException("Внутренний диаметр должен быть меньше внешнего диаметра");
                }
            }
        }

        /// <summary>
        /// Рассчитывает коэффициент скорости в коаксиальном кабеле
        /// </summary>
        /// <param name="dielectricConstant">Диэлектрическая постоянная</param>
        /// <returns>Коэффициент скорости (безразмерная величина)</returns>
        public double CalculateVelocityFactor(double dielectricConstant)
        {
            EnsurePositive(dielectricConstant, nameof(dielectricConstant));
            return 1 / Math.Sqrt(dielectricConstant);
        }

        /// <summary>
        /// Рассчитывает емкость коаксиального кабеля
        /// </summary>
        /// <param name="innerDiameter">Внутренний диаметр в метрах</param>
        /// <param name="outerDiameter">Внешний диаметр в метрах</param>
        /// <param name="dielectricConstant">Диэлектрическая постоянная</param>
        /// <param name="length">Длина кабеля в метрах</param>
        /// <returns>Емкость в Фарадах</returns>
        public double CalculateCoaxialCableCapacitance(double innerDiameter, double outerDiameter, double dielectricConstant, double length)
        {
            EnsureValidParameters();

            return (2 * Math.PI * 8.854e-12 * dielectricConstant * length) / Math.Log(outerDiameter / innerDiameter);

            void EnsureValidParameters()
            {
                EnsurePositive(innerDiameter, nameof(innerDiameter));
                EnsurePositive(outerDiameter, nameof(outerDiameter));
                EnsurePositive(dielectricConstant, nameof(dielectricConstant));
                EnsurePositive(length, nameof(length));

                if (innerDiameter >= outerDiameter)
                {
                    throw new ArgumentException("Внутренний диаметр должен быть меньше внешнего диаметра");
                }
            }
        }
        #endregion

        #region Различные методы проверки

        // Метод проверки на положительность
        private void EnsurePositive(double value, string parameterName)
        {
            if (value <= 0)
                throw new ArgumentException($"{parameterName} должен быть больше нуля.");
        }

        private void ValidatePositive(double value, string paramName = null)
        {
            if (value <= 0)
            {
                if (string.IsNullOrEmpty(paramName))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Значение должно быть положительным.");
                }
                else
                {
                    throw new ArgumentException($"{paramName} должно быть положительным числом.", paramName);
                }
            }
        }

        #endregion
    }
}
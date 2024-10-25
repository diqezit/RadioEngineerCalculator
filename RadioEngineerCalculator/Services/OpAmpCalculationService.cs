using RadioEngineerCalculator.ViewModel;
using System;
using System.Collections.Generic;

namespace RadioEngineerCalculator.Services
{
    /// <summary>
    /// Сервис для расчетов, связанных с операционными усилителями (ОУ).
    /// </summary>
    public class OpAmpCalculationService
    {
        #region Расчеты операционных усилителей

        private readonly OpAmpType _opAmpType;
        private readonly Dictionary<OpAmpType, OpAmpSpecs> _opAmpSpecs;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="OpAmpCalculationService"/>.
        /// </summary>
        /// <param name="opAmpType">Тип операционного усилителя.</param>
        /// <param name="opAmpSpecs">Спецификации операционных усилителей.</param>
        public OpAmpCalculationService(OpAmpType opAmpType, Dictionary<OpAmpType, OpAmpSpecs> opAmpSpecs)
        {
            _opAmpType = opAmpType;
            _opAmpSpecs = opAmpSpecs;
        }

        #region Основные расчеты

        /// <summary>
        /// Рассчитывает выходное напряжение операционного усилителя.
        /// </summary>
        /// <param name="inputVoltage">Входное напряжение.</param>
        /// <param name="gain">Коэффициент усиления.</param>
        /// <param name="supplyVoltage">Напряжение питания.</param>
        /// <param name="mode">Режим работы операционного усилителя.</param>
        /// <param name="opAmpType">Тип операционного усилителя.</param>
        /// <returns>Выходное напряжение.</returns>
        public double CalculateOutputVoltage(double inputVoltage, double gain,
            double supplyVoltage, OperatingMode mode, OpAmpType opAmpType)
        {
            var specs = _opAmpSpecs[opAmpType];
            double outputVoltage = mode switch
            {
                OperatingMode.Inverting => -inputVoltage * gain,
                OperatingMode.NonInverting => inputVoltage * (1 + gain),
                OperatingMode.Differential => inputVoltage * gain * 2,
                OperatingMode.Integrator => CalculateIntegratorOutput(inputVoltage, gain),
                OperatingMode.Differentiator => CalculateDifferentiatorOutput(inputVoltage, gain),
                _ => throw new ArgumentException("Неподдерживаемый режим работы")
            };

            // Ограничение выходного напряжения
            return Math.Min(Math.Max(outputVoltage, -supplyVoltage + 1), supplyVoltage - 1);
        }

        /// <summary>
        /// Рассчитывает фактический коэффициент усиления на заданной частоте.
        /// </summary>
        /// <param name="nominalGain">Номинальный коэффициент усиления.</param>
        /// <param name="frequency">Частота.</param>
        /// <returns>Фактический коэффициент усиления.</returns>
        public double CalculateActualGain(double nominalGain, double frequency)
        {
            var specs = _opAmpSpecs[_opAmpType];
            double unityGainFreq = specs.UnityGainBandwidth * 1e6; // Перевод в Гц
            return Math.Min(nominalGain, unityGainFreq / frequency);
        }

        /// <summary>
        /// Рассчитывает максимальную скорость нарастания напряжения.
        /// </summary>
        /// <param name="frequency">Частота.</param>
        /// <param name="outputVoltage">Выходное напряжение.</param>
        /// <returns>Максимальная скорость нарастания напряжения.</returns>
        public double CalculateMaximumSlewRate(double frequency, double outputVoltage)
        {
            // Расчет минимальной требуемой скорости нарастания
            return 2 * Math.PI * frequency * outputVoltage * 1e-6; // В/мкс
        }

        /// <summary>
        /// Анализирует потребляемую мощность и эффективность операционного усилителя.
        /// </summary>
        /// <param name="opAmpType">Тип операционного усилителя.</param>
        /// <param name="supplyVoltage">Напряжение питания.</param>
        /// <param name="outputCurrent">Выходной ток.</param>
        /// <param name="temperature">Температура.</param>
        /// <returns>Кортеж, содержащий общую потребляемую мощность и эффективность.</returns>
        public (double TotalPower, double Efficiency) AnalyzePower(OpAmpType opAmpType,
            double supplyVoltage, double outputCurrent, double temperature)
        {
            var specs = _opAmpSpecs[opAmpType];

            // Расчет потребляемой мощности
            double quiescentPower = specs.QuiescentCurrent * supplyVoltage * 1e-3; // мА в А
            double outputPower = Math.Abs(outputCurrent * supplyVoltage);
            double totalPower = quiescentPower + outputPower;

            // Расчет КПД
            double efficiency = (outputPower / totalPower) * 100;

            // Учет температурной зависимости
            double temperatureCoefficient = 1 + 0.002 * (temperature - 25); // 0.2% на градус
            totalPower *= temperatureCoefficient;

            return (totalPower, efficiency);
        }

        #endregion

        #region Расчет параметров

        /// <summary>
        /// Рассчитывает фактический коэффициент ослабления синфазного сигнала (CMRR) на заданной частоте.
        /// </summary>
        /// <param name="frequency">Частота.</param>
        /// <returns>Фактический CMRR.</returns>
        public double CalculateActualCMRR(double frequency)
        {
            var specs = _opAmpSpecs[_opAmpType];
            // CMRR уменьшается на 20 дБ/декаду после 100 Гц
            double cmrrAtFreq = specs.CMRRTypical - 20 * Math.Log10(frequency / 100);
            return Math.Max(cmrrAtFreq, 0);
        }

        /// <summary>
        /// Рассчитывает фактический коэффициент ослабления помех от источника питания (PSRR) на заданной частоте.
        /// </summary>
        /// <param name="frequency">Частота.</param>
        /// <returns>Фактический PSRR.</returns>
        public double CalculateActualPSRR(double frequency)
        {
            var specs = _opAmpSpecs[_opAmpType];
            // PSRR уменьшается на 20 дБ/декаду после 100 Гц
            double psrrAtFreq = specs.PSRRTypical - 20 * Math.Log10(frequency / 100);
            return Math.Max(psrrAtFreq, 0);
        }

        /// <summary>
        /// Рассчитывает коэффициент нелинейных искажений (THD) для заданного выходного напряжения и напряжения питания.
        /// </summary>
        /// <param name="outputVoltage">Выходное напряжение.</param>
        /// <param name="supplyVoltage">Напряжение питания.</param>
        /// <returns>Коэффициент нелинейных искажений.</returns>
        public double CalculateDistortion(double outputVoltage, double supplyVoltage)
        {
            // Расчет нелинейных искажений (THD)
            double normalizedOutput = outputVoltage / supplyVoltage;
            return 0.001 + 0.01 * Math.Pow(normalizedOutput, 2); // Базовые искажения + зависимость от амплитуды
        }

        /// <summary>
        /// Рассчитывает выходной импеданс на заданной частоте.
        /// </summary>
        /// <param name="gain">Коэффициент усиления.</param>
        /// <param name="frequency">Частота.</param>
        /// <returns>Выходной импеданс.</returns>
        public double CalculateOutputImpedance(double gain, double frequency)
        {
            var specs = _opAmpSpecs[_opAmpType];
            // Выходной импеданс увеличивается с частотой
            double baseImpedance = 100.0; // базовый выходной импеданс в Омах
            return baseImpedance * (1 + frequency / (specs.UnityGainBandwidth * 1e6));
        }

        #endregion

        #region Температурные эффекты

        /// <summary>
        /// Рассчитывает температурные эффекты для операционного усилителя.
        /// </summary>
        /// <param name="opAmpType">Тип операционного усилителя.</param>
        /// <param name="temperatureDelta">Разница температур от номинальной.</param>
        /// <returns>Кортеж, содержащий дрейф смещения, дрейф усиления и дрейф тока смещения.</returns>
        public (double InputOffsetDrift, double GainDrift, double BiasDrift)
            CalculateTemperatureEffects(OpAmpType opAmpType, double temperatureDelta)
        {
            var specs = _opAmpSpecs[opAmpType];

            // Типичные температурные коэффициенты
            const double offsetDriftCoeff = 2.0; // мкВ/°C
            const double gainDriftCoeff = 0.01; // %/°C
            const double biasDriftCoeff = 0.5; // %/°C

            double inputOffsetDrift = specs.InputOffsetVoltage * offsetDriftCoeff * temperatureDelta / 100;
            double gainDrift = gainDriftCoeff * temperatureDelta;
            double biasDrift = specs.InputBiasCurrentTypical * biasDriftCoeff * temperatureDelta / 100;

            return (inputOffsetDrift, gainDrift, biasDrift);
        }

        #endregion

        #region Анализ шумов

        /// <summary>
        /// Рассчитывает эквивалентный входной шум по напряжению и току.
        /// </summary>
        /// <param name="opAmpType">Тип операционного усилителя.</param>
        /// <param name="frequency">Частота.</param>
        /// <param name="sourceResistance">Сопротивление источника.</param>
        /// <returns>Кортеж, содержащий шум по напряжению и току.</returns>
        public (double VoltageNoise, double CurrentNoise) CalculateEquivalentInputNoise(
            OpAmpType opAmpType, double frequency, double sourceResistance)
        {
            // Расчет шумового напряжения и тока
            double voltageNoise = CalculateVoltageNoise(frequency);
            double currentNoise = CalculateCurrentNoise(frequency);

            return (voltageNoise, currentNoise);
        }

        /// <summary>
        /// Рассчитывает шум по напряжению на заданной частоте.
        /// </summary>
        /// <param name="frequency">Частота.</param>
        /// <returns>Шум по напряжению.</returns>
        private double CalculateVoltageNoise(double frequency)
        {
            // Типичные значения для шума по напряжению
            const double baseVoltageNoise = 10.0; // нВ/√Гц на 1 кГц
            return baseVoltageNoise * Math.Sqrt(1000 / frequency);
        }

        /// <summary>
        /// Рассчитывает шум по току на заданной частоте.
        /// </summary>
        /// <param name="frequency">Частота.</param>
        /// <returns>Шум по току.</returns>
        private double CalculateCurrentNoise(double frequency)
        {
            // Типичные значения для шума по току
            const double baseCurrentNoise = 0.5; // пА/√Гц на 1 кГц
            return baseCurrentNoise * Math.Sqrt(frequency / 1000);
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Рассчитывает выходное напряжение для режима интегратора.
        /// </summary>
        /// <param name="inputVoltage">Входное напряжение.</param>
        /// <param name="gain">Коэффициент усиления.</param>
        /// <returns>Выходное напряжение.</returns>
        private double CalculateIntegratorOutput(double inputVoltage, double gain)
        {
            // Упрощенный расчет для интегратора
            return inputVoltage * gain / 2.0;
        }

        /// <summary>
        /// Рассчитывает выходное напряжение для режима дифференциатора.
        /// </summary>
        /// <param name="inputVoltage">Входное напряжение.</param>
        /// <param name="gain">Коэффициент усиления.</param>
        /// <returns>Выходное напряжение.</returns>
        private double CalculateDifferentiatorOutput(double inputVoltage, double gain)
        {
            // Упрощенный расчет для дифференциатора
            return inputVoltage * gain * 2.0;
        }

        #endregion

        #endregion
    }
}
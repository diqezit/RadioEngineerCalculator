﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static RadioEngineerCalculator.Services.UnitConverter;

namespace RadioEngineerCalculator.Services
{
    /// <summary>
    /// Сервис для расчета параметров фильтров.
    /// </summary>
    public class FiltersCalculationService
    {
        /// <summary>
        /// Типы фильтров.
        /// </summary>
        public enum FilterType
        {
            LowPass,
            HighPass,
            BandPass,
            BandStop
        }

        /// <summary>
        /// Рассчитывает результаты для фильтра на основе входных значений.
        /// </summary>
        /// <param name="inputValues">Входные значения для расчета фильтра.</param>
        /// <returns>Результаты расчета фильтра.</returns>
        /// <exception cref="ApplicationException">Выбрасывается, если произошла ошибка при расчете параметров фильтра.</exception>
        public FilterResults CalculateFilterResults(FilterInputValues inputValues)
        {
            try
            {
                ValidateInputValues(inputValues);

                var (capacitanceInFarads, inductanceInHenries, resistanceInOhms, frequencyInHz) = (
                    Convert(inputValues.Capacitance, inputValues.CapacitanceUnit, "F", PhysicalQuantity.Capacitance),
                    Convert(inputValues.Inductance, inputValues.InductanceUnit, "H", PhysicalQuantity.Inductance),
                    Convert(inputValues.Resistance, inputValues.ResistanceUnit, "Ω", PhysicalQuantity.Resistance),
                    Convert(inputValues.Frequency, inputValues.FrequencyUnit, "Hz", PhysicalQuantity.Frequency)
                );

                var cutoffFrequency = CalculateCutoffFrequency(inductanceInHenries, capacitanceInFarads);
                var qualityFactor = CalculateQualityFactor(resistanceInOhms, inductanceInHenries, capacitanceInFarads);
                var bandwidth = CalculateBandwidth(cutoffFrequency, qualityFactor);
                var safeFrequency = EnsureSafeFrequency(frequencyInHz, cutoffFrequency);

                return new FilterResults
                {
                    FilterType = inputValues.FilterType,
                    CutoffFrequency = Convert(cutoffFrequency, "Hz", inputValues.FrequencyUnit, PhysicalQuantity.Frequency),
                    QualityFactor = qualityFactor,
                    Bandwidth = Convert(bandwidth, "Hz", inputValues.FrequencyUnit, PhysicalQuantity.Frequency),
                    Impedance = CalculateImpedance(safeFrequency, resistanceInOhms, inductanceInHenries, capacitanceInFarads).Magnitude,
                    PhaseShift = CalculatePhaseShift(inputValues.FilterType, safeFrequency, cutoffFrequency, bandwidth),
                    GroupDelay = CalculateGroupDelay(inputValues.FilterType, safeFrequency, cutoffFrequency, bandwidth),
                    Attenuation = CalculateAttenuation(inputValues.FilterType, safeFrequency, cutoffFrequency, bandwidth),
                    FrequencyResponse = CalculateFrequencyResponse(inputValues.FilterType, safeFrequency, cutoffFrequency, bandwidth, inputValues.FrequencyUnit),
                    FrequencyUnit = inputValues.FrequencyUnit,
                    StopbandFrequency = CalculateStopbandFrequency(inputValues.FilterType, cutoffFrequency),
                    StopbandAttenuation = CalculateStopbandAttenuation(inputValues.FilterType, cutoffFrequency, bandwidth),
                    RollOff = CalculateRollOff(inputValues.FilterType)
                };
            }
            catch (ArgumentException ex)
            {
                throw new ApplicationException($"Ошибка при расчете параметров фильтра: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Обеспечивает безопасное значение частоты, ограничивая его в пределах допустимого диапазона.
        /// </summary>
        /// <param name="frequency">Частота.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <returns>Безопасное значение частоты.</returns>
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

        /// <summary>
        /// Рассчитывает амплитудно-частотную характеристику фильтра.
        /// </summary>
        /// <param name="filterType">Тип фильтра.</param>
        /// <param name="frequency">Частота.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="bandwidth">Полоса пропускания.</param>
        /// <returns>Амплитудно-частотная характеристика.</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если частота, частота среза или полоса пропускания не являются положительными числами.</exception>
        public double CalculateMagnitudeResponse(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth) =>
            (frequency, cutoffFrequency, bandwidth) switch
            {
                ( < 0, _, _) => throw new ArgumentException("Частота должна быть положительным числом"),
                (_, <= 0, _) => throw new ArgumentException("Частота среза должна быть положительным числом"),
                (_, _, <= 0) => throw new ArgumentException("Полоса пропускания должна быть положительным числом"),
                _ => filterType switch
                {
                    FilterType.LowPass => 1 / Math.Sqrt(1 + Math.Pow(frequency / cutoffFrequency, 2)),
                    FilterType.HighPass => (frequency / cutoffFrequency) / Math.Sqrt(1 + Math.Pow(frequency / cutoffFrequency, 2)),
                    FilterType.BandPass => CalculateBandPassMagnitude(frequency / cutoffFrequency, cutoffFrequency, bandwidth),
                    FilterType.BandStop => CalculateBandStopMagnitude(frequency / cutoffFrequency, cutoffFrequency, bandwidth),
                    _ => throw new ArgumentException("Неподдерживаемый тип фильтра", nameof(filterType))
                }
            };

        /// <summary>
        /// Рассчитывает частоту заграждения для фильтра.
        /// </summary>
        /// <param name="filterType">Тип фильтра.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <returns>Частота заграждения.</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если тип фильтра не поддерживается.</exception>
        private double CalculateStopbandFrequency(FilterType filterType, double cutoffFrequency)
        {
            return filterType switch
            {
                FilterType.LowPass => cutoffFrequency * 10,
                FilterType.HighPass => cutoffFrequency / 10,
                FilterType.BandPass => cutoffFrequency,
                FilterType.BandStop => cutoffFrequency,
                _ => throw new ArgumentException("Неподдерживаемый тип фильтра", nameof(filterType))
            };
        }

        /// <summary>
        /// Рассчитывает затухание в полосе заграждения.
        /// </summary>
        /// <param name="filterType">Тип фильтра.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="bandwidth">Полоса пропускания.</param>
        /// <returns>Затухание в полосе заграждения.</returns>
        private double CalculateStopbandAttenuation(FilterType filterType, double cutoffFrequency, double bandwidth)
        {
            var stopbandFreq = CalculateStopbandFrequency(filterType, cutoffFrequency);
            return stopbandFreq > 0 ? CalculateAttenuation(filterType, stopbandFreq, cutoffFrequency, bandwidth) : 0;
        }

        /// <summary>
        /// Рассчитывает скорость спада фильтра.
        /// </summary>
        /// <param name="filterType">Тип фильтра.</param>
        /// <returns>Скорость спада фильтра.</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если тип фильтра не поддерживается.</exception>
        private double CalculateRollOff(FilterType filterType)
        {
            return filterType switch
            {
                FilterType.LowPass => -20,
                FilterType.HighPass => -20,
                FilterType.BandPass => -40,
                FilterType.BandStop => -40,
                _ => throw new ArgumentException("Неподдерживаемый тип фильтра", nameof(filterType))
            };
        }

        /// <summary>
        /// Рассчитывает фазовый сдвиг фильтра.
        /// </summary>
        /// <param name="filterType">Тип фильтра.</param>
        /// <param name="frequency">Частота.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="bandwidth">Полоса пропускания.</param>
        /// <returns>Фазовый сдвиг.</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если тип фильтра не поддерживается.</exception>
        public double CalculatePhaseShift(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth)
        {
            double normalizedFrequency = frequency / cutoffFrequency;
            return filterType switch
            {
                FilterType.LowPass => -Math.Atan(normalizedFrequency),
                FilterType.HighPass => Math.PI / 2 - Math.Atan(1 / normalizedFrequency),
                FilterType.BandPass => CalculateBandPassPhase(normalizedFrequency, cutoffFrequency, bandwidth),
                FilterType.BandStop => CalculateBandStopPhase(normalizedFrequency, cutoffFrequency, bandwidth),
                _ => throw new ArgumentException("Неподдерживаемый тип фильтра", nameof(filterType))
            };
        }

        /// <summary>
        /// Рассчитывает групповую задержку фильтра.
        /// </summary>
        /// <param name="filterType">Тип фильтра.</param>
        /// <param name="frequency">Частота.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="bandwidth">Полоса пропускания.</param>
        /// <returns>Групповая задержка.</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если тип фильтра не поддерживается.</exception>
        public double CalculateGroupDelay(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth)
        {
            double normalizedFrequency = frequency / cutoffFrequency;
            return filterType switch
            {
                FilterType.LowPass => 1 / (2 * Math.PI * frequency * (1 + Math.Pow(normalizedFrequency, 2))),
                FilterType.HighPass => 1 / (2 * Math.PI * frequency * (1 + Math.Pow(1 / normalizedFrequency, 2))),
                FilterType.BandPass or FilterType.BandStop => CalculateBandGroupDelay(normalizedFrequency, cutoffFrequency, bandwidth),
                _ => throw new ArgumentException("Неподдерживаемый тип фильтра", nameof(filterType))
            };
        }

        /// <summary>
        /// Рассчитывает затухание фильтра.
        /// </summary>
        /// <param name="filterType">Тип фильтра.</param>
        /// <param name="frequency">Частота.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="bandwidth">Полоса пропускания.</param>
        /// <returns>Затухание.</returns>
        public double CalculateAttenuation(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth)
        {
            double magnitudeResponse = CalculateMagnitudeResponse(filterType, frequency, cutoffFrequency, bandwidth);
            return -20 * Math.Log10(magnitudeResponse);
        }

        /// <summary>
        /// Рассчитывает частоту среза фильтра.
        /// </summary>
        /// <param name="inductance">Индуктивность.</param>
        /// <param name="capacitance">Емкость.</param>
        /// <returns>Частота среза.</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если индуктивность или емкость не являются положительными числами.</exception>
        private static double CalculateCutoffFrequency(double inductance, double capacitance)
        {
            if (inductance <= 0 || capacitance <= 0)
            {
                throw new ArgumentException("Индуктивность и емкость должны быть положительными числами");
            }
            return 1 / (2 * Math.PI * Math.Sqrt(inductance * capacitance));
        }

        /// <summary>
        /// Рассчитывает добротность фильтра.
        /// </summary>
        /// <param name="resistance">Сопротивление.</param>
        /// <param name="inductance">Индуктивность.</param>
        /// <param name="capacitance">Емкость.</param>
        /// <returns>Добротность.</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если сопротивление, индуктивность или емкость не являются положительными числами.</exception>
        private static double CalculateQualityFactor(double resistance, double inductance, double capacitance)
        {
            if (resistance <= 0 || inductance <= 0 || capacitance <= 0)
            {
                throw new ArgumentException("Сопротивление, индуктивность и емкость должны быть положительными числами");
            }
            return (1 / resistance) * Math.Sqrt(inductance / capacitance);
        }

        /// <summary>
        /// Рассчитывает полосу пропускания фильтра.
        /// </summary>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="qualityFactor">Добротность.</param>
        /// <returns>Полоса пропускания.</returns>
        private static double CalculateBandwidth(double cutoffFrequency, double qualityFactor)
        {
            return cutoffFrequency / qualityFactor;
        }

        /// <summary>
        /// Рассчитывает импеданс фильтра.
        /// </summary>
        /// <param name="frequency">Частота.</param>
        /// <param name="resistance">Сопротивление.</param>
        /// <param name="inductance">Индуктивность.</param>
        /// <param name="capacitance">Емкость.</param>
        /// <returns>Импеданс.</returns>
        private static Complex CalculateImpedance(double frequency, double resistance, double inductance, double capacitance)
        {
            double omega = 2 * Math.PI * frequency;
            return new Complex(resistance, omega * inductance - 1 / (omega * capacitance));
        }

        /// <summary>
        /// Рассчитывает частотную характеристику фильтра.
        /// </summary>
        /// <param name="filterType">Тип фильтра.</param>
        /// <param name="frequency">Частота.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="bandwidth">Полоса пропускания.</param>
        /// <param name="frequencyUnit">Единица измерения частоты.</param>
        /// <returns>Частотная характеристика.</returns>
        private List<Point> CalculateFrequencyResponse(FilterType filterType, double frequency, double cutoffFrequency, double bandwidth, string frequencyUnit)
        {
            const int pointCount = 1000;
            double startFreq = Math.Max(frequency / 100, 1e-6 * cutoffFrequency);
            double endFreq = Math.Min(frequency * 100, 1e6 * cutoffFrequency);

            return Enumerable.Range(0, pointCount)
                .Select(i =>
                {
                    double freq = Math.Pow(10, Math.Log10(startFreq) + (Math.Log10(endFreq) - Math.Log10(startFreq)) * i / (pointCount - 1));
                    double gain = CalculateMagnitudeResponse(filterType, freq, cutoffFrequency, bandwidth);
                    return new Point
                    {
                        X = Convert(freq, "Hz", frequencyUnit, PhysicalQuantity.Frequency),
                        Y = 20 * Math.Log10(gain)
                    };
                }).ToList();
        }

        /// <summary>
        /// Проверяет допустимость единицы измерения.
        /// </summary>
        /// <param name="unit">Единица измерения.</param>
        /// <param name="quantity">Физическая величина.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если единица измерения недопустима.</exception>
        private static void ValidateUnit(string? unit, PhysicalQuantity quantity)
        {
            if (unit is null || !UnitFactors.GetUnitFactors(quantity).Any(u => u.Symbol == unit))
                throw new ArgumentException($"Недопустимая единица измерения для {quantity}: {unit}");
        }

        /// <summary>
        /// Рассчитывает амплитудно-частотную характеристику полосового фильтра.
        /// </summary>
        /// <param name="normalizedFrequency">Нормализованная частота.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="bandwidth">Полоса пропускания.</param>
        /// <returns>Амплитудно-частотная характеристика полосового фильтра.</returns>
        private static double CalculateBandPassMagnitude(double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            return 1 / Math.Sqrt(1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2));
        }

        /// <summary>
        /// Рассчитывает амплитудно-частотную характеристику режекторного фильтра.
        /// </summary>
        /// <param name="normalizedFrequency">Нормализованная частота.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="bandwidth">Полоса пропускания.</param>
        /// <returns>Амплитудно-частотная характеристика режекторного фильтра.</returns>
        private static double CalculateBandStopMagnitude(double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            return Math.Abs(q * (normalizedFrequency - 1 / normalizedFrequency)) / Math.Sqrt(1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2));
        }

        /// <summary>
        /// Рассчитывает фазовый сдвиг полосового фильтра.
        /// </summary>
        /// <param name="normalizedFrequency">Нормализованная частота.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="bandwidth">Полоса пропускания.</param>
        /// <returns>Фазовый сдвиг полосового фильтра.</returns>
        private static double CalculateBandPassPhase(double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            return Math.Atan(q * (normalizedFrequency - 1 / normalizedFrequency));
        }

        /// <summary>
        /// Рассчитывает фазовый сдвиг режекторного фильтра.
        /// </summary>
        /// <param name="normalizedFrequency">Нормализованная частота.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="bandwidth">Полоса пропускания.</param>
        /// <returns>Фазовый сдвиг режекторного фильтра.</returns>
        private static double CalculateBandStopPhase(double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            return -Math.Atan(q * (normalizedFrequency - 1 / normalizedFrequency));
        }

        /// <summary>
        /// Рассчитывает групповую задержку полосового и режекторного фильтров.
        /// </summary>
        /// <param name="normalizedFrequency">Нормализованная частота.</param>
        /// <param name="cutoffFrequency">Частота среза.</param>
        /// <param name="bandwidth">Полоса пропускания.</param>
        /// <returns>Групповая задержка.</returns>
        private static double CalculateBandGroupDelay(double normalizedFrequency, double cutoffFrequency, double bandwidth)
        {
            double q = cutoffFrequency / bandwidth;
            return q / (Math.PI * cutoffFrequency * (1 + Math.Pow(q * (normalizedFrequency - 1 / normalizedFrequency), 2)));
        }

        /// <summary>
        /// Проверяет входные значения на допустимость.
        /// </summary>
        /// <param name="inputValues">Входные значения.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если входные значения равны null.</exception>
        /// <exception cref="ArgumentException">Выбрасывается, если какое-либо из входных значений не является положительным или единица измерения недопустима.</exception>
        private static void ValidateInputValues(FilterInputValues inputValues)
        {
            _ = inputValues ?? throw new ArgumentNullException(nameof(inputValues));

            if (inputValues.Capacitance <= 0 || inputValues.Inductance <= 0 || inputValues.Resistance <= 0 || inputValues.Frequency <= 0)
                throw new ArgumentException("Все значения должны быть положительными.");

            if (string.IsNullOrEmpty(inputValues.FrequencyUnit))
                throw new ArgumentException("Единица измерения частоты не указана.");

            foreach (var (unit, quantity) in new[]
            {
                (inputValues.CapacitanceUnit, PhysicalQuantity.Capacitance),
                (inputValues.InductanceUnit, PhysicalQuantity.Inductance),
                (inputValues.ResistanceUnit, PhysicalQuantity.Resistance),
                (inputValues.FrequencyUnit, PhysicalQuantity.Frequency)
            })
            {
                ValidateUnit(unit, quantity);
            }
        }
    }

    /// <summary>
    /// Класс, представляющий входные значения для расчета фильтра.
    /// </summary>
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

    /// <summary>
    /// Класс, представляющий результаты расчета фильтра.
    /// </summary>
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
        public List<Point> FrequencyResponse { get; set; } = new List<Point>();
        public string FrequencyUnit { get; set; }
        public double StopbandFrequency { get; set; }
        public double StopbandAttenuation { get; set; }
        public double RollOff { get; set; }
    }

    /// <summary>
    /// Структура, представляющая точку на графике.
    /// </summary>
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
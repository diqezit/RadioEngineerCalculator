using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using RadioEngineerCalculator.Services;
using static RadioEngineerCalculator.Services.FiltersCalculationService;

namespace RadioEngineerCalculator.Services
{
    public class Graph
    {
        #region Fields
        private readonly PlotModel _plotModel;
        private readonly FiltersCalculationService _filtersCalculationService;
        #endregion

        #region Constructor
        public Graph(PlotModel plotModel, FiltersCalculationService filtersCalculationService)
        {
            _plotModel = plotModel ?? throw new ArgumentNullException(nameof(plotModel));
            _filtersCalculationService = filtersCalculationService ?? throw new ArgumentNullException(nameof(filtersCalculationService));
        }
        #endregion

        #region Public Methods
        public void UpdateFilterResponsePlot(FilterResults results, string stopbandResult, string rollOffResult)
        {
            if (results is null) throw new ArgumentNullException(nameof(results));

            _plotModel.Series.Clear();
            _plotModel.Axes.Clear();

            AddAxes();
            AddSeries(results);
            AddStopbandSeries(results, stopbandResult);
            AddRollOffSeries(results, rollOffResult);

            _plotModel.InvalidatePlot(true);
        }
        #endregion

        #region Private Methods
        private void AddStopbandSeries(FilterResults results, string stopbandResult)
        {
            if (string.IsNullOrEmpty(stopbandResult)) return;

            double stopbandFrequency = results.FilterType switch
            {
                FilterType.LowPass => results.CutoffFrequency * 10,
                FilterType.HighPass => results.CutoffFrequency / 10,
                _ => throw new ArgumentOutOfRangeException()
            };

            double stopbandAttenuation = _filtersCalculationService.CalculateAttenuation(
                results.FilterType, stopbandFrequency, results.CutoffFrequency, results.Bandwidth);

            var stopbandSeries = new LineSeries
            {
                Title = "Полоса заграждения",
                Color = OxyColors.Green,
                LineStyle = LineStyle.Dash
            };

            stopbandSeries.Points.Add(new DataPoint(stopbandFrequency, stopbandAttenuation));
            stopbandSeries.Points.Add(new DataPoint(stopbandFrequency, -60)); // Линия до -60 дБ

            _plotModel.Series.Add(stopbandSeries);
        }

        private void AddRollOffSeries(FilterResults results, string rollOffResult)
        {
            if (string.IsNullOrEmpty(rollOffResult)) return;

            double rollOff = results.FilterType switch
            {
                FilterType.LowPass => -20,
                FilterType.HighPass => -20,
                _ => -40 // dB/decade для BandPass и BandStop
            };

            var rollOffSeries = new LineSeries
            {
                Title = "Крутизна спада",
                Color = OxyColors.Purple,
                LineStyle = LineStyle.Dot
            };

            double startFrequency = results.CutoffFrequency / 2;
            double endFrequency = results.CutoffFrequency * 2;

            rollOffSeries.Points.Add(new DataPoint(startFrequency, 0));
            rollOffSeries.Points.Add(new DataPoint(endFrequency, rollOff));

            _plotModel.Series.Add(rollOffSeries);
        }

        private void AddSeries(FilterResults results)
        {
            const int pointCount = 1000;
            double minFreq = results.CutoffFrequency / 100;
            double maxFreq = results.CutoffFrequency * 100;

            var points = GeneratePoints(results, minFreq, maxFreq, pointCount);

            var magnitudeSeries = CreateSeries("Magnitude", OxyColors.Blue);
            var phaseSeries = CreateSeries("Phase", OxyColors.Red, "PhaseAxis");

            magnitudeSeries.Points.AddRange(points.Select(p => new DataPoint(p.Frequency, p.Magnitude)));
            phaseSeries.Points.AddRange(points.Select(p => new DataPoint(p.Frequency, p.Phase)));

            _plotModel.Series.Add(magnitudeSeries);
            _plotModel.Series.Add(phaseSeries);
        }

        private LineSeries CreateSeries(string title, OxyColor color, string yAxisKey = null)
        {
            return new LineSeries
            {
                Title = title,
                Color = color,
                YAxisKey = yAxisKey
            };
        }

        private IEnumerable<(double Frequency, double Magnitude, double Phase)> GeneratePoints(FilterResults results, double minFreq, double maxFreq, int pointCount)
        {
            for (int i = 0; i < pointCount; i++)
            {
                double freq = minFreq * Math.Pow(maxFreq / minFreq, (double)i / (pointCount - 1));
                double magnitude = _filtersCalculationService.CalculateMagnitudeResponse(results.FilterType, freq, results.CutoffFrequency, results.Bandwidth);
                double phase = _filtersCalculationService.CalculatePhaseShift(results.FilterType, freq, results.CutoffFrequency, results.Bandwidth);

                yield return (
                    Frequency: freq,
                    Magnitude: 20 * Math.Log10(magnitude),
                    Phase: phase * 180 / Math.PI
                );
            }
        }

        private void AddAxes()
        {
            _plotModel.Axes.Add(CreateFrequencyAxis());
            _plotModel.Axes.Add(CreateMagnitudeAxis());
            _plotModel.Axes.Add(CreatePhaseAxis());
        }

        private Axis CreateFrequencyAxis()
        {
            return new LogarithmicAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Частота (Гц)",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Base = 10,
                StringFormat = "0.0E0"
            };
        }

        private Axis CreateMagnitudeAxis()
        {
            return new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Амплитуда (дБ)",
                Minimum = -60,
                Maximum = 20,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorStep = 20,
                MinorStep = 5
            };
        }

        private Axis CreatePhaseAxis()
        {
            return new LinearAxis
            {
                Key = "PhaseAxis",
                Position = AxisPosition.Right,
                Title = "Фаза (градусы)",
                Minimum = -180,
                Maximum = 180,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorStep = 90,
                MinorStep = 30
            };
        }
        #endregion
    }
}
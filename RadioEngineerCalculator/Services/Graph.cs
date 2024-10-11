using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;

namespace RadioEngineerCalculator.Services
{
    public class Graph
    {
        private readonly PlotModel _plotModel;
        private readonly FiltersCalculationService _filtersCalculationService;

        public Graph(PlotModel plotModel, FiltersCalculationService filtersCalculationService)
        {
            _plotModel = plotModel;
            _filtersCalculationService = filtersCalculationService;
        }

        public void UpdateFilterResponsePlot(FilterResults results)
        {
            _plotModel.Series.Clear();
            _plotModel.Axes.Clear();

            AddAxes();

            var magnitudeSeries = new LineSeries
            {
                Title = "Амплитудно-частотная характеристика",
                Color = OxyColors.Blue
            };

            var phaseSeries = new LineSeries
            {
                Title = "Фазо-частотная характеристика",
                Color = OxyColors.Red,
                YAxisKey = "PhaseAxis"
            };

            const int pointCount = 1000;
            double minFreq = results.CutoffFrequency / 100;
            double maxFreq = results.CutoffFrequency * 100;

            for (int i = 0; i < pointCount; i++)
            {
                double freq = minFreq * Math.Pow(maxFreq / minFreq, (double)i / (pointCount - 1));
                double magnitude = _filtersCalculationService.CalculateFilterMagnitudeResponse(results.FilterType, freq, results.CutoffFrequency, results.Bandwidth);
                double phase = _filtersCalculationService.CalculateFilterPhaseResponse(results.FilterType, freq, results.CutoffFrequency, results.Bandwidth);

                magnitudeSeries.Points.Add(new DataPoint(freq, 20 * Math.Log10(magnitude)));
                phaseSeries.Points.Add(new DataPoint(freq, phase * 180 / Math.PI));
            }

            _plotModel.Series.Add(magnitudeSeries);
            _plotModel.Series.Add(phaseSeries);

            _plotModel.InvalidatePlot(true);
        }

        private void AddAxes()
        {
            _plotModel.Axes.Add(new LogarithmicAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Частота (Гц)",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Base = 10,
                MajorStep = 1,
                MinorStep = 0.1,
                StringFormat = "1E0"
            });

            _plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Амплитуда (дБ)",
                Minimum = -60,
                Maximum = 20,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorStep = 20,
                MinorStep = 5
            });

            _plotModel.Axes.Add(new LinearAxis
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
            });
        }
    }
}
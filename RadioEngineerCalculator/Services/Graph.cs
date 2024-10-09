using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RadioEngineerCalculator.Services;
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
            _plotModel.Series.Clear(); // Очищаем серию графиков
            _plotModel.Axes.Clear();   // Очищаем оси графиков

            // Добавляем ось для частоты
            var frequencyAxis = new LogarithmicAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Частота (Гц)",
                Minimum = results.CutoffFrequency / 100, // Минимальная частота
                Maximum = results.CutoffFrequency * 100  // Максимальная частота
            };

            // Добавляем ось для амплитуды
            var magnitudeAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Амплитуда (дБ)",
                Minimum = -60,  // Минимум для амплитуды
                Maximum = 10    // Максимум для амплитуды
            };

            // Добавляем ось для фазы
            var phaseAxis = new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "Фаза (градусы)",
                Key = "PhaseAxis", // Ключ для оси фазы
                Minimum = -180,    // Минимум для фазы
                Maximum = 180      // Максимум для фазы
            };

            _plotModel.Axes.Add(frequencyAxis);
            _plotModel.Axes.Add(magnitudeAxis);
            _plotModel.Axes.Add(phaseAxis);

            var magnitudeSeries = new LineSeries
            {
                Title = "Амплитудно-частотная характеристика",
                Color = OxyColors.Blue // Цвет линии амплитуды
            };

            var phaseSeries = new LineSeries
            {
                Title = "Фазо-частотная характеристика",
                Color = OxyColors.Red, // Цвет линии фазы
                YAxisKey = "PhaseAxis"  // Привязываем к оси фазы
            };

            const int pointCount = 1000; // Количество точек
            double minFreq = results.CutoffFrequency / 100; // Минимальная частота
            double maxFreq = results.CutoffFrequency * 100; // Максимальная частота

            for (int i = 0; i < pointCount; i++)
            {
                double freq = minFreq * Math.Pow(maxFreq / minFreq, (double)i / (pointCount - 1));
                double magnitude = _filtersCalculationService.CalculateFilterMagnitudeResponse(results.FilterType, freq, results.CutoffFrequency, results.Bandwidth);
                double phase = _filtersCalculationService.CalculateFilterPhaseResponse(results.FilterType, freq, results.CutoffFrequency, results.Bandwidth);

                magnitudeSeries.Points.Add(new DataPoint(freq, magnitude)); // Добавляем точку амплитуды
                phaseSeries.Points.Add(new DataPoint(freq, phase));       // Добавляем точку фазы
            }

            _plotModel.Series.Add(magnitudeSeries); // Добавляем серию амплитуды на график
            _plotModel.Series.Add(phaseSeries);     // Добавляем серию фазы на график

            _plotModel.InvalidatePlot(true); // Обновляем график
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

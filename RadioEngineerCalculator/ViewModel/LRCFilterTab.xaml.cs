using OxyPlot;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.ComboBoxService;
using static RadioEngineerCalculator.Services.FiltersCalculationService;
using static RadioEngineerCalculator.Services.UnitConverter;
using static RadioEngineerCalculator.Services.Validate;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class LRCFilterTab : UserControl, INotifyPropertyChanged
    {
        #region Поля и свойства

        private readonly FiltersCalculationService _filtersCalculationService;
        private PlotModel _filterResponseModel;
        private (double Capacitance, double Inductance, double Resistance, double Frequency) _parameters = (1, 1, 1, 1000);
        private (string CapacitanceUnit, string InductanceUnit, string ResistanceUnit, string FrequencyUnit, string FilterType) _selectedUnits;
        private (string CutoffFrequency, string QualityFactor, string Bandwidth, string Impedance, string PhaseShift, string GroupDelay, string Attenuation) _results;
        private bool _canCalculate;

        public ICommand CalculateCommand { get; }
        public ObservableCollection<string> CapacitanceUnits { get; }
        public ObservableCollection<string> InductanceUnits { get; }
        public ObservableCollection<string> ResistanceUnits { get; }
        public ObservableCollection<string> FrequencyUnits { get; }
        public ObservableCollection<string> FilterTypes { get; }

        public bool CanCalculate
        {
            get => _canCalculate;
            set => SetProperty(ref _canCalculate, value);
        }

        public string SelectedCapacitanceUnit
        {
            get => _selectedUnits.CapacitanceUnit;
            set => SetSelectedUnit(ref _selectedUnits.CapacitanceUnit, value);
        }

        public string SelectedFilterType
        {
            get => _selectedUnits.FilterType;
            set => SetSelectedUnit(ref _selectedUnits.FilterType, value);
        }

        public string SelectedFrequencyUnit
        {
            get => _selectedUnits.FrequencyUnit;
            set => SetSelectedUnit(ref _selectedUnits.FrequencyUnit, value, UpdateCanCalculate);
        }

        public string SelectedInductanceUnit
        {
            get => _selectedUnits.InductanceUnit;
            set => SetSelectedUnit(ref _selectedUnits.InductanceUnit, value);
        }

        public string SelectedResistanceUnit
        {
            get => _selectedUnits.ResistanceUnit;
            set => SetSelectedUnit(ref _selectedUnits.ResistanceUnit, value);
        }

        public double Capacitance
        {
            get => _parameters.Capacitance;
            set => SetParameter(ref _parameters.Capacitance, value);
        }

        public double Inductance
        {
            get => _parameters.Inductance;
            set => SetParameter(ref _parameters.Inductance, value);
        }

        public double Frequency
        {
            get => _parameters.Frequency;
            set => SetParameter(ref _parameters.Frequency, value);
        }

        public double Resistance
        {
            get => _parameters.Resistance;
            set => SetParameter(ref _parameters.Resistance, value);
        }

        public PlotModel FilterResponseModel
        {
            get => _filterResponseModel;
            set => SetProperty(ref _filterResponseModel, value);
        }

        public string CutoffFrequencyResult
        {
            get => _results.CutoffFrequency;
            set => SetProperty(ref _results.CutoffFrequency, value);
        }

        public string QualityFactorResult
        {
            get => _results.QualityFactor;
            set => SetProperty(ref _results.QualityFactor, value);
        }

        public string BandwidthResult
        {
            get => _results.Bandwidth;
            set => SetProperty(ref _results.Bandwidth, value);
        }

        public string ImpedanceResult
        {
            get => _results.Impedance;
            set => SetProperty(ref _results.Impedance, value);
        }

        public string PhaseShiftResult
        {
            get => _results.PhaseShift;
            set => SetProperty(ref _results.PhaseShift, value);
        }

        public string GroupDelayResult
        {
            get => _results.GroupDelay;
            set => SetProperty(ref _results.GroupDelay, value);
        }

        public string AttenuationResult
        {
            get => _results.Attenuation;
            set => SetProperty(ref _results.Attenuation, value);
        }

        private string _stopbandResult;
        public string StopbandResult
        {
            get => _stopbandResult;
            set => SetProperty(ref _stopbandResult, value);
        }

        private string _rollOffResult;
        public string RollOffResult
        {
            get => _rollOffResult;
            set => SetProperty(ref _rollOffResult, value);
        }

        #endregion

        #region Конструктор и инициализация

        public LRCFilterTab()
        {
            InitializeComponent();
            _filtersCalculationService = new FiltersCalculationService();
            CapacitanceUnits = new ObservableCollection<string>(GetUnits("Capacitance"));
            InductanceUnits = new ObservableCollection<string>(GetUnits("Inductance"));
            ResistanceUnits = new ObservableCollection<string>(GetUnits("Resistance"));
            FrequencyUnits = new ObservableCollection<string>(GetUnits("Frequency"));
            FilterTypes = new ObservableCollection<string> { "LowPass", "HighPass", "BandPass", "BandStop" };
            InitializeDefaultValues();
            CalculateCommand = new RelayCommand(CalculateFilterParameters, () => CanCalculate);
            DataContext = this;
        }

        private void InitializeDefaultValues()
        {
            SelectedCapacitanceUnit = CapacitanceUnits.FirstOrDefault();
            SelectedInductanceUnit = InductanceUnits.FirstOrDefault();
            SelectedResistanceUnit = ResistanceUnits.FirstOrDefault();
            SelectedFrequencyUnit = FrequencyUnits.FirstOrDefault();
            SelectedFilterType = FilterTypes.FirstOrDefault();

            FilterResponseModel = new PlotModel { Title = "Амплитудно-частотная характеристика" };
            UpdateCanCalculate();
        }

        #endregion

        #region Обработка событий

        private void OnParameterChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.BorderBrush = double.TryParse(textBox.Text, out _)
                    ? System.Windows.Media.Brushes.Gray
                    : System.Windows.Media.Brushes.Red;
            }
            UpdateCanCalculate();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem != null)
            {
                UpdateCanCalculate();
            }
        }

        #endregion

        #region Вспомогательные методы

        private void UpdateCanCalculate()
        {
            CanCalculate = InputsAreValid(_parameters.Capacitance, _parameters.Inductance, _parameters.Resistance, _parameters.Frequency) &&
                           IsAllUnitsSelected();

            bool IsAllUnitsSelected() =>
                !string.IsNullOrWhiteSpace(_selectedUnits.FilterType) &&
                !string.IsNullOrWhiteSpace(_selectedUnits.CapacitanceUnit) &&
                !string.IsNullOrWhiteSpace(_selectedUnits.InductanceUnit) &&
                !string.IsNullOrWhiteSpace(_selectedUnits.ResistanceUnit) &&
                !string.IsNullOrWhiteSpace(_selectedUnits.FrequencyUnit);
        }

        private void CalculateStopband(FilterResults results)
        {
            double stopbandFrequency = 0;
            double stopbandAttenuation = 0;

            stopbandFrequency = results.FilterType switch
            {
                FilterType.LowPass => results.CutoffFrequency * 10,
                FilterType.HighPass => results.CutoffFrequency / 10,
                FilterType.BandPass => throw new InvalidOperationException("Расчет полосы заграждения не применим для полосовых фильтров"),
                FilterType.BandStop => throw new InvalidOperationException("Расчет полосы заграждения не применим для полосовых фильтров"),
                _ => throw new NotSupportedException("Неизвестный тип фильтра")
            };

            stopbandAttenuation = _filtersCalculationService.CalculateAttenuation(
                results.FilterType, stopbandFrequency, results.CutoffFrequency, results.Bandwidth);

            StopbandResult = $"Полоса заграждения: частота {Formatter.Format(stopbandFrequency, PhysicalQuantity.Frequency)}, затухание {stopbandAttenuation:F2} дБ";
        }

        private void CalculateFilterParameters()
        {
            try
            {
                SetLoadingOverlayVisibility(Visibility.Visible);

                var inputValues = new FilterInputValues
                {
                    FilterType = (FilterType)Enum.Parse(typeof(FilterType), _selectedUnits.FilterType),
                    Capacitance = _parameters.Capacitance,
                    Inductance = _parameters.Inductance,
                    Resistance = _parameters.Resistance,
                    Frequency = _parameters.Frequency,
                    CapacitanceUnit = _selectedUnits.CapacitanceUnit,
                    InductanceUnit = _selectedUnits.InductanceUnit,
                    ResistanceUnit = _selectedUnits.ResistanceUnit,
                    FrequencyUnit = _selectedUnits.FrequencyUnit
                };

                var results = _filtersCalculationService.CalculateFilterResults(inputValues);
                UpdateUIWithResults(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при расчете: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ClearResults();
            }
            finally
            {
                SetLoadingOverlayVisibility(Visibility.Collapsed);
            }
        }

        private void ClearResults()
        {
            _results = default;
            StopbandResult = string.Empty;

            foreach (var propertyName in new[]
            {
        nameof(CutoffFrequencyResult),
        nameof(QualityFactorResult),
        nameof(BandwidthResult),
        nameof(ImpedanceResult),
        nameof(PhaseShiftResult),
        nameof(GroupDelayResult),
        nameof(AttenuationResult),
        nameof(StopbandResult)
    })
            {
                OnPropertyChanged(propertyName);
            }

            FilterResponseModel.Series.Clear();
            FilterResponseModel.InvalidatePlot(true);
        }

        private void SetLoadingOverlayVisibility(Visibility visibility)
        {
            if (LoadingOverlay != null)
            {
                LoadingOverlay.Visibility = visibility;
            }
        }

        private void CalculateRollOff(FilterResults results)
        {
            double rollOff = results.FilterType switch
            {
                FilterType.LowPass => -20, // Для фильтров первого порядка крутизна спада обычно составляет -20 дБ/декаду
                FilterType.HighPass => -20,
                FilterType.BandPass => -40, // Для полосовых фильтров второго порядка крутизна спада обычно составляет -40 дБ/декаду
                FilterType.BandStop => -40,
                _ => throw new NotSupportedException("Неизвестный тип фильтра")
            };

            RollOffResult = $"Крутизна спада АЧХ: {rollOff} дБ/декаду";
        }


        private void UpdateUIWithResults(FilterResults results)
        {
            CutoffFrequencyResult = $"Частота среза: {Formatter.Format(results.CutoffFrequency, PhysicalQuantity.Frequency)}";
            QualityFactorResult = $"Добротность (Q): {results.QualityFactor:F2}";
            BandwidthResult = $"Полоса пропускания: {Formatter.Format(results.Bandwidth, PhysicalQuantity.Frequency)}";
            ImpedanceResult = $"Импеданс: {Formatter.Format(results.Impedance, PhysicalQuantity.Resistance)}";
            PhaseShiftResult = $"Фазовый сдвиг: {results.PhaseShift:F2}°";
            GroupDelayResult = $"Групповая задержка: {Formatter.Format(results.GroupDelay, PhysicalQuantity.Time)}";
            AttenuationResult = $"Затухание: {Formatter.Decibels(results.Attenuation)}";

            CalculateStopband(results);
            CalculateRollOff(results);

            // Передаем два аргумента: results и значения Stopband и RollOff
            UpdateFilterResponsePlot(results);
        }


        private void UpdateFilterResponsePlot(FilterResults results)
        {
            var graph = new Graph(FilterResponseModel, _filtersCalculationService);
            graph.UpdateFilterResponsePlot(results, StopbandResult, RollOffResult);
            OnPropertyChanged(nameof(FilterResponseModel));
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void SetSelectedUnit(ref string field, string value, Action additionalAction = null)
        {
            if (SetProperty(ref field, value))
            {
                additionalAction?.Invoke();
                UpdateCanCalculate();
            }
        }

        private void SetParameter(ref double field, double value)
        {
            if (SetProperty(ref field, value))
            {
                UpdateCanCalculate();
            }
        }

        #endregion
    }
}
using OxyPlot;
using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.FiltersCalculationService;
using static RadioEngineerCalculator.Services.UnitC;
using static RadioEngineerCalculator.Services.Validate;
using static RadioEngineerCalculator.Services.ComboBoxService;
using static RadioEngineerCalculator.Infos.ErrorMessages;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class LRCFilterTab : UserControl, INotifyPropertyChanged
    {
        private readonly FiltersCalculationService _filtersCalculationService;
        private PlotModel _filterResponseModel;
        private (double Capacitance, double Inductance, double Resistance, double Frequency) _parameters = (1, 1, 1, 1000);
        private (string CapacitanceUnit, string InductanceUnit, string ResistanceUnit, string FrequencyUnit, string FilterType) _selectedUnits;
        private (string CutoffFrequency, string QualityFactor, string Bandwidth, string Impedance, string PhaseShift, string GroupDelay, string Attenuation) _results;
        private bool _canCalculate;

        public ICommand CalculateCommand { get; }
        public ObservableCollection<string> CapacitanceUnits { get; } = new ObservableCollection<string>(GetUnits("Capacitance"));
        public ObservableCollection<string> InductanceUnits { get; } = new ObservableCollection<string>(GetUnits("Inductance"));
        public ObservableCollection<string> ResistanceUnits { get; } = new ObservableCollection<string>(GetUnits("Resistance"));
        public ObservableCollection<string> FrequencyUnits { get; } = new ObservableCollection<string>(GetUnits("Frequency"));
        public ObservableCollection<string> FilterTypes { get; } = new ObservableCollection<string> { "LowPass", "HighPass", "BandPass", "BandStop" };



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

        public LRCFilterTab()
        {
            InitializeComponent();
            _filtersCalculationService = new FiltersCalculationService();
            InitializeDefaultValues();
            CalculateCommand = new RelayCommand(CalculateFilterParameters, () => CanCalculate);
            DataContext = this;
        }

        private void InitializeDefaultValues()
        {
            _selectedUnits = (
                CapacitanceUnits.FirstOrDefault(),
                InductanceUnits.FirstOrDefault(),
                ResistanceUnits.FirstOrDefault(),
                FrequencyUnits.FirstOrDefault(),
                FilterTypes.FirstOrDefault()
            );

            FilterResponseModel = new PlotModel { Title = "Амплитудно-частотная характеристика" };
            UpdateCanCalculate();
        }

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

        private void UpdateCanCalculate()
        {
            CanCalculate = InputsAreValid(_parameters.Capacitance, _parameters.Inductance, _parameters.Resistance, _parameters.Frequency) &&
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

            switch (results.FilterType)
            {
                case FilterType.LowPass:
                    stopbandFrequency = results.CutoffFrequency * 10;
                    stopbandAttenuation = _filtersCalculationService.CalculateAttenuation(
                        results.FilterType, stopbandFrequency, results.CutoffFrequency, results.Bandwidth);
                    break;
                case FilterType.HighPass:
                    stopbandFrequency = results.CutoffFrequency / 10;
                    stopbandAttenuation = _filtersCalculationService.CalculateAttenuation(
                        results.FilterType, stopbandFrequency, results.CutoffFrequency, results.Bandwidth);
                    break;
                case FilterType.BandPass:
                case FilterType.BandStop:
                    StopbandResult = "Расчет полосы заграждения не применим для полосовых фильтров";
                    return;
            }

            StopbandResult = $"Полоса заграждения: частота {Form.Frequency(stopbandFrequency)}, затухание {stopbandAttenuation:F2} дБ";
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
            OnPropertyChanged(nameof(CutoffFrequencyResult));
            OnPropertyChanged(nameof(QualityFactorResult));
            OnPropertyChanged(nameof(BandwidthResult));
            OnPropertyChanged(nameof(ImpedanceResult));
            OnPropertyChanged(nameof(PhaseShiftResult));
            OnPropertyChanged(nameof(GroupDelayResult));
            OnPropertyChanged(nameof(AttenuationResult));
            OnPropertyChanged(nameof(StopbandResult));

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
            double rollOff = 0;
            switch (results.FilterType)
            {
                case FilterType.LowPass:
                case FilterType.HighPass:
                    // Для фильтров первого порядка крутизна спада обычно составляет -20 дБ/декаду
                    rollOff = -20;
                    break;
                case FilterType.BandPass:
                case FilterType.BandStop:
                    // Для полосовых фильтров второго порядка крутизна спада обычно составляет -40 дБ/декаду
                    rollOff = -40;
                    break;
            }

            RollOffResult = $"Крутизна спада АЧХ: {rollOff} дБ/декаду";
        }

        private void UpdateUIWithResults(FilterResults results)
        {
            CutoffFrequencyResult = $"Частота среза: {Form.Frequency(results.CutoffFrequency)}";
            QualityFactorResult = $"Добротность (Q): {results.QualityFactor:F2}";
            BandwidthResult = $"Полоса пропускания: {Form.Frequency(results.Bandwidth)}";
            ImpedanceResult = $"Импеданс: {Form.Resistance(results.Impedance)}";
            PhaseShiftResult = $"Фазовый сдвиг: {results.PhaseShift:F2}°";
            GroupDelayResult = $"Групповая задержка: {Form.Time(results.GroupDelay)}";
            AttenuationResult = $"Затухание: {results.Attenuation:F2} дБ";

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
            if (field != value)
            {
                field = value;
                additionalAction?.Invoke();
                OnPropertyChanged();
            }
        }

        private void SetParameter(ref double field, double value)
        {
            if (SetProperty(ref field, value))
            {
                UpdateCanCalculate();
            }
        }
    }
}
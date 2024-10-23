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
        private readonly FiltersCalculationService _filtersCalculationService;
        private PlotModel _filterResponseModel;

        // Replace records with classes for mutable state
        private class FilterParameters
        {
            public double Capacitance { get; set; }
            public double Inductance { get; set; }
            public double Resistance { get; set; }
            public double Frequency { get; set; }
        }

        private class UnitParameters
        {
            public string CapacitanceUnit { get; set; }
            public string InductanceUnit { get; set; }
            public string ResistanceUnit { get; set; }
            public string FrequencyUnit { get; set; }
            public string FilterType { get; set; }
        }

        private (string CapacitanceUnit, string InductanceUnit, string ResistanceUnit, string FrequencyUnit, string FilterType) _selectedUnitsRecord;
        private (string CutoffFrequency, string QualityFactor, string Bandwidth, string Impedance, string PhaseShift, string GroupDelay, string Attenuation) _results;
        private bool _canCalculate;
        private readonly FilterParameters _parameters;
        private readonly UnitParameters _selectedUnits;

        public ICommand CalculateCommand { get; }
        public ObservableCollection<string> CapacitanceUnits { get; init; } = new(GetUnits("Capacitance"));
        public ObservableCollection<string> InductanceUnits { get; init; } = new(GetUnits("Inductance"));
        public ObservableCollection<string> ResistanceUnits { get; init; } = new(GetUnits("Resistance"));
        public ObservableCollection<string> FrequencyUnits { get; init; } = new(GetUnits("Frequency"));
        public ObservableCollection<string> FilterTypes { get; init; } = new() { "LowPass", "HighPass", "BandPass", "BandStop" };

        public bool CanCalculate { get => _canCalculate; set => SetProperty(ref _canCalculate, value); }

        public string SelectedCapacitanceUnit
        {
            get => _selectedUnits.CapacitanceUnit;
            set { _selectedUnits.CapacitanceUnit = value; OnPropertyChanged(); }
        }

        public string SelectedFilterType
        {
            get => _selectedUnits.FilterType;
            set { _selectedUnits.FilterType = value; OnPropertyChanged(); }
        }

        public string SelectedFrequencyUnit
        {
            get => _selectedUnits.FrequencyUnit;
            set { _selectedUnits.FrequencyUnit = value; OnPropertyChanged(); UpdateCanCalculate(); }
        }

        public string SelectedInductanceUnit
        {
            get => _selectedUnits.InductanceUnit;
            set { _selectedUnits.InductanceUnit = value; OnPropertyChanged(); }
        }

        public string SelectedResistanceUnit
        {
            get => _selectedUnits.ResistanceUnit;
            set { _selectedUnits.ResistanceUnit = value; OnPropertyChanged(); }
        }

        public double Capacitance
        {
            get => _parameters.Capacitance;
            set { _parameters.Capacitance = value; OnPropertyChanged(); UpdateCanCalculate(); }
        }

        public double Inductance
        {
            get => _parameters.Inductance;
            set { _parameters.Inductance = value; OnPropertyChanged(); UpdateCanCalculate(); }
        }

        public double Frequency
        {
            get => _parameters.Frequency;
            set { _parameters.Frequency = value; OnPropertyChanged(); UpdateCanCalculate(); }
        }

        public double Resistance
        {
            get => _parameters.Resistance;
            set { _parameters.Resistance = value; OnPropertyChanged(); UpdateCanCalculate(); }
        }

        public PlotModel FilterResponseModel { get => _filterResponseModel; set => SetProperty(ref _filterResponseModel, value); }
        public string CutoffFrequencyResult { get => _results.CutoffFrequency; set => SetProperty(ref _results.CutoffFrequency, value); }
        public string QualityFactorResult { get => _results.QualityFactor; set => SetProperty(ref _results.QualityFactor, value); }
        public string BandwidthResult { get => _results.Bandwidth; set => SetProperty(ref _results.Bandwidth, value); }
        public string ImpedanceResult { get => _results.Impedance; set => SetProperty(ref _results.Impedance, value); }
        public string PhaseShiftResult { get => _results.PhaseShift; set => SetProperty(ref _results.PhaseShift, value); }
        public string GroupDelayResult { get => _results.GroupDelay; set => SetProperty(ref _results.GroupDelay, value); }
        public string AttenuationResult { get => _results.Attenuation; set => SetProperty(ref _results.Attenuation, value); }
        private string _stopbandResult;
        public string StopbandResult { get => _stopbandResult; set => SetProperty(ref _stopbandResult, value); }
        private string _rollOffResult;
        public string RollOffResult { get => _rollOffResult; set => SetProperty(ref _rollOffResult, value); }
        public LRCFilterTab()
        {
            InitializeComponent();
            _filtersCalculationService = new FiltersCalculationService();

            _parameters = new FilterParameters
            {
                Capacitance = 1,
                Inductance = 1,
                Resistance = 1,
                Frequency = 1000
            };

            _selectedUnits = new UnitParameters
            {
                CapacitanceUnit = "uF",
                InductanceUnit = "μH",
                ResistanceUnit = "kΩ",
                FrequencyUnit = "kHz",
                FilterType = "LowPass"
            };

            InitializeDefaultValues();
            CalculateCommand = new RelayCommand(CalculateFilterParameters, () => CanCalculate);
            DataContext = this;
        }

        private void InitializeDefaultValues()
        {
            _selectedUnitsRecord = (
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

        private void UpdateCanCalculate() =>
            CanCalculate = (_parameters, _selectedUnitsRecord) switch
            {
                ({ } p, { } u) when InputsAreValid(p.Capacitance, p.Inductance, p.Resistance, p.Frequency)
                    && !string.IsNullOrEmpty(u.FilterType)
                    && !string.IsNullOrEmpty(u.CapacitanceUnit)
                    && !string.IsNullOrEmpty(u.InductanceUnit)
                    && !string.IsNullOrEmpty(u.ResistanceUnit)
                    && !string.IsNullOrEmpty(u.FrequencyUnit) => true,
                _ => false
            };

        private void CalculateStopband(FilterResults results)
        {
            double stopbandFrequency = results.FilterType switch
            {
                FilterType.LowPass => results.CutoffFrequency * 10,
                FilterType.HighPass => results.CutoffFrequency / 10,
                _ => throw new InvalidOperationException("Неподдерживаемый тип фильтра")
            };

            double stopbandAttenuation = _filtersCalculationService.CalculateAttenuation(
                results.FilterType, stopbandFrequency, results.CutoffFrequency, results.Bandwidth);

            StopbandResult = stopbandAttenuation > 0
                ? $"Полоса заграждения: частота {Formatter.Frequency(stopbandFrequency)}, затухание {stopbandAttenuation:F2} дБ"
                : "Расчет полосы заграждения не применим для полосовых фильтров";
        }

        private void CalculateFilterParameters()
        {
            try
            {
                SetLoadingOverlayVisibility(Visibility.Visible);

                var inputValues = new FilterInputValues
                {
                    FilterType = Enum.TryParse<FilterType>(_selectedUnits.FilterType, out var filterType) ? filterType : 
                    throw new InvalidOperationException("Некорректный тип фильтра"),
                    Capacitance = Convert(_parameters.Capacitance, _selectedUnitsRecord.CapacitanceUnit, "F", PhysicalQuantity.Capacitance),
                    Inductance = Convert(_parameters.Inductance, _selectedUnitsRecord.InductanceUnit, "H", PhysicalQuantity.Inductance),
                    Resistance = Convert(_parameters.Resistance, _selectedUnitsRecord.ResistanceUnit, "Ω", PhysicalQuantity.Resistance),
                    Frequency = Convert(_parameters.Frequency, _selectedUnitsRecord.FrequencyUnit, "Hz", PhysicalQuantity.Frequency)
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
            double rollOff = results.FilterType switch
            {
                FilterType.LowPass => -20,
                FilterType.HighPass => -20,
                _ => -40 // dB/decade для BandPass и BandStop
            };

            RollOffResult = $"Крутизна спада АЧХ: {rollOff} дБ/декаду";
        }

        private void UpdateUIWithResults(FilterResults results)
        {
            CutoffFrequencyResult = $"Частота среза: {Formatter.Frequency(results.CutoffFrequency)}";
            QualityFactorResult = $"Добротность (Q): {results.QualityFactor:F2}";
            BandwidthResult = $"Полоса пропускания: {Formatter.Frequency(results.Bandwidth)}";
            ImpedanceResult = $"Импеданс: {Formatter.Resistance(results.Impedance)}";
            PhaseShiftResult = $"Фазовый сдвиг: {results.PhaseShift:F2}°";
            GroupDelayResult = $"Групповая задержка: {Formatter.Time(results.GroupDelay)}";
            AttenuationResult = $"Затухание: {Formatter.Attenuation(results.Attenuation)}";

            if (results.StopbandFrequency > 0)
            {
                StopbandResult = $"Полоса заграждения: частота {Formatter.Frequency(results.StopbandFrequency)}, затухание {results.StopbandAttenuation:F2} дБ";
            }

            RollOffResult = $"Крутизна спада АЧХ: {results.RollOff} дБ/декаду";

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

        private void SetSelectedUnit(ref string field, string value, Action? additionalAction = null)
        {
            if (field != value)
            {
                field = value ?? throw new ArgumentNullException(nameof(value));
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
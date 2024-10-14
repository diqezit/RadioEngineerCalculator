using OxyPlot;
using OxyPlot.Series;
using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.FiltersCalculationService;

namespace RadioEngineerCalculator.ViewModel
{

    public partial class LRCFilterTab : UserControl, INotifyPropertyChanged
    {
        #region Properties
        private readonly FiltersCalculationService _filtersCalculationService;
        private PlotModel _filterResponseModel;
        private double _capacitance = 1;
        private double _inductance = 1;
        private double _resistance = 1;
        private double _frequency = 1000;
        private string _selectedCapacitanceUnit;
        private string _selectedFilterType;
        private string _selectedFrequencyUnit;
        private string _selectedInductanceUnit;
        private string _selectedResistanceUnit;
        private string _cutoffFrequencyResult;
        private string _qualityFactorResult;
        private bool _canCalculate = false;

        public ICommand CalculateCommand { get; }
        public ObservableCollection<string> CapacitanceUnits { get; set; }
        public ObservableCollection<string> InductanceUnits { get; set; }
        public ObservableCollection<string> ResistanceUnits { get; set; }
        public ObservableCollection<string> FrequencyUnits { get; set; }
        public ObservableCollection<string> FilterTypes { get; set; }
        #endregion

        #region Public Properties
        public bool CanCalculate
        {
            get => _canCalculate;
            set => SetProperty(ref _canCalculate, value);
        }

        public string SelectedCapacitanceUnit
        {
            get => _selectedCapacitanceUnit;
            set => SetProperty(ref _selectedCapacitanceUnit, value);
        }

        public string SelectedFilterType
        {
            get => _selectedFilterType;
            set => SetProperty(ref _selectedFilterType, value);
        }

        public string SelectedFrequencyUnit
        {
            get => _selectedFrequencyUnit;
            set => SetProperty(ref _selectedFrequencyUnit, value);
        }

        public string CutoffFrequencyResult
        {
            get => _cutoffFrequencyResult;
            set => SetProperty(ref _cutoffFrequencyResult, value);
        }

        public string QualityFactorResult
        {
            get => _qualityFactorResult;
            set => SetProperty(ref _qualityFactorResult, value);
        }

        public string SelectedInductanceUnit
        {
            get => _selectedInductanceUnit;
            set => SetProperty(ref _selectedInductanceUnit, value);
        }

        public string SelectedResistanceUnit
        {
            get => _selectedResistanceUnit;
            set => SetProperty(ref _selectedResistanceUnit, value);
        }

        public double Capacitance
        {
            get => _capacitance;
            set => SetProperty(ref _capacitance, value);
        }

        public double Inductance
        {
            get => _inductance;
            set => SetProperty(ref _inductance, value);
        }

        public double Frequency
        {
            get => _frequency;
            set => SetProperty(ref _frequency, value);
        }

        public double Resistance
        {
            get => _resistance;
            set => SetProperty(ref _resistance, value);
        }

        public PlotModel FilterResponseModel
        {
            get => _filterResponseModel;
            set => SetProperty(ref _filterResponseModel, value);
        }

        #endregion

        #region Constructor
        public LRCFilterTab()
        {
            InitializeComponent();
            _filtersCalculationService = new FiltersCalculationService();
            InitializeCollections();
            InitializeDefaultValues();
            CalculateCommand = new RelayCommand(CalculateFilterParameters, () => CanCalculate);
            DataContext = this;
        }
        #endregion

        #region Initialization Methods
        private void InitializeCollections()
        {
            CapacitanceUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Capacitance"));
            InductanceUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Inductance"));
            ResistanceUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Resistance"));
            FrequencyUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Frequency"));
            FilterTypes = new ObservableCollection<string> { "LowPass", "HighPass", "BandPass", "BandStop" };
        }

        private void InitializeDefaultValues()
        {
            SelectedCapacitanceUnit = CapacitanceUnits.FirstOrDefault();
            SelectedFilterType = FilterTypes.FirstOrDefault();
            SelectedFrequencyUnit = FrequencyUnits.FirstOrDefault();
            SelectedInductanceUnit = InductanceUnits.FirstOrDefault();
            SelectedResistanceUnit = ResistanceUnits.FirstOrDefault();
            FilterResponseModel = new PlotModel { Title = "Amplitude-Frequency Response" };
            UpdateCanCalculate();
        }

        #endregion

        #region Event Handlers
        private bool CanCalculateFilterParameters()
        {
            return !string.IsNullOrWhiteSpace(SelectedFilterType) &&
                   !string.IsNullOrWhiteSpace(SelectedCapacitanceUnit) &&
                   !string.IsNullOrWhiteSpace(SelectedInductanceUnit) &&
                   !string.IsNullOrWhiteSpace(SelectedResistanceUnit) &&
                   !string.IsNullOrWhiteSpace(SelectedFrequencyUnit);
        }

        private void OnParameterChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!double.TryParse(textBox.Text, out _))
                {
                    MessageBox.Show(ErrorMessages.InvalidInputFormat, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            UpdateCanCalculate();
        }


        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem == null)
            {
                MessageBox.Show(ErrorMessages.CheckComboBox, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            UpdateCanCalculate();
        }

        private void UpdateCanCalculate()
        {
            CanCalculate = Validate.InputsAreValid(Capacitance, Inductance, Resistance, Frequency) &&
                           !string.IsNullOrWhiteSpace(SelectedFilterType) &&
                           !string.IsNullOrWhiteSpace(SelectedCapacitanceUnit) &&
                           !string.IsNullOrWhiteSpace(SelectedInductanceUnit) &&
                           !string.IsNullOrWhiteSpace(SelectedResistanceUnit) &&
                           !string.IsNullOrWhiteSpace(SelectedFrequencyUnit);
        }

        #endregion

        #region Calculation Methods
        private void CalculateFilterParameters()
        {
            try
            {
                var inputValues = new FilterInputValues
                {
                    FilterType = (FilterType)Enum.Parse(typeof(FilterType), SelectedFilterType),
                    Capacitance = UnitC.Convert(Capacitance, SelectedCapacitanceUnit, "F", UnitC.PhysicalQuantity.Capacitance),
                    Inductance = UnitC.Convert(Inductance, SelectedInductanceUnit, "H", UnitC.PhysicalQuantity.Inductance),
                    Resistance = UnitC.Convert(Resistance, SelectedResistanceUnit, "Ω", UnitC.PhysicalQuantity.Resistance),
                    Frequency = UnitC.Convert(Frequency, SelectedFrequencyUnit, "Hz", UnitC.PhysicalQuantity.Frequency),
                };

                var results = _filtersCalculationService.CalculateFilterResults(inputValues);
                UpdateUIWithResults(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ErrorMessages.CalculationError}\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateUIWithResults(FilterResults results)
        {
            CutoffFrequencyResult = $"Частота среза: {UnitC.Convert(results.CutoffFrequency, "Hz", SelectedFrequencyUnit, UnitC.PhysicalQuantity.Frequency):F2} {SelectedFrequencyUnit}";
            QualityFactorResult = $"Добротность (Q): {results.QualityFactor:F2}";

            UpdateFilterResponsePlot(results);
        }

        private void UpdateFilterResponsePlot(FilterResults results)
        {
            var graph = new Graph(FilterResponseModel, _filtersCalculationService);
            graph.UpdateFilterResponsePlot(results);
            OnPropertyChanged(nameof(FilterResponseModel));
        }

        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
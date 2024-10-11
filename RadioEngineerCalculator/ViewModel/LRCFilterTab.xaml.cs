using OxyPlot;
using OxyPlot.Series;
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
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
    public partial class LRCFilterTab : UserControl, INotifyPropertyChanged
    {
        #region Properties
        public ICommand CalculateCommand { get; private set; }
        public ObservableCollection<string> CapacitanceUnits { get; set; }
        public ObservableCollection<string> InductanceUnits { get; set; }
        public ObservableCollection<string> ResistanceUnits { get; set; }
        public ObservableCollection<string> FrequencyUnits { get; set; }
        public ObservableCollection<string> FilterTypes { get; set; }

        private FiltersCalculationService _filtersCalculationService;
        private PlotModel _filterResponseModel;
        private double _capacitance;
        private double _inductance;
        private double _resistance;
        private double _frequency;
        private string _selectedCapacitanceUnit;
        private string _selectedFilterType;
        private string _selectedFrequencyUnit;
        private string _selectedInductanceUnit;
        private string _selectedResistanceUnit;
        #endregion

        #region Public Properties
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
            InitializeCollections();
            InitializeServices();
            InitializeDefaultValues();
            DataContext = this;
            CalculateCommand = new RelayCommand(CalculateFilterParameters);
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

        private void InitializeServices()
        {
            _filtersCalculationService = new FiltersCalculationService();
            _filterResponseModel = new PlotModel { Title = "Amplitude-Frequency Response" };
            FilterResponseModel = _filterResponseModel;
        }

        private void InitializeDefaultValues()
        {
            Capacitance = 1;
            Inductance = 1;
            Resistance = 1;
            Frequency = 1;
            SelectedCapacitanceUnit = CapacitanceUnits.FirstOrDefault();
            SelectedFilterType = FilterTypes.FirstOrDefault();
            SelectedFrequencyUnit = FrequencyUnits.FirstOrDefault();
            SelectedInductanceUnit = InductanceUnits.FirstOrDefault();
            SelectedResistanceUnit = ResistanceUnits.FirstOrDefault();

        }

        #endregion

        #region Event Handlers
        private void OnParameterChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFilterParameters();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a valid value.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            UpdateFilterParameters();
        }

        private void UpdateFilterParameters()
        {
            CalculateFilterParameters();
        }

        #endregion

        #region Calculation Methods
        private void CalculateFilterParameters()
        {
            if (string.IsNullOrEmpty(SelectedCapacitanceUnit) ||
                string.IsNullOrEmpty(SelectedFilterType) ||
                string.IsNullOrEmpty(SelectedFrequencyUnit) ||
                string.IsNullOrEmpty(SelectedInductanceUnit) ||
                string.IsNullOrEmpty(SelectedResistanceUnit))
            {
                MessageBox.Show("Please fill in all required fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var inputValues = new FilterInputValues
            {
                Capacitance = Capacitance,
                Inductance = Inductance,
                Resistance = Resistance,
                Frequency = Frequency,
                CapacitanceUnit = SelectedCapacitanceUnit,
                InductanceUnit = SelectedInductanceUnit,
                ResistanceUnit = SelectedResistanceUnit,
                FrequencyUnit = SelectedFrequencyUnit,
                FilterType = (FilterType)Enum.Parse(typeof(FilterType), SelectedFilterType)
            };

            if (!InputValidator.ValidateInputValues(inputValues))
            {
                return;
            }

            try
            {
                var results = _filtersCalculationService.CalculateFilterResults(inputValues);
                UpdateUIWithResults(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during calculation:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateUIWithResults(FilterResults results)
        {
            txtCutoffFrequencyResult.Text = $"Cutoff Frequency: {results.CutoffFrequency:F2} Hz";
            txtQualityFactorResult.Text = $"Quality Factor (Q): {results.QualityFactor:F2}";
            UpdateFilterResponsePlot(results);
        }

        private void UpdateFilterResponsePlot(FilterResults results)
        {
            _filterResponseModel.Series.Clear();
            var magnitudeSeries = new LineSeries { Title = "Magnitude" };
            var phaseSeries = new LineSeries { Title = "Phase" };

            double minFreq = results.CutoffFrequency / 10;
            double maxFreq = results.CutoffFrequency * 10;
            int points = 1000;

            for (int i = 0; i < points; i++)
            {
                double freq = minFreq * Math.Pow(maxFreq / minFreq, (double)i / (points - 1));
                (double magnitude, double phase) = CalculateResponse(freq, results);
                magnitudeSeries.Points.Add(new DataPoint(freq, 20 * Math.Log10(magnitude)));
                phaseSeries.Points.Add(new DataPoint(freq, phase * 180 / Math.PI));
            }

            _filterResponseModel.Series.Add(magnitudeSeries);
            _filterResponseModel.Series.Add(phaseSeries);
            _filterResponseModel.InvalidatePlot(true);
        }

        private (double magnitude, double phase) CalculateResponse(double freq, FilterResults results)
        {
            double magnitude = _filtersCalculationService.CalculateFilterMagnitudeResponse(results.FilterType, freq, results.CutoffFrequency, results.Bandwidth);
            double phase = _filtersCalculationService.CalculateFilterPhaseResponse(results.FilterType, freq, results.CutoffFrequency, results.Bandwidth);
            return (magnitude, phase);
        }

        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
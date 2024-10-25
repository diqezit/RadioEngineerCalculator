using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.UnitConverter;
using static RadioEngineerCalculator.Services.Validate;
using static RadioEngineerCalculator.Services.ComboBoxService;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class AntennaTab : UserControl, INotifyPropertyChanged
    {
        private readonly CalculationService _calculationService;
        private readonly Dictionary<string, ObservableCollection<string>> _unitCollections;

        private string _wavelengthResult;
        private string _vswrResult;
        private double _frequency;
        private double _forwardPower;
        private double _reflectedPower;
        private string _selectedFrequencyUnit;
        private string _selectedForwardPowerUnit;
        private string _selectedReflectedPowerUnit;
        private string _selectedWavelengthUnit;

        public AntennaTab()
        {
            InitializeComponent();
            DataContext = this;
            _calculationService = new CalculationService();
            _unitCollections = InitializeUnitCollections();
            InitializeCommands();
            InitializeDefaultUnits();
        }

        public ICommand CalculateWavelengthCommand { get; private set; }
        public ICommand CalculateVSWRCommand { get; private set; }

        public bool CanCalculateWavelength => InputsAreValid(Frequency) && !string.IsNullOrWhiteSpace(SelectedFrequencyUnit);
        public bool CanCalculateVSWR => InputsAreValid(ForwardPower, ReflectedPower) &&
                                        !string.IsNullOrWhiteSpace(SelectedForwardPowerUnit) &&
                                        !string.IsNullOrWhiteSpace(SelectedReflectedPowerUnit);

        public string WavelengthResult { get => _wavelengthResult; set => SetProperty(ref _wavelengthResult, value); }
        public string VSWRResult { get => _vswrResult; set => SetProperty(ref _vswrResult, value); }
        public double Frequency { get => _frequency; set { if (SetProperty(ref _frequency, value)) OnPropertyChanged(nameof(CanCalculateWavelength)); } }
        public double ForwardPower { get => _forwardPower; set { if (SetProperty(ref _forwardPower, value)) OnPropertyChanged(nameof(CanCalculateVSWR)); } }
        public double ReflectedPower { get => _reflectedPower; set { if (SetProperty(ref _reflectedPower, value)) OnPropertyChanged(nameof(CanCalculateVSWR)); } }
        public string SelectedFrequencyUnit { get => _selectedFrequencyUnit; set { if (SetProperty(ref _selectedFrequencyUnit, value)) { OnPropertyChanged(nameof(CanCalculateWavelength)); ConvertFrequency(); } } }
        public string SelectedForwardPowerUnit { get => _selectedForwardPowerUnit; set { if (SetProperty(ref _selectedForwardPowerUnit, value)) { OnPropertyChanged(nameof(CanCalculateVSWR)); ConvertForwardPower(); } } }
        public string SelectedReflectedPowerUnit { get => _selectedReflectedPowerUnit; set { if (SetProperty(ref _selectedReflectedPowerUnit, value)) { OnPropertyChanged(nameof(CanCalculateVSWR)); ConvertReflectedPower(); } } }
        public string SelectedWavelengthUnit { get => _selectedWavelengthUnit; set { if (SetProperty(ref _selectedWavelengthUnit, value)) UpdateWavelengthResult(); } }


        public ObservableCollection<string> FrequencyUnits => _unitCollections["Frequency"];
        public ObservableCollection<string> PowerUnits => _unitCollections["Power"];
        public ObservableCollection<string> LengthUnits => _unitCollections["Length"];

        private Dictionary<string, ObservableCollection<string>> InitializeUnitCollections()
        {
            return new Dictionary<string, ObservableCollection<string>>
            {
                ["Frequency"] = new ObservableCollection<string>(GetUnits("Frequency")),
                ["Power"] = new ObservableCollection<string>(GetUnits("Power")),
                ["Length"] = new ObservableCollection<string>(GetUnits("Length"))
            };
        }

        private void InitializeCommands()
        {
            CalculateWavelengthCommand = new RelayCommand(CalculateWavelength, () => CanCalculateWavelength);
            CalculateVSWRCommand = new RelayCommand(CalculateVSWR, () => CanCalculateVSWR);
        }

        private void InitializeDefaultUnits()
        {
            SelectedFrequencyUnit = FrequencyUnits[0];
            SelectedForwardPowerUnit = PowerUnits[0];
            SelectedReflectedPowerUnit = PowerUnits[0];
            SelectedWavelengthUnit = LengthUnits[0];
        }

        private void CalculateWavelength()
        {
            try
            {
                double frequencyHz = Convert(Frequency, SelectedFrequencyUnit, "Hz", PhysicalQuantity.Frequency);
                double wavelength = _calculationService.CalculateWavelength(frequencyHz);
                WavelengthResult = $"Длина волны: {Formatter.Format(wavelength, PhysicalQuantity.Length)}";
            }
            catch (Exception ex)
            {
                WavelengthResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculateVSWR()
        {
            try
            {
                double forwardPowerW = Convert(ForwardPower, SelectedForwardPowerUnit, "W", PhysicalQuantity.Power);
                double reflectedPowerW = Convert(ReflectedPower, SelectedReflectedPowerUnit, "W", PhysicalQuantity.Power);
                double vswr = _calculationService.CalculateVSWR(forwardPowerW, reflectedPowerW);
                VSWRResult = $"КСВН: {vswr:F2}";
            }
            catch (Exception ex)
            {
                VSWRResult = $"Ошибка: {ex.Message}";
            }
        }

        private void ConvertFrequency() => _frequency = Convert(Frequency, SelectedFrequencyUnit, "Hz", PhysicalQuantity.Frequency);
        private void ConvertForwardPower() => _forwardPower = Convert(ForwardPower, SelectedForwardPowerUnit, "W", PhysicalQuantity.Power);
        private void ConvertReflectedPower() => _reflectedPower = Convert(ReflectedPower, SelectedReflectedPowerUnit, "W", PhysicalQuantity.Power);

        private void UpdateWavelengthResult()
        {
            if (string.IsNullOrEmpty(WavelengthResult)) return;

            var parts = WavelengthResult.Split(':');
            if (parts.Length != 2) return;

            var valueParts = parts[1].Trim().Split(' ');
            if (valueParts.Length != 2 || !double.TryParse(valueParts[0], out double value)) return;

            double convertedWavelength = Convert(value, valueParts[1], SelectedWavelengthUnit, PhysicalQuantity.Length);
            WavelengthResult = $"Длина волны: {Formatter.Format(convertedWavelength, PhysicalQuantity.Length)}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
    }
}
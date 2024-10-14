using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class AntennaTab : UserControl, INotifyPropertyChanged
    {
        #region Fields
        private readonly CalculationService _calculationService;
        private string _wavelengthResult;
        private string _vswrResult;
        private bool _canCalculateWavelength = false;
        private bool _canCalculateVSWR = false;
        private double _frequency;
        private double _forwardPower;
        private double _reflectedPower;
        private string _selectedFrequencyUnit;
        private string _selectedForwardPowerUnit;
        private string _selectedReflectedPowerUnit;
        private string _selectedWavelengthUnit;
        #endregion

        #region Properties
        public ObservableCollection<string> FrequencyUnits { get; set; }
        public ObservableCollection<string> PowerUnits { get; set; }
        public ObservableCollection<string> LengthUnits { get; set; }

        public ICommand CalculateWavelengthCommand { get; private set; }
        public ICommand CalculateVSWRCommand { get; private set; }

        public bool CanCalculateWavelength
        {
            get => _canCalculateWavelength;
            set => SetProperty(ref _canCalculateWavelength, value);
        }

        public bool CanCalculateVSWR
        {
            get => _canCalculateVSWR;
            set => SetProperty(ref _canCalculateVSWR, value);
        }

        public string WavelengthResult
        {
            get => _wavelengthResult;
            set => SetProperty(ref _wavelengthResult, value);
        }

        public string VSWRResult
        {
            get => _vswrResult;
            set => SetProperty(ref _vswrResult, value);
        }

        public double Frequency
        {
            get => _frequency;
            set
            {
                if (SetProperty(ref _frequency, value))
                {
                    UpdateCanCalculateWavelength();
                }
            }
        }

        public double ForwardPower
        {
            get => _forwardPower;
            set
            {
                if (SetProperty(ref _forwardPower, value))
                {
                    UpdateCanCalculateVSWR();
                }
            }
        }

        public double ReflectedPower
        {
            get => _reflectedPower;
            set
            {
                if (SetProperty(ref _reflectedPower, value))
                {
                    UpdateCanCalculateVSWR();
                }
            }
        }

        public string SelectedFrequencyUnit
        {
            get => _selectedFrequencyUnit;
            set
            {
                if (SetProperty(ref _selectedFrequencyUnit, value))
                {
                    UpdateCanCalculateWavelength();
                    ConvertFrequency();
                }
            }
        }

        public string SelectedForwardPowerUnit
        {
            get => _selectedForwardPowerUnit;
            set
            {
                if (SetProperty(ref _selectedForwardPowerUnit, value))
                {
                    UpdateCanCalculateVSWR();
                    ConvertForwardPower();
                }
            }
        }

        public string SelectedReflectedPowerUnit
        {
            get => _selectedReflectedPowerUnit;
            set
            {
                if (SetProperty(ref _selectedReflectedPowerUnit, value))
                {
                    UpdateCanCalculateVSWR();
                    ConvertReflectedPower();
                }
            }
        }

        public string SelectedWavelengthUnit
        {
            get => _selectedWavelengthUnit;
            set
            {
                if (SetProperty(ref _selectedWavelengthUnit, value))
                {
                    UpdateWavelengthResult();
                }
            }
        }
        #endregion

        #region Constructor
        public AntennaTab()
        {
            InitializeComponent();
            DataContext = this;
            _calculationService = new CalculationService();
            InitializeCollections();
            InitializeCommands();
        }
        #endregion

        #region Initialization Methods
        private void InitializeCollections()
        {
            FrequencyUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Frequency"));
            PowerUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Power"));
            LengthUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Length"));

            SelectedFrequencyUnit = FrequencyUnits[0];
            SelectedForwardPowerUnit = PowerUnits[0];
            SelectedReflectedPowerUnit = PowerUnits[0];
            SelectedWavelengthUnit = LengthUnits[0];
        }

        private void InitializeCommands()
        {
            CalculateWavelengthCommand = new RelayCommand(CalculateWavelength, () => CanCalculateWavelength);
            CalculateVSWRCommand = new RelayCommand(CalculateVSWR, () => CanCalculateVSWR);
        }
        #endregion

        #region Validation and Update Methods
        private void UpdateCanCalculateWavelength()
        {
            CanCalculateWavelength = Validate.InputsAreValid(Frequency) && !string.IsNullOrWhiteSpace(SelectedFrequencyUnit);
        }

        private void UpdateCanCalculateVSWR()
        {
            CanCalculateVSWR = Validate.InputsAreValid(ForwardPower, ReflectedPower) &&
                                !string.IsNullOrWhiteSpace(SelectedForwardPowerUnit) &&
                                !string.IsNullOrWhiteSpace(SelectedReflectedPowerUnit);
        }
        #endregion

        #region Calculation Methods
        private void CalculateWavelength()
        {
            try
            {
                double frequencyHz = UnitC.Conv.Frequency(Frequency, SelectedFrequencyUnit, "Hz");
                double wavelengthMeters = _calculationService.CalculateWavelength(frequencyHz);
                WavelengthResult = $"Длина волны: {UnitC.Form.Length(wavelengthMeters)}";
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
                double forwardPowerW = UnitC.Conv.Power(ForwardPower, SelectedForwardPowerUnit, "W");
                double reflectedPowerW = UnitC.Conv.Power(ReflectedPower, SelectedReflectedPowerUnit, "W");
                double vswr = _calculationService.CalculateVSWR(forwardPowerW, reflectedPowerW);
                VSWRResult = $"КСВН: {vswr:F2}";
            }
            catch (Exception ex)
            {
                VSWRResult = $"Ошибка: {ex.Message}";
            }
        }
        #endregion

        #region Conversion Methods
        private void ConvertFrequency()
        {
            if (Validate.InputsAreValid(Frequency) && !string.IsNullOrWhiteSpace(SelectedFrequencyUnit))
            {
                Frequency = UnitC.Conv.Frequency(Frequency, "Hz", SelectedFrequencyUnit);
            }
        }

        private void ConvertForwardPower()
        {
            if (Validate.InputsAreValid(ForwardPower) && !string.IsNullOrWhiteSpace(SelectedForwardPowerUnit))
            {
                ForwardPower = UnitC.Conv.Power(ForwardPower, "W", SelectedForwardPowerUnit);
            }
        }

        private void ConvertReflectedPower()
        {
            if (Validate.InputsAreValid(ReflectedPower) && !string.IsNullOrWhiteSpace(SelectedReflectedPowerUnit))
            {
                ReflectedPower = UnitC.Conv.Power(ReflectedPower, "W", SelectedReflectedPowerUnit);
            }
        }

        private void UpdateWavelengthResult(double wavelengthMeters = 0)
        {
            if (wavelengthMeters != 0)
            {
                double convertedWavelength = UnitC.Conv.Length(wavelengthMeters, "m", SelectedWavelengthUnit);
                WavelengthResult = $"Длина волны: {convertedWavelength:F2} {SelectedWavelengthUnit}";
            }
            else if (!string.IsNullOrEmpty(WavelengthResult))
            {
                string[] parts = WavelengthResult.Split(':');
                if (parts.Length == 2)
                {
                    string[] valueParts = parts[1].Trim().Split(' ');
                    if (valueParts.Length == 2 && double.TryParse(valueParts[0], out double value))
                    {
                        double convertedWavelength = UnitC.Conv.Length(value, valueParts[1], SelectedWavelengthUnit);
                        WavelengthResult = $"Длина волны: {convertedWavelength:F2} {SelectedWavelengthUnit}";
                    }
                }
            }
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
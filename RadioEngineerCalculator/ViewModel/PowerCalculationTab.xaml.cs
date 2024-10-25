﻿using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.UnitConverter;
using static RadioEngineerCalculator.Services.Validate;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class PowerCalculationTab : UserControl, INotifyPropertyChanged
    {
        #region Private Fields
        private readonly CalculationService _calculationService;
        private double _current;
        private double _resistance;
        private double _voltage;
        private double _currentVI;
        private double _apparentPower;
        private double _realPower;
        private string _selectedCurrentUnit;
        private string _selectedResistanceUnit;
        private string _selectedVoltageUnit;
        private string _selectedCurrentVIUnit;
        private string _selectedApparentPowerUnit;
        private string _selectedRealPowerUnit;
        private string _powerResult;
        private string _powerResultVI;
        private string _powerFactorResult;
        private string _reactivePowerResult;
        #endregion

        #region Constructor and Initialization
        public PowerCalculationTab()
        {
            InitializeComponent();
            DataContext = this;
            _calculationService = new CalculationService();
            InitializeCommands();
            InitializeUnitCollections();
            InitializeDefaultUnits();
        }

        private void InitializeCommands()
        {
            CalculatePowerCommand = new RelayCommand(CalculatePower, () => CanCalculatePower);
            CalculatePowerVICommand = new RelayCommand(CalculatePowerVI, () => CanCalculatePowerVI);
            CalculatePowerFactorCommand = new RelayCommand(CalculatePowerFactor, () => CanCalculatePowerFactor);
            CalculateReactivePowerCommand = new RelayCommand(CalculateReactivePower, () => CanCalculateReactivePower);
        }

        private void InitializeUnitCollections()
        {
            CurrentUnits = new ObservableCollection<string>(
                UnitFactors.GetUnitFactors(PhysicalQuantity.Current).Select(u => u.Symbol));
            ResistanceUnits = new ObservableCollection<string>(
                UnitFactors.GetUnitFactors(PhysicalQuantity.Resistance).Select(u => u.Symbol));
            VoltageUnits = new ObservableCollection<string>(
                UnitFactors.GetUnitFactors(PhysicalQuantity.Voltage).Select(u => u.Symbol));
            PowerUnits = new ObservableCollection<string>(
                UnitFactors.GetUnitFactors(PhysicalQuantity.Power).Select(u => u.Symbol));
        }

        private void InitializeDefaultUnits()
        {
            // Используем базовые единицы из UnitConverter
            SelectedCurrentUnit = "A";
            SelectedResistanceUnit = "Ω";
            SelectedVoltageUnit = "V";
            SelectedCurrentVIUnit = "A";
            SelectedApparentPowerUnit = "VA";
            SelectedRealPowerUnit = "W";
        }
        #endregion

        #region Properties and Commands
        public ICommand CalculatePowerCommand { get; private set; }
        public ICommand CalculatePowerVICommand { get; private set; }
        public ICommand CalculatePowerFactorCommand { get; private set; }
        public ICommand CalculateReactivePowerCommand { get; private set; }

        public ObservableCollection<string> CurrentUnits { get; set; }
        public ObservableCollection<string> ResistanceUnits { get; set; }
        public ObservableCollection<string> VoltageUnits { get; set; }
        public ObservableCollection<string> PowerUnits { get; set; }

        // Value Properties with Validation
        public double Current
        {
            get => _current;
            set
            {
                if (SetProperty(ref _current, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePower));
                }
            }
        }

        public double Resistance
        {
            get => _resistance;
            set
            {
                if (SetProperty(ref _resistance, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePower));
                }
            }
        }

        public double Voltage
        {
            get => _voltage;
            set
            {
                if (SetProperty(ref _voltage, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePowerVI));
                }
            }
        }

        public double CurrentVI
        {
            get => _currentVI;
            set
            {
                if (SetProperty(ref _currentVI, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePowerVI));
                }
            }
        }

        public double ApparentPower
        {
            get => _apparentPower;
            set
            {
                if (SetProperty(ref _apparentPower, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePowerFactor));
                    OnPropertyChanged(nameof(CanCalculateReactivePower));
                }
            }
        }

        public double RealPower
        {
            get => _realPower;
            set
            {
                if (SetProperty(ref _realPower, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePowerFactor));
                    OnPropertyChanged(nameof(CanCalculateReactivePower));
                }
            }
        }

        // Unit Selection Properties
        public string SelectedCurrentUnit
        {
            get => _selectedCurrentUnit;
            set
            {
                if (SetProperty(ref _selectedCurrentUnit, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePower));
                    ConvertCurrent();
                }
            }
        }

        public string SelectedResistanceUnit
        {
            get => _selectedResistanceUnit;
            set
            {
                if (SetProperty(ref _selectedResistanceUnit, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePower));
                    ConvertResistance();
                }
            }
        }

        public string SelectedVoltageUnit
        {
            get => _selectedVoltageUnit;
            set
            {
                if (SetProperty(ref _selectedVoltageUnit, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePowerVI));
                    ConvertVoltage();
                }
            }
        }

        public string SelectedCurrentVIUnit
        {
            get => _selectedCurrentVIUnit;
            set
            {
                if (SetProperty(ref _selectedCurrentVIUnit, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePowerVI));
                    ConvertCurrentVI();
                }
            }
        }

        public string SelectedApparentPowerUnit
        {
            get => _selectedApparentPowerUnit;
            set
            {
                if (SetProperty(ref _selectedApparentPowerUnit, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePowerFactor));
                    OnPropertyChanged(nameof(CanCalculateReactivePower));
                    ConvertApparentPower();
                }
            }
        }

        public string SelectedRealPowerUnit
        {
            get => _selectedRealPowerUnit;
            set
            {
                if (SetProperty(ref _selectedRealPowerUnit, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePowerFactor));
                    OnPropertyChanged(nameof(CanCalculateReactivePower));
                    ConvertRealPower();
                }
            }
        }

        // Result Properties
        public string PowerResult { get => _powerResult; set => SetProperty(ref _powerResult, value); }
        public string PowerResultVI { get => _powerResultVI; set => SetProperty(ref _powerResultVI, value); }
        public string PowerFactorResult { get => _powerFactorResult; set => SetProperty(ref _powerFactorResult, value); }
        public string ReactivePowerResult { get => _reactivePowerResult; set => SetProperty(ref _reactivePowerResult, value); }
        #endregion

        #region Calculation Methods
        private void CalculatePower()
        {
            try
            {
                double currentA = UnitConverter.Convert(Current, SelectedCurrentUnit, "A", PhysicalQuantity.Current);
                double resistanceOhm = UnitConverter.Convert(Resistance, SelectedResistanceUnit, "Ω", PhysicalQuantity.Resistance);

                double result = _calculationService.CalculatePower(currentA, resistanceOhm);
                PowerResult = $"Мощность: {UnitConverter.Formatter.Format(result, PhysicalQuantity.Power)}";
            }
            catch (Exception ex)
            {
                PowerResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculatePowerVI()
        {
            try
            {
                double voltageV = UnitConverter.Convert(Voltage, SelectedVoltageUnit, "V", PhysicalQuantity.Voltage);
                double currentA = UnitConverter.Convert(CurrentVI, SelectedCurrentVIUnit, "A", PhysicalQuantity.Current);

                double result = _calculationService.CalculatePowerVI(voltageV, currentA);
                PowerResultVI = $"Мощность: {UnitConverter.Formatter.Format(result, PhysicalQuantity.Power)}";
            }
            catch (Exception ex)
            {
                PowerResultVI = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculatePowerFactor()
        {
            try
            {
                double apparentPowerVA = UnitConverter.Convert(ApparentPower, SelectedApparentPowerUnit, "VA", PhysicalQuantity.Power);
                double realPowerW = UnitConverter.Convert(RealPower, SelectedRealPowerUnit, "W", PhysicalQuantity.Power);

                double powerFactor = _calculationService.CalculatePowerFactor(realPowerW, apparentPowerVA);
                PowerFactorResult = $"Коэффициент мощности: {powerFactor:F4}";
            }
            catch (Exception ex)
            {
                PowerFactorResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculateReactivePower()
        {
            try
            {
                double apparentPowerVA = UnitConverter.Convert(ApparentPower, SelectedApparentPowerUnit, "VA", PhysicalQuantity.Power);
                double realPowerW = UnitConverter.Convert(RealPower, SelectedRealPowerUnit, "W", PhysicalQuantity.Power);

                double reactivePower = _calculationService.CalculateReactivePower(apparentPowerVA, realPowerW);
                ReactivePowerResult = $"Реактивная мощность: {UnitConverter.Formatter.Format(reactivePower, PhysicalQuantity.ReactivePower)}";
            }
            catch (Exception ex)
            {
                ReactivePowerResult = $"Ошибка: {ex.Message}";
            }
        }
        #endregion

        #region Unit Conversion Methods
        private void ConvertCurrent() =>
            ConvertUnit(ref _current, "A", SelectedCurrentUnit, PhysicalQuantity.Current);

        private void ConvertResistance() =>
            ConvertUnit(ref _resistance, "Ω", SelectedResistanceUnit, PhysicalQuantity.Resistance);

        private void ConvertVoltage() =>
            ConvertUnit(ref _voltage, "V", SelectedVoltageUnit, PhysicalQuantity.Voltage);

        private void ConvertCurrentVI() =>
            ConvertUnit(ref _currentVI, "A", SelectedCurrentVIUnit, PhysicalQuantity.Current);

        private void ConvertApparentPower() =>
            ConvertUnit(ref _apparentPower, "VA", SelectedApparentPowerUnit, PhysicalQuantity.Power);

        private void ConvertRealPower() =>
            ConvertUnit(ref _realPower, "W", SelectedRealPowerUnit, PhysicalQuantity.Power);

        private void ConvertUnit(ref double value, string fromUnit, string toUnit, PhysicalQuantity quantity)
        {
            if (InputsAreValid(value) && !string.IsNullOrWhiteSpace(toUnit))
            {
                value = UnitConverter.Convert(value, fromUnit, toUnit, quantity);
            }
        }
        #endregion

        #region Validation
        public bool CanCalculatePower =>
            InputsAreValid(Current, Resistance) &&
            !string.IsNullOrWhiteSpace(SelectedCurrentUnit) &&
            !string.IsNullOrWhiteSpace(SelectedResistanceUnit);

        public bool CanCalculatePowerVI =>
            InputsAreValid(Voltage, CurrentVI) &&
            !string.IsNullOrWhiteSpace(SelectedVoltageUnit) &&
            !string.IsNullOrWhiteSpace(SelectedCurrentVIUnit);

        public bool CanCalculatePowerFactor =>
            InputsAreValid(ApparentPower, RealPower) &&
            !string.IsNullOrWhiteSpace(SelectedApparentPowerUnit) &&
            !string.IsNullOrWhiteSpace(SelectedRealPowerUnit);

        public bool CanCalculateReactivePower =>
            InputsAreValid(ApparentPower, RealPower) &&
            !string.IsNullOrWhiteSpace(SelectedApparentPowerUnit) &&
            !string.IsNullOrWhiteSpace(SelectedRealPowerUnit);
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
        #endregion
    }
}
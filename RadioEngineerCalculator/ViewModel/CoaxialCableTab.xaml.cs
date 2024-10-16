using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.UnitC;
using static RadioEngineerCalculator.Services.Validate;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class CoaxialCableTab : UserControl, INotifyPropertyChanged
    {
        #region Приватные поля

        private readonly CalculationService _calculationService;
        private double _innerDiameter;
        private double _outerDiameter;
        private double _cableLength;
        private double _frequency;
        private double _dielectricConstant;
        private string _selectedInnerDiameterUnit;
        private string _selectedOuterDiameterUnit;
        private string _selectedCableLengthUnit;
        private string _selectedFrequencyUnit;
        private string _impedanceResult;
        private string _attenuationResult;
        private string _velocityFactorResult;
        private string _capacitanceResult;

        #endregion

        #region Публичные свойства

        public ObservableCollection<string> LengthUnits { get; }
        public ObservableCollection<string> FrequencyUnits { get; }

        public double InnerDiameter
        {
            get => _innerDiameter;
            set
            {
                if (SetProperty(ref _innerDiameter, value))
                {
                    OnPropertyChanged(nameof(CanCalculate));
                }
            }
        }

        public double OuterDiameter
        {
            get => _outerDiameter;
            set
            {
                if (SetProperty(ref _outerDiameter, value))
                {
                    OnPropertyChanged(nameof(CanCalculate));
                }
            }
        }

        public double CableLength
        {
            get => _cableLength;
            set
            {
                if (SetProperty(ref _cableLength, value))
                {
                    OnPropertyChanged(nameof(CanCalculate));
                }
            }
        }

        public double Frequency
        {
            get => _frequency;
            set
            {
                if (SetProperty(ref _frequency, value))
                {
                    OnPropertyChanged(nameof(CanCalculate));
                }
            }
        }

        public double DielectricConstant
        {
            get => _dielectricConstant;
            set
            {
                if (SetProperty(ref _dielectricConstant, value))
                {
                    OnPropertyChanged(nameof(CanCalculate));
                }
            }
        }

        public string SelectedInnerDiameterUnit
        {
            get => _selectedInnerDiameterUnit;
            set
            {
                if (SetProperty(ref _selectedInnerDiameterUnit, value))
                {
                    ConvertInnerDiameter();
                }
            }
        }

        public string SelectedOuterDiameterUnit
        {
            get => _selectedOuterDiameterUnit;
            set
            {
                if (SetProperty(ref _selectedOuterDiameterUnit, value))
                {
                    ConvertOuterDiameter();
                }
            }
        }

        public string SelectedCableLengthUnit
        {
            get => _selectedCableLengthUnit;
            set
            {
                if (SetProperty(ref _selectedCableLengthUnit, value))
                {
                    ConvertCableLength();
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
                    ConvertFrequency();
                }
            }
        }

        public string ImpedanceResult
        {
            get => _impedanceResult;
            set => SetProperty(ref _impedanceResult, value);
        }

        public string AttenuationResult
        {
            get => _attenuationResult;
            set => SetProperty(ref _attenuationResult, value);
        }

        public string VelocityFactorResult
        {
            get => _velocityFactorResult;
            set => SetProperty(ref _velocityFactorResult, value);
        }

        public string CapacitanceResult
        {
            get => _capacitanceResult;
            set => SetProperty(ref _capacitanceResult, value);
        }

        public bool CanCalculate =>
            InputsAreValid(InnerDiameter, OuterDiameter, CableLength, Frequency, DielectricConstant) &&
            !string.IsNullOrWhiteSpace(SelectedInnerDiameterUnit) &&
            !string.IsNullOrWhiteSpace(SelectedOuterDiameterUnit) &&
            !string.IsNullOrWhiteSpace(SelectedCableLengthUnit) &&
            !string.IsNullOrWhiteSpace(SelectedFrequencyUnit);

        #endregion

        #region Команды

        public ICommand CalculateCommand { get; }

        #endregion

        #region Конструктор

        public CoaxialCableTab()
        {
            InitializeComponent();
            DataContext = this;
            _calculationService = new CalculationService();
            LengthUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Length"));
            FrequencyUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Frequency"));
            CalculateCommand = new RelayCommand(Calculate, () => CanCalculate);
            InitializeDefaultUnits();
        }

        #endregion

        #region Приватные методы

        private void InitializeDefaultUnits()
        {
            SelectedInnerDiameterUnit = LengthUnits[0];
            SelectedOuterDiameterUnit = LengthUnits[0];
            SelectedCableLengthUnit = LengthUnits[0];
            SelectedFrequencyUnit = FrequencyUnits[0];
        }

        private void Calculate()
        {
            try
            {
                double innerDiameterM = Convert(InnerDiameter, SelectedInnerDiameterUnit, "m", PhysicalQuantity.Length);
                double outerDiameterM = Convert(OuterDiameter, SelectedOuterDiameterUnit, "m", PhysicalQuantity.Length);
                double cableLengthM = Convert(CableLength, SelectedCableLengthUnit, "m", PhysicalQuantity.Length);
                double frequencyHz = Convert(Frequency, SelectedFrequencyUnit, "Hz", PhysicalQuantity.Frequency);

                double impedance = _calculationService.CalculateCoaxialCableImpedance(innerDiameterM, outerDiameterM, DielectricConstant);
                double attenuation = _calculationService.CalculateCoaxialCableAttenuation(innerDiameterM, outerDiameterM, frequencyHz, DielectricConstant, cableLengthM);
                double velocityFactor = _calculationService.CalculateVelocityFactor(DielectricConstant);
                double capacitance = _calculationService.CalculateCoaxialCableCapacitance(innerDiameterM, outerDiameterM, DielectricConstant, cableLengthM);

                ImpedanceResult = $"Волновое сопротивление: {FormatResult(impedance, PhysicalQuantity.Resistance)}";
                AttenuationResult = $"Затухание: {FormatResult(attenuation, PhysicalQuantity.Attenuation)}";
                VelocityFactorResult = $"Коэффициент скорости: {velocityFactor:F2}";
                CapacitanceResult = $"Емкость: {FormatResult(capacitance, PhysicalQuantity.Capacitance)}";
            }
            catch (Exception ex)
            {
                ImpedanceResult = $"Ошибка: {ex.Message}";
                AttenuationResult = string.Empty;
                VelocityFactorResult = string.Empty;
                CapacitanceResult = string.Empty;
            }
        }

        private void ConvertInnerDiameter() => ConvertUnit(ref _innerDiameter, "m", SelectedInnerDiameterUnit, PhysicalQuantity.Length);
        private void ConvertOuterDiameter() => ConvertUnit(ref _outerDiameter, "m", SelectedOuterDiameterUnit, PhysicalQuantity.Length);
        private void ConvertCableLength() => ConvertUnit(ref _cableLength, "m", SelectedCableLengthUnit, PhysicalQuantity.Length);
        private void ConvertFrequency() => ConvertUnit(ref _frequency, "Hz", SelectedFrequencyUnit, PhysicalQuantity.Frequency);

        private void ConvertUnit(ref double value, string fromUnit, string toUnit, PhysicalQuantity quantity)
        {
            if (InputsAreValid(value) && !string.IsNullOrWhiteSpace(toUnit))
            {
                value = Convert(value, fromUnit, toUnit, quantity);
                OnPropertyChanged(nameof(CanCalculate));
            }
        }

        private string FormatResult(double value, PhysicalQuantity quantity) => AutoFormat(value, quantity);

        #endregion

        #region Реализация INotifyPropertyChanged

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
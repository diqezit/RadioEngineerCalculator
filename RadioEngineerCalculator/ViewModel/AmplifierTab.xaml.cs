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
using static RadioEngineerCalculator.Infos.ErrorMessages;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class AmplifierTab : UserControl, INotifyPropertyChanged
    {
        #region Fields
        private readonly CalculationService _calculationService;
        private readonly Dictionary<string, ObservableCollection<string>> _unitCollections;

        private double _powerIn;
        private double _powerOut;
        private double _noiseFactor;
        private double _outputPower;
        private double _inputDCPower;
        private double _inputPower;
        private double _outputPowerDBm;
        private double _smallSignalGain;
        private double _fundamentalPower;
        private double _thirdOrderPower;

        private string _selectedPowerInUnit;
        private string _selectedPowerOutUnit;

        private string _gainResult;
        private string _noiseFigureResult;
        private string _efficiencyResult;
        private string _compressionPointResult;
        private string _ip3Result;
        #endregion

        #region Constructor and Initialization
        public AmplifierTab()
        {
            InitializeComponent();
            DataContext = this;
            _calculationService = new CalculationService();
            _unitCollections = InitializeUnitCollections();
            InitializeCommands();
            InitializeDefaultUnits();
        }

        private Dictionary<string, ObservableCollection<string>> InitializeUnitCollections()
        {
            return new Dictionary<string, ObservableCollection<string>>
            {
                ["Power"] = new ObservableCollection<string>(ComboBoxService.GetUnits("Power"))
            };
        }

        private void InitializeCommands()
        {
            CalculateGainCommand = new RelayCommand(CalculateGain, () => CanCalculateGain);
            CalculateNoiseFigureCommand = new RelayCommand(CalculateNoiseFigure, () => CanCalculateNoiseFigure);
            CalculateEfficiencyCommand = new RelayCommand(CalculateEfficiency, () => CanCalculateEfficiency);
            Calculate1dBCompressionPointCommand = new RelayCommand(Calculate1dBCompressionPoint, () => CanCalculate1dBCompressionPoint);
            CalculateIP3Command = new RelayCommand(CalculateIP3, () => CanCalculateIP3);
        }

        private void InitializeDefaultUnits()
        {
            SelectedPowerInUnit = PowerUnits[0];
            SelectedPowerOutUnit = PowerUnits[0];
        }
        #endregion

        #region Properties
        public ICommand CalculateGainCommand { get; private set; }
        public ICommand CalculateNoiseFigureCommand { get; private set; }
        public ICommand CalculateEfficiencyCommand { get; private set; }
        public ICommand Calculate1dBCompressionPointCommand { get; private set; }
        public ICommand CalculateIP3Command { get; private set; }

        public ObservableCollection<string> PowerUnits => _unitCollections["Power"];

        public double PowerIn { get => _powerIn; set { if (SetProperty(ref _powerIn, value)) OnPropertyChanged(nameof(CanCalculateGain)); } }
        public double PowerOut { get => _powerOut; set { if (SetProperty(ref _powerOut, value)) OnPropertyChanged(nameof(CanCalculateGain)); } }
        public string SelectedPowerInUnit { get => _selectedPowerInUnit; set { if (SetProperty(ref _selectedPowerInUnit, value)) { OnPropertyChanged(nameof(CanCalculateGain)); ConvertPowerIn(); } } }
        public string SelectedPowerOutUnit { get => _selectedPowerOutUnit; set { if (SetProperty(ref _selectedPowerOutUnit, value)) { OnPropertyChanged(nameof(CanCalculateGain)); ConvertPowerOut(); } } }
        public double NoiseFactor { get => _noiseFactor; set { if (SetProperty(ref _noiseFactor, value)) OnPropertyChanged(nameof(CanCalculateNoiseFigure)); } }
        public double OutputPower { get => _outputPower; set { if (SetProperty(ref _outputPower, value)) OnPropertyChanged(nameof(CanCalculateEfficiency)); } }
        public double InputDCPower { get => _inputDCPower; set { if (SetProperty(ref _inputDCPower, value)) OnPropertyChanged(nameof(CanCalculateEfficiency)); } }
        public double InputPower { get => _inputPower; set { if (SetProperty(ref _inputPower, value)) OnPropertyChanged(nameof(CanCalculate1dBCompressionPoint)); } }
        public double OutputPowerDBm { get => _outputPowerDBm; set { if (SetProperty(ref _outputPowerDBm, value)) OnPropertyChanged(nameof(CanCalculate1dBCompressionPoint)); } }
        public double SmallSignalGain { get => _smallSignalGain; set { if (SetProperty(ref _smallSignalGain, value)) OnPropertyChanged(nameof(CanCalculate1dBCompressionPoint)); } }
        public double FundamentalPower { get => _fundamentalPower; set { if (SetProperty(ref _fundamentalPower, value)) OnPropertyChanged(nameof(CanCalculateIP3)); } }
        public double ThirdOrderPower { get => _thirdOrderPower; set { if (SetProperty(ref _thirdOrderPower, value)) OnPropertyChanged(nameof(CanCalculateIP3)); } }
        public string GainResult { get => _gainResult; set => SetProperty(ref _gainResult, value); }
        public string NoiseFigureResult { get => _noiseFigureResult; set => SetProperty(ref _noiseFigureResult, value); }
        public string EfficiencyResult { get => _efficiencyResult; set => SetProperty(ref _efficiencyResult, value); }
        public string CompressionPointResult { get => _compressionPointResult; set => SetProperty(ref _compressionPointResult, value); }
        public string IP3Result { get => _ip3Result; set => SetProperty(ref _ip3Result, value); }

        #endregion

        #region Calculation Methods
        private void CalculateGain()
        {
            try
            {
                double powerInW = Convert(PowerIn, SelectedPowerInUnit, "W", PhysicalQuantity.Power);
                double powerOutW = Convert(PowerOut, SelectedPowerOutUnit, "W", PhysicalQuantity.Power);
                double gain = _calculationService.CalculateGain(powerInW, powerOutW);
                GainResult = $"Усиление: {Formatter.Decibels(gain)} dB";
            }
            catch (Exception ex)
            {
                GainResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculateNoiseFigure()
        {
            try
            {
                double noiseFigure = _calculationService.CalculateNoiseFigure(NoiseFactor);
                NoiseFigureResult = $"Коэффициент шума: {Formatter.Decibels(noiseFigure)} dB";
            }
            catch (Exception ex)
            {
                NoiseFigureResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculateEfficiency()
        {
            try
            {
                double efficiency = _calculationService.CalculateAmplifierEfficiency(OutputPower, InputDCPower);
                EfficiencyResult = $"КПД: {efficiency:F2}%";
            }
            catch (Exception ex)
            {
                EfficiencyResult = $"Ошибка: {ex.Message}";
            }
        }

        private void Calculate1dBCompressionPoint()
        {
            try
            {
                double compressionPoint = _calculationService.Calculate1dBCompressionPoint(InputPower, OutputPowerDBm, SmallSignalGain);
                CompressionPointResult = $"Точка компрессии 1 dB: {Formatter.Format(compressionPoint, PhysicalQuantity.Power)} dBm";
            }
            catch (Exception ex)
            {
                CompressionPointResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculateIP3()
        {
            try
            {
                double ip3 = _calculationService.CalculateIP3(FundamentalPower, ThirdOrderPower);
                IP3Result = $"Точка пересечения третьего порядка (IP3): {Formatter.Format(ip3, PhysicalQuantity.Power)} dBm";
            }
            catch (Exception ex)
            {
                IP3Result = $"Ошибка: {ex.Message}";
            }
        }
        #endregion

        #region Can Calculate Properties
        public bool CanCalculateGain => InputsAreValid(PowerIn, PowerOut) &&
                                        !string.IsNullOrWhiteSpace(SelectedPowerInUnit) &&
                                        !string.IsNullOrWhiteSpace(SelectedPowerOutUnit);

        public bool CanCalculateNoiseFigure => InputsAreValid(NoiseFactor);

        public bool CanCalculateEfficiency => InputsAreValid(OutputPower, InputDCPower);

        public bool CanCalculate1dBCompressionPoint => InputsAreValid(InputPower, OutputPowerDBm, SmallSignalGain);

        public bool CanCalculateIP3 => InputsAreValid(FundamentalPower, ThirdOrderPower);
        #endregion

        #region Unit Conversion Methods
        private void ConvertPowerIn() => _powerIn = Convert(PowerIn, SelectedPowerInUnit, "W", PhysicalQuantity.Power);
        private void ConvertPowerOut() => _powerOut = Convert(PowerOut, SelectedPowerOutUnit, "W", PhysicalQuantity.Power);
        #endregion

        #region INotifyPropertyChanged Implementation
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
        #endregion
    }
}
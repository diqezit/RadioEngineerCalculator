using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
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
    public enum OpAmpType
    {
        GeneralPurpose,
        Precision,
        HighSpeed,
        LowNoise,
        RailToRail,
        Instrumentation,
        LowPower
    }

    public enum OperatingMode
    {
        Inverting,
        NonInverting,
        Differential,
        Integrator,
        Differentiator
    }

    public record OpAmpSpecs
    {
        public double InputOffsetVoltage { get; init; }  // мкВ
        public double InputBiasCurrentTypical { get; init; }  // нА
        public double InputOffsetCurrentMax { get; init; }  // нА
        public double UnityGainBandwidth { get; init; }  // МГц
        public double SlewRateTypical { get; init; }  // В/мкс
        public double CMRRTypical { get; init; }  // дБ
        public double PSRRTypical { get; init; }  // дБ
        public double OutputCurrentMax { get; init; }  // мА
        public double QuiescentCurrent { get; init; }  // мА
        public double CommonModeInputRange { get; init; }  // В
        public double PowerConsumptionMw { get; init; }  // мВт
    }

    public partial class OpAmpTab : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private readonly OpAmpCalculationService _calculationService;
        private readonly Dictionary<OpAmpType, OpAmpSpecs> _opAmpSpecs;

        private OpAmpType _selectedOpAmpType = OpAmpType.GeneralPurpose;
        private OperatingMode _selectedOperatingMode = OperatingMode.NonInverting;

        private readonly (double InputVoltage, double Gain, double SupplyVoltage,
                         double InputResistor, double FeedbackResistor, double LoadResistor)
            _defaultParameters = (1, 10, 12, 1000, 10000, 1000);

        private double _inputVoltage;
        private double _gain;
        private double _supplyVoltage;
        private double _inputResistor;
        private double _feedbackResistor;
        private double _loadResistor;
        private double _frequency;
        private double _temperature = 25.0; // Default temperature in Celsius

        private string _selectedVoltageUnit;
        private string _selectedSupplyVoltageUnit;
        private string _selectedResistorUnit;
        private string _selectedFrequencyUnit;

        private string _outputVoltageResult;
        private string _powerDissipationResult;
        private string _slewRateResult;
        private string _bandwidthResult;
        private string _inputImpedanceResult;
        private string _outputImpedanceResult;
        private string _cmrrResult;
        private string _noiseResult;
        private string _thdResult;
        private string _temperatureEffectsResult;

        #endregion

        #region Properties

        public OpAmpType SelectedOpAmpType
        {
            get => _selectedOpAmpType;
            set
            {
                if (SetProperty(ref _selectedOpAmpType, value))
                {
                    UpdateCalculations();
                }
            }
        }

        public OperatingMode SelectedOperatingMode
        {
            get => _selectedOperatingMode;
            set
            {
                if (SetProperty(ref _selectedOperatingMode, value))
                {
                    UpdateCalculations();
                }
            }
        }

        public double Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        public double InputVoltage
        {
            get => _inputVoltage;
            set => SetProperty(ref _inputVoltage, value);
        }

        public double Gain
        {
            get => _gain;
            set => SetProperty(ref _gain, value);
        }

        public double SupplyVoltage
        {
            get => _supplyVoltage;
            set => SetProperty(ref _supplyVoltage, value);
        }

        public double InputResistor
        {
            get => _inputResistor;
            set => SetProperty(ref _inputResistor, value);
        }

        public double FeedbackResistor
        {
            get => _feedbackResistor;
            set => SetProperty(ref _feedbackResistor, value);
        }

        public double LoadResistor
        {
            get => _loadResistor;
            set => SetProperty(ref _loadResistor, value);
        }

        public double Frequency
        {
            get => _frequency;
            set => SetProperty(ref _frequency, value);
        }

        public string SelectedVoltageUnit
        {
            get => _selectedVoltageUnit;
            set
            {
                if (SetProperty(ref _selectedVoltageUnit, value))
                {
                    ConvertAndUpdateVoltage(ref _inputVoltage, value);
                }
            }
        }

        public string SelectedSupplyVoltageUnit
        {
            get => _selectedSupplyVoltageUnit;
            set
            {
                if (SetProperty(ref _selectedSupplyVoltageUnit, value))
                {
                    ConvertAndUpdateVoltage(ref _supplyVoltage, value);
                }
            }
        }

        public string SelectedResistorUnit
        {
            get => _selectedResistorUnit;
            set
            {
                if (SetProperty(ref _selectedResistorUnit, value))
                {
                    ConvertAndUpdateResistor(ref _inputResistor, value);
                    ConvertAndUpdateResistor(ref _feedbackResistor, value);
                    ConvertAndUpdateResistor(ref _loadResistor, value);
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
                    ConvertAndUpdateFrequency(ref _frequency, value);
                }
            }
        }

        public string OutputVoltageResult
        {
            get => _outputVoltageResult;
            set => SetProperty(ref _outputVoltageResult, value);
        }

        public string PowerDissipationResult
        {
            get => _powerDissipationResult;
            set => SetProperty(ref _powerDissipationResult, value);
        }

        public string SlewRateResult
        {
            get => _slewRateResult;
            set => SetProperty(ref _slewRateResult, value);
        }

        public string BandwidthResult
        {
            get => _bandwidthResult;
            set => SetProperty(ref _bandwidthResult, value);
        }

        public string InputImpedanceResult
        {
            get => _inputImpedanceResult;
            set => SetProperty(ref _inputImpedanceResult, value);
        }

        public string OutputImpedanceResult
        {
            get => _outputImpedanceResult;
            set => SetProperty(ref _outputImpedanceResult, value);
        }

        public string CMRRResult
        {
            get => _cmrrResult;
            set => SetProperty(ref _cmrrResult, value);
        }

        public string NoiseResult
        {
            get => _noiseResult;
            set => SetProperty(ref _noiseResult, value);
        }

        public string THDResult
        {
            get => _thdResult;
            set => SetProperty(ref _thdResult, value);
        }

        public string TemperatureEffectsResult
        {
            get => _temperatureEffectsResult;
            set => SetProperty(ref _temperatureEffectsResult, value);
        }

        public IEnumerable<OpAmpType> AvailableOpAmpTypes =>
            Enum.GetValues(typeof(OpAmpType)).Cast<OpAmpType>();

        public IEnumerable<OperatingMode> AvailableOperatingModes =>
            Enum.GetValues(typeof(OperatingMode)).Cast<OperatingMode>();

        #endregion

        #region Constructor and Initialization

        public OpAmpTab()
        {
            InitializeComponent();
            DataContext = this;

            _opAmpSpecs = InitializeOpAmpSpecs();
            _calculationService = new OpAmpCalculationService(_selectedOpAmpType, _opAmpSpecs);

            // Initialize collections
            VoltageUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Voltage"));
            ResistorUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Resistance"));
            FrequencyUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Frequency"));

            InitializeCommands();
            InitializeDefaultValues();
        }

        public ObservableCollection<string> VoltageUnits { get; }
        public ObservableCollection<string> ResistorUnits { get; }
        public ObservableCollection<string> FrequencyUnits { get; }

        public bool CanCalculate =>
            InputVoltage > 0 &&
            Gain > 0 &&
            SupplyVoltage > 0 &&
            InputResistor > 0 &&
            FeedbackResistor > 0 &&
            LoadResistor > 0 &&
            Frequency > 0;

        public ICommand CalculateCommand { get; private set; }

        private Dictionary<OpAmpType, OpAmpSpecs> InitializeOpAmpSpecs()
        {
            return new Dictionary<OpAmpType, OpAmpSpecs>
            {
                [OpAmpType.GeneralPurpose] = new OpAmpSpecs
                {
                    InputOffsetVoltage = 2000,
                    InputBiasCurrentTypical = 80,
                    InputOffsetCurrentMax = 20,
                    UnityGainBandwidth = 1,
                    SlewRateTypical = 0.5,
                    CMRRTypical = 85,
                    PSRRTypical = 85,
                    OutputCurrentMax = 20,
                    QuiescentCurrent = 1.5,
                    CommonModeInputRange = 12,
                    PowerConsumptionMw = 50
                },
                [OpAmpType.Precision] = new OpAmpSpecs
                {
                    InputOffsetVoltage = 25,
                    InputBiasCurrentTypical = 0.5,
                    InputOffsetCurrentMax = 0.2,
                    UnityGainBandwidth = 2,
                    SlewRateTypical = 2,
                    CMRRTypical = 120,
                    PSRRTypical = 120,
                    OutputCurrentMax = 30,
                    QuiescentCurrent = 3,
                    CommonModeInputRange = 14,
                    PowerConsumptionMw = 100
                },
                [OpAmpType.HighSpeed] = new OpAmpSpecs
                {
                    InputOffsetVoltage = 1000,
                    InputBiasCurrentTypical = 500,
                    InputOffsetCurrentMax = 100,
                    UnityGainBandwidth = 50,
                    SlewRateTypical = 250,
                    CMRRTypical = 80,
                    PSRRTypical = 80,
                    OutputCurrentMax = 50,
                    QuiescentCurrent = 8,
                    CommonModeInputRange = 10,
                    PowerConsumptionMw = 200
                },
                [OpAmpType.LowNoise] = new OpAmpSpecs
                {
                    InputOffsetVoltage = 50,
                    InputBiasCurrentTypical = 1,
                    InputOffsetCurrentMax = 0.5,
                    UnityGainBandwidth = 3,
                    SlewRateTypical = 1,
                    CMRRTypical = 110,
                    PSRRTypical = 110,
                    OutputCurrentMax = 25,
                    QuiescentCurrent = 2.5,
                    CommonModeInputRange = 13,
                    PowerConsumptionMw = 75
                },
                [OpAmpType.RailToRail] = new OpAmpSpecs
                {
                    InputOffsetVoltage = 100,
                    InputBiasCurrentTypical = 20,
                    InputOffsetCurrentMax = 10,
                    UnityGainBandwidth = 5,
                    SlewRateTypical = 10,
                    CMRRTypical = 90,
                    PSRRTypical = 90,
                    OutputCurrentMax = 35,
                    QuiescentCurrent = 5,
                    CommonModeInputRange = 15,
                    PowerConsumptionMw = 120
                },
                [OpAmpType.Instrumentation] = new OpAmpSpecs
                {
                    InputOffsetVoltage = 10,
                    InputBiasCurrentTypical = 0.2,
                    InputOffsetCurrentMax = 0.1,
                    UnityGainBandwidth = 1,
                    SlewRateTypical = 0.2,
                    CMRRTypical = 130,
                    PSRRTypical = 130,
                    OutputCurrentMax = 10,
                    QuiescentCurrent = 1,
                    CommonModeInputRange = 10,
                    PowerConsumptionMw = 30
                },
                [OpAmpType.LowPower] = new OpAmpSpecs
                {
                    InputOffsetVoltage = 150,
                    InputBiasCurrentTypical = 5,
                    InputOffsetCurrentMax = 2,
                    UnityGainBandwidth = 0.5,
                    SlewRateTypical = 0.05,
                    CMRRTypical = 75,
                    PSRRTypical = 75,
                    OutputCurrentMax = 5,
                    QuiescentCurrent = 0.5,
                    CommonModeInputRange = 12,
                    PowerConsumptionMw = 10
                }
            };
        }

        private void InitializeCommands()
        {
            CalculateCommand = new RelayCommand(Calculate, () => CanCalculate);
        }

        private void InitializeDefaultValues()
        {
            (_inputVoltage, _gain, _supplyVoltage, _inputResistor, _feedbackResistor, _loadResistor) = _defaultParameters;
            _frequency = 1000; // Default 1 kHz

            SelectedVoltageUnit = VoltageUnits[0];
            SelectedSupplyVoltageUnit = VoltageUnits[0];
            SelectedResistorUnit = ResistorUnits[0];
            SelectedFrequencyUnit = FrequencyUnits[0];
        }

        #endregion

        #region Calculation Methods
        private void UpdateCalculations()
        {
            if (CanCalculate)
            {
                try
                {
                    Calculate();
                }
                catch (Exception ex)
                {
                    HandleCalculationError(ex);
                }
            }
        }

        private void Calculate()
        {
            try
            {
                ValidateInputs();
                PerformBasicCalculations();
                PerformAdvancedCalculations();
                PerformTemperatureCalculations();
                PerformNoiseAnalysis();
            }
            catch (Exception ex)
            {
                HandleCalculationError(ex);
            }
        }

        private void ValidateInputs()
        {
            if (InputVoltage <= 0) throw new ArgumentException("Входное напряжение должно быть больше 0");
            if (Gain <= 0) throw new ArgumentException("Коэффициент усиления должен быть больше 0");
            if (SupplyVoltage <= 0) throw new ArgumentException("Напряжение питания должно быть больше 0");
            if (InputResistor <= 0) throw new ArgumentException("Входной резистор должен быть больше 0");
            if (FeedbackResistor <= 0) throw new ArgumentException("Резистор обратной связи должен быть больше 0");
            if (LoadResistor <= 0) throw new ArgumentException("Резистор нагрузки должен быть больше 0");
            if (Frequency <= 0) throw new ArgumentException("Частота должна быть больше 0");
            if (Temperature < -55 || Temperature > 125)
                throw new ArgumentException("Температура должна быть в диапазоне от -55°C до 125°C");
        }

        private void PerformBasicCalculations()
        {
            var specs = _opAmpSpecs[_selectedOpAmpType];
            double voltageInV = Convert(_inputVoltage, _selectedVoltageUnit, "V", PhysicalQuantity.Voltage);
            double supplyVoltageInV = Convert(_supplyVoltage, _selectedSupplyVoltageUnit, "V", PhysicalQuantity.Voltage);

            // Calculate output voltage considering operating mode
            double outputVoltage = _calculationService.CalculateOutputVoltage(
                voltageInV, _gain, supplyVoltageInV, _selectedOperatingMode, _selectedOpAmpType);
            OutputVoltageResult = $"Выходное напряжение: {Formatter.Format(outputVoltage, PhysicalQuantity.Voltage)}";

            // Calculate actual gain at the given frequency
            double actualGain = _calculationService.CalculateActualGain(_gain, _frequency);

            // Calculate slew rate limitations
            double maxSlewRate = _calculationService.CalculateMaximumSlewRate(_frequency, outputVoltage);
            SlewRateResult = $"Макс. скорость нарастания: {maxSlewRate:F2} В/мкс (Требуемая: {specs.SlewRateTypical:F2} В/мкс)";

            // Power calculations
            var powerAnalysis = _calculationService.AnalyzePower(
                _selectedOpAmpType, supplyVoltageInV, outputVoltage / _loadResistor, _temperature);
            PowerDissipationResult = $"Мощность: {powerAnalysis.TotalPower:F2}Вт (КПД: {powerAnalysis.Efficiency:F1}%)";
        }

        private void PerformAdvancedCalculations()
        {
            // Calculate CMRR and PSRR at the given frequency
            double actualCMRR = _calculationService.CalculateActualCMRR(_frequency);
            double actualPSRR = _calculationService.CalculateActualPSRR(_frequency);
            CMRRResult = $"CMRR: {actualCMRR:F1}дБ, PSRR: {actualPSRR:F1}дБ";

            // Calculate distortion
            double distortion = _calculationService.CalculateDistortion(_inputVoltage * _gain, _supplyVoltage);
            THDResult = $"Коэффициент искажений: {distortion:P3}";

            // Calculate impedances
            double outputImpedance = _calculationService.CalculateOutputImpedance(_gain, _frequency);
            OutputImpedanceResult = $"Выходной импеданс: {Formatter.Format(outputImpedance, PhysicalQuantity.Resistance)}";
        }

        private void PerformTemperatureCalculations()
        {
            var tempEffects = _calculationService.CalculateTemperatureEffects(
                _selectedOpAmpType, _temperature - 25.0);

            TemperatureEffectsResult = $"Дрейф смещения: {tempEffects.InputOffsetDrift:F1}мкВ, " +
                                     $"Дрейф усиления: {tempEffects.GainDrift:F2}%, " +
                                     $"Дрейф тока смещения: {tempEffects.BiasDrift:F1}нА";
        }

        private void PerformNoiseAnalysis()
        {
            var (voltageNoise, currentNoise) = _calculationService.CalculateEquivalentInputNoise(
                _selectedOpAmpType, _frequency, _inputResistor);

            NoiseResult = $"Шум по напряжению: {voltageNoise:F2}нВ/√Гц, " +
                         $"Шум по току: {currentNoise:F2}пА/√Гц";
        }

        #endregion

        #region Helper Methods

        private void ConvertAndUpdateVoltage(ref double voltage, string unit)
        {
            if (InputsAreValid(voltage) && !string.IsNullOrWhiteSpace(unit))
            {
                voltage = Convert(voltage, "V", unit, PhysicalQuantity.Voltage);
                OnPropertyChanged(nameof(InputVoltage));
            }
        }

        private void ConvertAndUpdateResistor(ref double resistance, string unit)
        {
            if (InputsAreValid(resistance) && !string.IsNullOrWhiteSpace(unit))
            {
                resistance = Convert(resistance, "Ω", unit, PhysicalQuantity.Resistance);
                OnPropertyChanged(nameof(InputResistor));
                OnPropertyChanged(nameof(FeedbackResistor));
                OnPropertyChanged(nameof(LoadResistor));
            }
        }

        private void ConvertAndUpdateFrequency(ref double frequency, string unit)
        {
            if (InputsAreValid(frequency) && !string.IsNullOrWhiteSpace(unit))
            {
                frequency = Convert(frequency, "Hz", unit, PhysicalQuantity.Frequency);
                OnPropertyChanged(nameof(Frequency));
            }
        }

        private void HandleCalculationError(Exception ex)
        {
            string errorMessage = $"Ошибка: {ex.Message}";
            OutputVoltageResult = errorMessage;
            PowerDissipationResult = errorMessage;
            SlewRateResult = errorMessage;
            BandwidthResult = errorMessage;
            InputImpedanceResult = errorMessage;
            OutputImpedanceResult = errorMessage;
            CMRRResult = errorMessage;
            NoiseResult = errorMessage;
            THDResult = errorMessage;
            TemperatureEffectsResult = errorMessage;
        }

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
            OnPropertyChanged(nameof(CanCalculate));
            return true;
        }
        #endregion
    }
}
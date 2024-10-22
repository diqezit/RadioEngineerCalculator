using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.UnitC;
using static RadioEngineerCalculator.Services.Validate;
using static RadioEngineerCalculator.Services.ComboBoxService;
using static RadioEngineerCalculator.Infos.ErrorMessages;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class ModulationTab : UserControl, INotifyPropertyChanged
    {
        #region Приватные поля

        private readonly CalculationService _calculationService;
        private readonly Dictionary<string, ObservableCollection<string>> _unitCollections;

        private double _carrierAmplitude;
        private double _modulatingAmplitude;
        private double _carrierFrequency;
        private double _frequencyDeviation;
        private double _carrierPhase;
        private double _phaseDeviation;

        private string _selectedCarrierAmplitudeUnit;
        private string _selectedModulatingAmplitudeUnit;
        private string _selectedCarrierFrequencyUnit;
        private string _selectedFrequencyDeviationUnit;
        private string _selectedCarrierPhaseUnit;
        private string _selectedPhaseDeviationUnit;

        private string _amIndexResult;
        private string _fmIndexResult;
        private string _pmIndexResult;

        #endregion

        #region Публичные свойства

        public double CarrierAmplitude
        {
            get => _carrierAmplitude;
            set
            {
                if (SetProperty(ref _carrierAmplitude, value))
                {
                    OnPropertyChanged(nameof(CanCalculateAMIndex));
                }
            }
        }

        public double ModulatingAmplitude
        {
            get => _modulatingAmplitude;
            set
            {
                if (SetProperty(ref _modulatingAmplitude, value))
                {
                    OnPropertyChanged(nameof(CanCalculateAMIndex));
                }
            }
        }

        public double CarrierFrequency
        {
            get => _carrierFrequency;
            set
            {
                if (SetProperty(ref _carrierFrequency, value))
                {
                    OnPropertyChanged(nameof(CanCalculateFMIndex));
                }
            }
        }

        public double FrequencyDeviation
        {
            get => _frequencyDeviation;
            set
            {
                if (SetProperty(ref _frequencyDeviation, value))
                {
                    OnPropertyChanged(nameof(CanCalculateFMIndex));
                }
            }
        }

        public double CarrierPhase
        {
            get => _carrierPhase;
            set
            {
                if (SetProperty(ref _carrierPhase, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePMIndex));
                }
            }
        }

        public double PhaseDeviation
        {
            get => _phaseDeviation;
            set
            {
                if (SetProperty(ref _phaseDeviation, value))
                {
                    OnPropertyChanged(nameof(CanCalculatePMIndex));
                }
            }
        }

        public string SelectedCarrierAmplitudeUnit
        {
            get => _selectedCarrierAmplitudeUnit;
            set
            {
                if (SetProperty(ref _selectedCarrierAmplitudeUnit, value))
                {
                    ConvertCarrierAmplitude();
                }
            }
        }

        public string SelectedModulatingAmplitudeUnit
        {
            get => _selectedModulatingAmplitudeUnit;
            set
            {
                if (SetProperty(ref _selectedModulatingAmplitudeUnit, value))
                {
                    ConvertModulatingAmplitude();
                }
            }
        }

        public string SelectedCarrierFrequencyUnit
        {
            get => _selectedCarrierFrequencyUnit;
            set
            {
                if (SetProperty(ref _selectedCarrierFrequencyUnit, value))
                {
                    ConvertCarrierFrequency();
                }
            }
        }

        public string SelectedFrequencyDeviationUnit
        {
            get => _selectedFrequencyDeviationUnit;
            set
            {
                if (SetProperty(ref _selectedFrequencyDeviationUnit, value))
                {
                    ConvertFrequencyDeviation();
                }
            }
        }

        public string SelectedCarrierPhaseUnit
        {
            get => _selectedCarrierPhaseUnit;
            set
            {
                if (SetProperty(ref _selectedCarrierPhaseUnit, value))
                {
                    ConvertCarrierPhase();
                }
            }
        }

        public string SelectedPhaseDeviationUnit
        {
            get => _selectedPhaseDeviationUnit;
            set
            {
                if (SetProperty(ref _selectedPhaseDeviationUnit, value))
                {
                    ConvertPhaseDeviation();
                }
            }
        }

        public string AMIndexResult
        {
            get => _amIndexResult;
            set => SetProperty(ref _amIndexResult, value);
        }

        public string FMIndexResult
        {
            get => _fmIndexResult;
            set => SetProperty(ref _fmIndexResult, value);
        }

        public string PMIndexResult
        {
            get => _pmIndexResult;
            set => SetProperty(ref _pmIndexResult, value);
        }

        public ObservableCollection<string> AmplitudeUnits => _unitCollections["Voltage"];
        public ObservableCollection<string> FrequencyUnits => _unitCollections["Frequency"];
        public ObservableCollection<string> AngleUnits => _unitCollections["Angle"];

        #endregion

        #region Команды

        public ICommand CalculateAMIndexCommand { get; private set; }
        public ICommand CalculateFMIndexCommand { get; private set; }
        public ICommand CalculatePMIndexCommand { get; private set; }

        #endregion

        #region Конструктор и инициализация

        public ModulationTab()
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
                ["Voltage"] = new ObservableCollection<string>(GetUnits("Voltage")),
                ["Frequency"] = new ObservableCollection<string>(GetUnits("Frequency")),
                ["Angle"] = new ObservableCollection<string>(GetUnits("Angle"))
            };
        }

        private void InitializeCommands()
        {
            CalculateAMIndexCommand = new RelayCommand(CalculateAMIndex, () => CanCalculateAMIndex);
            CalculateFMIndexCommand = new RelayCommand(CalculateFMIndex, () => CanCalculateFMIndex);
            CalculatePMIndexCommand = new RelayCommand(CalculatePMIndex, () => CanCalculatePMIndex);
        }

        private void InitializeDefaultUnits()
        {
            SelectedCarrierAmplitudeUnit = AmplitudeUnits[0];
            SelectedModulatingAmplitudeUnit = AmplitudeUnits[0];
            SelectedCarrierFrequencyUnit = FrequencyUnits[0];
            SelectedFrequencyDeviationUnit = FrequencyUnits[0];
            SelectedCarrierPhaseUnit = AngleUnits[0];
            SelectedPhaseDeviationUnit = AngleUnits[0];
        }

        #endregion

        #region Методы расчета

        private void CalculateAMIndex()
        {
            try
            {
                ValidateAMInputs();
                double carrierAmplitudeV = Convert(CarrierAmplitude, SelectedCarrierAmplitudeUnit, "V", PhysicalQuantity.Voltage);
                double modulatingAmplitudeV = Convert(ModulatingAmplitude, SelectedModulatingAmplitudeUnit, "V", PhysicalQuantity.Voltage);
                double amIndex = _calculationService.CalculateAMIndex(carrierAmplitudeV, modulatingAmplitudeV);
                AMIndexResult = $"Индекс модуляции AM: {amIndex:F2}";
            }
            catch (Exception ex)
            {
                AMIndexResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculateFMIndex()
        {
            try
            {
                ValidateFMInputs();
                double carrierFrequencyHz = Convert(CarrierFrequency, SelectedCarrierFrequencyUnit, "Hz", PhysicalQuantity.Frequency);
                double frequencyDeviationHz = Convert(FrequencyDeviation, SelectedFrequencyDeviationUnit, "Hz", PhysicalQuantity.Frequency);
                double fmIndex = _calculationService.CalculateFMIndex(carrierFrequencyHz, frequencyDeviationHz);
                FMIndexResult = $"Индекс модуляции FM: {fmIndex:F2}";
            }
            catch (Exception ex)
            {
                FMIndexResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculatePMIndex()
        {
            try
            {
                ValidatePMInputs();
                double carrierPhaseRad = Convert(CarrierPhase, SelectedCarrierPhaseUnit, "rad", PhysicalQuantity.Angle);
                double phaseDeviationRad = Convert(PhaseDeviation, SelectedPhaseDeviationUnit, "rad", PhysicalQuantity.Angle);
                double pmIndex = _calculationService.CalculatePMIndex(carrierPhaseRad, phaseDeviationRad);
                PMIndexResult = $"Индекс модуляции PM: {pmIndex:F2}";
            }
            catch (Exception ex)
            {
                PMIndexResult = $"Ошибка: {ex.Message}";
            }
        }

        #endregion

        #region Методы валидации

        private void ValidateAMInputs()
        {
            if (!InputsAreValid(CarrierAmplitude, ModulatingAmplitude))
            {
                throw new ArgumentException(InvalidInput);
            }
        }

        private void ValidateFMInputs()
        {
            if (!InputsAreValid(CarrierFrequency, FrequencyDeviation))
            {
                throw new ArgumentException(InvalidInput);
            }
        }

        private void ValidatePMInputs()
        {
            if (!InputsAreValid(CarrierPhase, PhaseDeviation))
            {
                throw new ArgumentException(InvalidInput);
            }
        }

        #endregion

        #region Методы конвертации единиц измерения

        private void ConvertCarrierAmplitude() => ConvertUnit(ref _carrierAmplitude, "V", SelectedCarrierAmplitudeUnit, PhysicalQuantity.Voltage);
        private void ConvertModulatingAmplitude() => ConvertUnit(ref _modulatingAmplitude, "V", SelectedModulatingAmplitudeUnit, PhysicalQuantity.Voltage);
        private void ConvertCarrierFrequency() => ConvertUnit(ref _carrierFrequency, "Hz", SelectedCarrierFrequencyUnit, PhysicalQuantity.Frequency);
        private void ConvertFrequencyDeviation() => ConvertUnit(ref _frequencyDeviation, "Hz", SelectedFrequencyDeviationUnit, PhysicalQuantity.Frequency);
        private void ConvertCarrierPhase() => ConvertUnit(ref _carrierPhase, "rad", SelectedCarrierPhaseUnit, PhysicalQuantity.Angle);
        private void ConvertPhaseDeviation() => ConvertUnit(ref _phaseDeviation, "rad", SelectedPhaseDeviationUnit, PhysicalQuantity.Angle);

        private void ConvertUnit(ref double value, string fromUnit, string toUnit, PhysicalQuantity quantity)
        {
            if (InputsAreValid(value) && !string.IsNullOrWhiteSpace(toUnit))
            {
                value = Convert(value, fromUnit, toUnit, quantity);
            }
        }

        #endregion

        #region Свойства CanCalculate

        public bool CanCalculateAMIndex => InputsAreValid(CarrierAmplitude, ModulatingAmplitude) &&
                                           !string.IsNullOrWhiteSpace(SelectedCarrierAmplitudeUnit) &&
                                           !string.IsNullOrWhiteSpace(SelectedModulatingAmplitudeUnit);

        public bool CanCalculateFMIndex => InputsAreValid(CarrierFrequency, FrequencyDeviation) &&
                                           !string.IsNullOrWhiteSpace(SelectedCarrierFrequencyUnit) &&
                                           !string.IsNullOrWhiteSpace(SelectedFrequencyDeviationUnit);

        public bool CanCalculatePMIndex => InputsAreValid(CarrierPhase, PhaseDeviation) &&
                                           !string.IsNullOrWhiteSpace(SelectedCarrierPhaseUnit) &&
                                           !string.IsNullOrWhiteSpace(SelectedPhaseDeviationUnit);

        #endregion

        #region INotifyPropertyChanged

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
using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Numerics;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.UnitC;
using static RadioEngineerCalculator.Services.Validate;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class ResonantCircuitTab : UserControl, INotifyPropertyChanged
    {
        #region Поля и свойства

        private readonly CalculationService _calculationService;
        private readonly Dictionary<string, ObservableCollection<string>> _unitCollections;

        private double _inductance;
        private double _capacitance;
        private double _resistance;
        private double _frequency;
        private string _selectedInductanceUnit;
        private string _selectedCapacitanceUnit;
        private string _selectedResistanceUnit;
        private string _selectedFrequencyUnit;
        private string _resonanceFrequencyResult;
        private string _seriesImpedanceResult;
        private string _parallelImpedanceResult;
        private string _seriesQFactorResult;
        private string _parallelQFactorResult;

        public double Inductance
        {
            get => _inductance;
            set
            {
                if (SetProperty(ref _inductance, value))
                {
                    OnPropertyChanged(nameof(CanCalculateResonanceFrequency));
                    OnPropertyChanged(nameof(CanCalculateImpedance));
                    OnPropertyChanged(nameof(CanCalculateQFactor));
                }
            }
        }

        public double Capacitance
        {
            get => _capacitance;
            set
            {
                if (SetProperty(ref _capacitance, value))
                {
                    OnPropertyChanged(nameof(CanCalculateResonanceFrequency));
                    OnPropertyChanged(nameof(CanCalculateImpedance));
                    OnPropertyChanged(nameof(CanCalculateQFactor));
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
                    OnPropertyChanged(nameof(CanCalculateImpedance));
                    OnPropertyChanged(nameof(CanCalculateQFactor));
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
                    OnPropertyChanged(nameof(CanCalculateImpedance));
                    OnPropertyChanged(nameof(CanCalculateQFactor));
                }
            }
        }

        public string SelectedInductanceUnit
        {
            get => _selectedInductanceUnit;
            set
            {
                if (SetProperty(ref _selectedInductanceUnit, value))
                {
                    ConvertInductance();
                }
            }
        }

        public string SelectedCapacitanceUnit
        {
            get => _selectedCapacitanceUnit;
            set
            {
                if (SetProperty(ref _selectedCapacitanceUnit, value))
                {
                    ConvertCapacitance();
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
                    ConvertResistance();
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

        public string ResonanceFrequencyResult
        {
            get => _resonanceFrequencyResult;
            set => SetProperty(ref _resonanceFrequencyResult, value);
        }

        public string SeriesImpedanceResult
        {
            get => _seriesImpedanceResult;
            set => SetProperty(ref _seriesImpedanceResult, value);
        }

        public string ParallelImpedanceResult
        {
            get => _parallelImpedanceResult;
            set => SetProperty(ref _parallelImpedanceResult, value);
        }

        public string SeriesQFactorResult
        {
            get => _seriesQFactorResult;
            set => SetProperty(ref _seriesQFactorResult, value);
        }

        public string ParallelQFactorResult
        {
            get => _parallelQFactorResult;
            set => SetProperty(ref _parallelQFactorResult, value);
        }

        public ObservableCollection<string> InductanceUnits => _unitCollections["Inductance"];
        public ObservableCollection<string> CapacitanceUnits => _unitCollections["Capacitance"];
        public ObservableCollection<string> ResistanceUnits => _unitCollections["Resistance"];
        public ObservableCollection<string> FrequencyUnits => _unitCollections["Frequency"];

        #endregion

        #region Команды

        public ICommand CalculateResonanceFrequencyCommand { get; private set; }
        public ICommand CalculateImpedanceCommand { get; private set; }
        public ICommand CalculateQFactorCommand { get; private set; }

        public bool CanCalculateResonanceFrequency => InputsAreValid(Inductance, Capacitance) &&
                                                      !string.IsNullOrWhiteSpace(SelectedInductanceUnit) &&
                                                      !string.IsNullOrWhiteSpace(SelectedCapacitanceUnit);

        public bool CanCalculateImpedance => InputsAreValid(Resistance, Inductance, Capacitance, Frequency) &&
                                             !string.IsNullOrWhiteSpace(SelectedResistanceUnit) &&
                                             !string.IsNullOrWhiteSpace(SelectedInductanceUnit) &&
                                             !string.IsNullOrWhiteSpace(SelectedCapacitanceUnit) &&
                                             !string.IsNullOrWhiteSpace(SelectedFrequencyUnit);

        public bool CanCalculateQFactor => InputsAreValid(Inductance, Resistance, Frequency) &&
                                           !string.IsNullOrWhiteSpace(SelectedInductanceUnit) &&
                                           !string.IsNullOrWhiteSpace(SelectedResistanceUnit) &&
                                           !string.IsNullOrWhiteSpace(SelectedFrequencyUnit);

        #endregion

        #region Конструктор и инициализация

        public ResonantCircuitTab()
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
                ["Inductance"] = new ObservableCollection<string>(ComboBoxService.GetUnits("Inductance")),
                ["Capacitance"] = new ObservableCollection<string>(ComboBoxService.GetUnits("Capacitance")),
                ["Resistance"] = new ObservableCollection<string>(ComboBoxService.GetUnits("Resistance")),
                ["Frequency"] = new ObservableCollection<string>(ComboBoxService.GetUnits("Frequency"))
            };
        }

        private void InitializeCommands()
        {
            CalculateResonanceFrequencyCommand = new RelayCommand(CalculateResonanceFrequency, () => CanCalculateResonanceFrequency);
            CalculateImpedanceCommand = new RelayCommand(CalculateImpedance, () => CanCalculateImpedance);
            CalculateQFactorCommand = new RelayCommand(CalculateQFactor, () => CanCalculateQFactor);
        }

        private void InitializeDefaultUnits()
        {
            SelectedInductanceUnit = InductanceUnits[0];
            SelectedCapacitanceUnit = CapacitanceUnits[0];
            SelectedResistanceUnit = ResistanceUnits[0];
            SelectedFrequencyUnit = FrequencyUnits[0];
        }

        #endregion

        #region Методы расчета

        private void CalculateResonanceFrequency()
        {
            try
            {
                double inductanceH = Convert(Inductance, SelectedInductanceUnit, "H", PhysicalQuantity.Inductance);
                double capacitanceF = Convert(Capacitance, SelectedCapacitanceUnit, "F", PhysicalQuantity.Capacitance);
                double result = _calculationService.CalculateResonanceFrequency(inductanceH, capacitanceF);
                ResonanceFrequencyResult = $"Резонансная частота: {FormatResult(result, PhysicalQuantity.Frequency)}";
            }
            catch (Exception ex)
            {
                ResonanceFrequencyResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculateImpedance()
        {
            try
            {
                double resistanceOhm = Convert(Resistance, SelectedResistanceUnit, "Ω", PhysicalQuantity.Resistance);
                double inductanceH = Convert(Inductance, SelectedInductanceUnit, "H", PhysicalQuantity.Inductance);
                double capacitanceF = Convert(Capacitance, SelectedCapacitanceUnit, "F", PhysicalQuantity.Capacitance);
                double frequencyHz = Convert(Frequency, SelectedFrequencyUnit, "Hz", PhysicalQuantity.Frequency);

                Complex seriesImpedance = _calculationService.CalculateSeriesImpedance(resistanceOhm, inductanceH, capacitanceF, frequencyHz);
                Complex parallelImpedance = _calculationService.CalculateParallelImpedance(resistanceOhm, inductanceH, capacitanceF, frequencyHz);

                SeriesImpedanceResult = $"Последовательный импеданс: {FormatComplex(seriesImpedance)} Ом";
                ParallelImpedanceResult = $"Параллельный импеданс: {FormatComplex(parallelImpedance)} Ом";
            }
            catch (Exception ex)
            {
                SeriesImpedanceResult = $"Ошибка: {ex.Message}";
                ParallelImpedanceResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculateQFactor()
        {
            try
            {
                double inductanceH = Convert(Inductance, SelectedInductanceUnit, "H", PhysicalQuantity.Inductance);
                double resistanceOhm = Convert(Resistance, SelectedResistanceUnit, "Ω", PhysicalQuantity.Resistance);
                double frequencyHz = Convert(Frequency, SelectedFrequencyUnit, "Hz", PhysicalQuantity.Frequency);

                double seriesQFactor = _calculationService.CalculateSeriesQFactor(inductanceH, resistanceOhm, frequencyHz);
                double parallelQFactor = _calculationService.CalculateParallelQFactor(inductanceH, resistanceOhm, frequencyHz);

                SeriesQFactorResult = $"Последовательный Q-фактор: {seriesQFactor:F2}";
                ParallelQFactorResult = $"Параллельный Q-фактор: {parallelQFactor:F2}";
            }
            catch (Exception ex)
            {
                SeriesQFactorResult = $"Ошибка: {ex.Message}";
                ParallelQFactorResult = $"Ошибка: {ex.Message}";
            }
        }

        #endregion

        #region Вспомогательные методы

        private void ConvertInductance() => ConvertUnit(ref _inductance, "H", SelectedInductanceUnit, PhysicalQuantity.Inductance);
        private void ConvertCapacitance() => ConvertUnit(ref _capacitance, "F", SelectedCapacitanceUnit, PhysicalQuantity.Capacitance);
        private void ConvertResistance() => ConvertUnit(ref _resistance, "Ω", SelectedResistanceUnit, PhysicalQuantity.Resistance);
        private void ConvertFrequency() => ConvertUnit(ref _frequency, "Hz", SelectedFrequencyUnit, PhysicalQuantity.Frequency);

        private void ConvertUnit(ref double value, string fromUnit, string toUnit, PhysicalQuantity quantity)
        {
            if (InputsAreValid(value) && !string.IsNullOrWhiteSpace(toUnit))
            {
                value = Convert(value, fromUnit, toUnit, quantity);
            }
        }

        private string FormatResult(double value, PhysicalQuantity quantity) => UnitC.AutoFormat(value, quantity);
        private string FormatComplex(Complex complex) => $"{complex.Real:F2} + {complex.Imaginary:F2}j";

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
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
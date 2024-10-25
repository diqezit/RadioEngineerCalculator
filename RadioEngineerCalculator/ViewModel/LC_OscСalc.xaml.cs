using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Numerics;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.UnitConverter;
using static RadioEngineerCalculator.Services.Validate;
using static RadioEngineerCalculator.Services.ComboBoxService;
using static RadioEngineerCalculator.Infos.ErrorMessages;
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

        public double Capacitance { get => _capacitance; set => SetProperty(ref _capacitance, value); }
        public string ParallelImpedanceResult { get => _parallelImpedanceResult; set => SetProperty(ref _parallelImpedanceResult, value); }
        public string ParallelQFactorResult { get => _parallelQFactorResult; set => SetProperty(ref _parallelQFactorResult, value); }
        public double Frequency { get => _frequency; set => SetProperty(ref _frequency, value); }
        public double Inductance { get => _inductance; set => SetProperty(ref _inductance, value); }
        public double Resistance { get => _resistance; set => SetProperty(ref _resistance, value); }
        public string ResonanceFrequencyResult { get => _resonanceFrequencyResult; set => SetProperty(ref _resonanceFrequencyResult, value); }
        public string SelectedCapacitanceUnit
        {
            get => _selectedCapacitanceUnit;
            set
            {
                if (SetProperty(ref _selectedCapacitanceUnit, value))
                {
                    ConvertAndUpdate(ref _capacitance, value, PhysicalQuantity.Capacitance);
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
                    ConvertAndUpdate(ref _frequency, value, PhysicalQuantity.Frequency);
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
                    ConvertAndUpdate(ref _inductance, value, PhysicalQuantity.Inductance);
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
                    ConvertAndUpdate(ref _resistance, value, PhysicalQuantity.Resistance);
                }
            }
        }
        public string SeriesImpedanceResult { get => _seriesImpedanceResult; set => SetProperty(ref _seriesImpedanceResult, value); }
        public string SeriesQFactorResult { get => _seriesQFactorResult; set => SetProperty(ref _seriesQFactorResult, value); }


        public ObservableCollection<string> InductanceUnits => _unitCollections["Inductance"];
        public ObservableCollection<string> CapacitanceUnits => _unitCollections["Capacitance"];
        public ObservableCollection<string> ResistanceUnits => _unitCollections["Resistance"];
        public ObservableCollection<string> FrequencyUnits => _unitCollections["Frequency"];

        #endregion

        #region Команды

        public ICommand CalculateCommand { get; private set; }

        public bool CanCalculate => InputsAreValid(Inductance, Capacitance, Resistance, Frequency) &&
                                    !string.IsNullOrWhiteSpace(SelectedInductanceUnit) &&
                                    !string.IsNullOrWhiteSpace(SelectedCapacitanceUnit) &&
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
                ["Inductance"] = new ObservableCollection<string>(GetUnits(PhysicalQuantity.Inductance.ToString())),
                ["Capacitance"] = new ObservableCollection<string>(GetUnits(PhysicalQuantity.Capacitance.ToString())),
                ["Resistance"] = new ObservableCollection<string>(GetUnits(PhysicalQuantity.Resistance.ToString())),
                ["Frequency"] = new ObservableCollection<string>(GetUnits(PhysicalQuantity.Frequency.ToString()))
            };
        }

        private void InitializeCommands()
        {
            CalculateCommand = new RelayCommand(CalculateAll, () => CanCalculate);
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

        private void CalculateAll()
        {
            try
            {
                CalculateResonanceFrequency();
                CalculateImpedance();
                CalculateQFactor();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private void CalculateResonanceFrequency()
        {
            try
            {
                double inductanceH = Convert(Inductance, SelectedInductanceUnit, "H", PhysicalQuantity.Inductance);
                double capacitanceF = Convert(Capacitance, SelectedCapacitanceUnit, "F", PhysicalQuantity.Capacitance);
                double result = _calculationService.CalculateResonanceFrequency(inductanceH, capacitanceF);
                ResonanceFrequencyResult = $"Резонансная частота: {Formatter.Format(result, PhysicalQuantity.Frequency)}";
            }
            catch (Exception ex)
            {
                HandleError(ex);
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
                HandleError(ex);
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
                HandleError(ex);
            }
        }

        #endregion

        #region Вспомогательные методы

        private void ConvertUnit(ref double value, string baseUnit, string toUnit, PhysicalQuantity quantity)
        {
            if (InputsAreValid(value) && !string.IsNullOrWhiteSpace(toUnit))
            {
                value = Convert(value, baseUnit, toUnit, quantity);
                OnPropertyChanged(quantity.ToString());
            }
        }

        private void ConvertAndUpdate(ref double value, string unit, PhysicalQuantity quantity)
        {
            ConvertUnit(ref value, GetBaseUnit(quantity), unit, quantity);
        }

        private string GetBaseUnit(PhysicalQuantity quantity)
        {
            return quantity switch
            {
                PhysicalQuantity.Inductance => "H",
                PhysicalQuantity.Capacitance => "F",
                PhysicalQuantity.Resistance => "Ω",
                PhysicalQuantity.Frequency => "Hz",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string FormatComplex(Complex complex)
            => $"{Formatter.Format(complex.Real, PhysicalQuantity.Resistance)} + {Formatter.Format(complex.Imaginary, PhysicalQuantity.Resistance)}j";

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
            OnPropertyChanged(nameof(CanCalculate));
            return true;
        }

        #endregion

        #region Ошибки
        private void HandleError(Exception ex)
        {
            ResonanceFrequencyResult = $"Ошибка: {ex.Message}";
            SeriesImpedanceResult = $"Ошибка: {ex.Message}";
            ParallelImpedanceResult = $"Ошибка: {ex.Message}";
            SeriesQFactorResult = $"Ошибка: {ex.Message}";
            ParallelQFactorResult = $"Ошибка: {ex.Message}";
        }
        #endregion
    }
}
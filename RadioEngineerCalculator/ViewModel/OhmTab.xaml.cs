using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Infos.ErrorMessages;
using static RadioEngineerCalculator.Services.ComboBoxService;
using static RadioEngineerCalculator.Services.UnitConverter;
using static RadioEngineerCalculator.Services.Validate;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class OhmTab : UserControl, INotifyPropertyChanged
    {
        #region Поля
        private readonly CalculationService _calculationService;
        private readonly Dictionary<string, ObservableCollection<string>> _unitCollections;

        private double _resistance;
        private double _current;
        private double _voltage;
        private string _selectedResistanceUnit;
        private string _selectedCurrentUnit;
        private string _selectedVoltageUnit;
        private string _resultText;
        #endregion

        #region Конструктор и Инициализация
        public OhmTab()
        {
            InitializeComponent();
            DataContext = this;
            _calculationService = new CalculationService();
            _unitCollections = InitializeUnitCollections();
            InitializeCommands();
            InitializeDefaultUnits();
        }

        private Dictionary<string, ObservableCollection<string>> InitializeUnitCollections() => new()
        {
            ["Resistance"] = new ObservableCollection<string>(GetUnits("Resistance")),
            ["Current"] = new ObservableCollection<string>(GetUnits("Current")),
            ["Voltage"] = new ObservableCollection<string>(GetUnits("Voltage"))
        };

        private void InitializeCommands() => CalculateCommand = new RelayCommand(Calculate, () => CanCalculate);

        private void InitializeDefaultUnits()
        {
            SelectedResistanceUnit = ResistanceUnits[0];
            SelectedCurrentUnit = CurrentUnits[0];
            SelectedVoltageUnit = VoltageUnits[0];
        }
        #endregion

        #region Свойства
        public ICommand CalculateCommand { get; private set; }

        public ObservableCollection<string> ResistanceUnits => _unitCollections["Resistance"];
        public ObservableCollection<string> CurrentUnits => _unitCollections["Current"];
        public ObservableCollection<string> VoltageUnits => _unitCollections["Voltage"];

        public double Resistance
        {
            get => _resistance;
            set => SetAndNotify(ref _resistance, value);
        }

        public double Current
        {
            get => _current;
            set => SetAndNotify(ref _current, value);
        }

        public double Voltage
        {
            get => _voltage;
            set => SetAndNotify(ref _voltage, value);
        }

        public string SelectedResistanceUnit
        {
            get => _selectedResistanceUnit;
            set => SetUnit(ref _selectedResistanceUnit, value, ConvertResistance);
        }

        public string SelectedCurrentUnit
        {
            get => _selectedCurrentUnit;
            set => SetUnit(ref _selectedCurrentUnit, value, ConvertCurrent);
        }

        public string SelectedVoltageUnit
        {
            get => _selectedVoltageUnit;
            set => SetUnit(ref _selectedVoltageUnit, value, ConvertVoltage);
        }

        public string ResultText
        {
            get => _resultText;
            set => SetProperty(ref _resultText, value);
        }

        private bool HasTwoFilledInputs() => new[] { Resistance, Current, Voltage }.Count(v => v != 0) >= 2;

        public bool CanCalculate => HasTwoFilledInputs() &&
                                    !string.IsNullOrWhiteSpace(SelectedResistanceUnit) &&
                                    !string.IsNullOrWhiteSpace(SelectedCurrentUnit) &&
                                    !string.IsNullOrWhiteSpace(SelectedVoltageUnit);
        #endregion

        #region Методы Валидации
        private bool InputsOhmTab(double resistance, double current, double voltage) =>
            new[] { resistance, current, voltage }.Count(v => v != 0) >= 2;
        #endregion

        #region Методы Расчета
        private void Calculate()
        {
            try
            {
                var resistanceOhm = UnitConverter.Convert(Resistance, SelectedResistanceUnit, "Ω", PhysicalQuantity.Resistance);
                var currentAmp = UnitConverter.Convert(Current, SelectedCurrentUnit, "A", PhysicalQuantity.Current);
                var voltageVolt = UnitConverter.Convert(Voltage, SelectedVoltageUnit, "V", PhysicalQuantity.Voltage);

                var emptyField = GetEmptyField();
                if (emptyField is null)
                {
                    ResultText = FieldsAreEmpty;
                    return;
                }

                CalculateValues[emptyField](voltageVolt, currentAmp, resistanceOhm);
            }
            catch (FormatException ex)
            {
                ResultText = $"Ошибка ввода данных: {ex.Message}";
            }
            catch (Exception ex)
            {
                ResultText = $"Ошибка: {ex.Message}";
            }
        }

        private Dictionary<string, Action<double, double, double>> CalculateValues => new()
        {
            ["Resistance"] = (voltage, current, _) => CalculateResistance(voltage, current),
            ["Current"] = (voltage, _, resistance) => CalculateCurrent(voltage, resistance),
            ["Voltage"] = (_, current, resistance) => CalculateVoltage(resistance, current)
        };

        private void CalculateResistance(double voltage, double current)
        {
            var resistance = _calculationService.CalculateResistance(voltage, current);
            Resistance = resistance;
            UpdateSelectedUnit(resistance, ResistanceUnits, UpdateSelectedResistanceUnit);
            ResultText = $"Сопротивление: {AutoFormat(resistance, PhysicalQuantity.Resistance)}";
        }

        private void CalculateCurrent(double voltage, double resistance)
        {
            var current = _calculationService.CalculateCurrent(voltage, resistance);
            Current = current;
            UpdateSelectedUnit(current, CurrentUnits, UpdateSelectedCurrentUnit);
            ResultText = $"Ток: {AutoFormat(current, PhysicalQuantity.Current)}";
        }

        private void CalculateVoltage(double resistance, double current)
        {
            var voltage = _calculationService.CalculateVoltage(resistance, current);
            Voltage = voltage;
            UpdateSelectedUnit(voltage, VoltageUnits, UpdateSelectedVoltageUnit);
            ResultText = $"Напряжение: {AutoFormat(voltage, PhysicalQuantity.Voltage)}";
        }
        #endregion

        #region Методы Обновления Единиц Измерения
        private void UpdateSelectedUnit(double value, ObservableCollection<string> units, Action<double> updateUnit)
        {
            var formattedValue = Formatter.Format(value, PhysicalQuantity.Resistance);
            var unit = formattedValue.Split(' ')[1];

            if (units.Contains(unit))
            {
                updateUnit(value);
            }
            else
            {
                ResultText = $"Недопустимая единица измерения: {unit}";
            }
        }

        private void UpdateSelectedResistanceUnit(double resistance) => SelectedResistanceUnit = AutoFormat(resistance, PhysicalQuantity.Resistance).Split(' ')[1];

        private void UpdateSelectedCurrentUnit(double current) => SelectedCurrentUnit = AutoFormat(current, PhysicalQuantity.Current).Split(' ')[1];

        private void UpdateSelectedVoltageUnit(double voltage) => SelectedVoltageUnit = AutoFormat(voltage, PhysicalQuantity.Voltage).Split(' ')[1];
        #endregion

        #region Вспомогательные Методы
        private string GetEmptyField() => Resistance == 0 ? "Resistance" : Current == 0 ? "Current" : Voltage == 0 ? "Voltage" : null;

        private void ConvertResistance() => ConvertUnit(ref _resistance, "Ω", SelectedResistanceUnit, PhysicalQuantity.Resistance);
        private void ConvertCurrent() => ConvertUnit(ref _current, "A", SelectedCurrentUnit, PhysicalQuantity.Current);
        private void ConvertVoltage() => ConvertUnit(ref _voltage, "V", SelectedVoltageUnit, PhysicalQuantity.Voltage);

        private void ConvertUnit(ref double value, string fromUnit, string toUnit, PhysicalQuantity quantity)
        {
            if (InputsAreValid(value) && !string.IsNullOrWhiteSpace(toUnit))
            {
                value = Convert(value, fromUnit, toUnit, quantity);
            }
        }
        #endregion

        #region Методы Уведомления
        protected bool SetAndNotify<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        #region Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void SetUnit(ref string field, string value, Action convertMethod)
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(CanCalculate));
                convertMethod();
            }
        }
        #endregion
    }
}
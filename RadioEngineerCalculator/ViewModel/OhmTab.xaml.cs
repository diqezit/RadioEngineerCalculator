using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.UnitC;
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

        private Dictionary<string, ObservableCollection<string>> InitializeUnitCollections()
        {
            return new Dictionary<string, ObservableCollection<string>>
            {
                ["Resistance"] = new ObservableCollection<string>(ComboBoxService.GetUnits("Resistance")),
                ["Current"] = new ObservableCollection<string>(ComboBoxService.GetUnits("Current")),
                ["Voltage"] = new ObservableCollection<string>(ComboBoxService.GetUnits("Voltage"))
            };
        }

        private void InitializeCommands()
        {
            CalculateCommand = new RelayCommand(Calculate, () => CanCalculate);
        }

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
            set
            {
                if (double.TryParse(value.ToString(), out double parsedValue))
                {
                    SetProperty(ref _resistance, parsedValue);
                }
                else
                {
                    SetProperty(ref _resistance, 0.0);
                }
                OnPropertyChanged(nameof(CanCalculate));
            }
        }

        public double Current
        {
            get => _current;
            set
            {
                if (double.TryParse(value.ToString(), out double parsedValue))
                {
                    SetProperty(ref _current, parsedValue);
                }
                else
                {
                    SetProperty(ref _current, 0.0);
                }
                OnPropertyChanged(nameof(CanCalculate));
            }
        }

        public double Voltage
        {
            get => _voltage;
            set
            {
                if (double.TryParse(value.ToString(), out double parsedValue))
                {
                    SetProperty(ref _voltage, parsedValue);
                }
                else
                {
                    SetProperty(ref _voltage, 0.0);
                }
                OnPropertyChanged(nameof(CanCalculate));
            }
        }

        public string SelectedResistanceUnit
        {
            get => _selectedResistanceUnit;
            set
            {
                if (SetProperty(ref _selectedResistanceUnit, value))
                {
                    OnPropertyChanged(nameof(CanCalculate));
                    ConvertResistance();
                }
            }
        }

        public string SelectedCurrentUnit
        {
            get => _selectedCurrentUnit;
            set
            {
                if (SetProperty(ref _selectedCurrentUnit, value))
                {
                    OnPropertyChanged(nameof(CanCalculate));
                    ConvertCurrent();
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
                    OnPropertyChanged(nameof(CanCalculate));
                    ConvertVoltage();
                }
            }
        }

        public string ResultText
        {
            get => _resultText;
            set => SetProperty(ref _resultText, value);
        }

        public bool CanCalculate => InputsOhmTab(Resistance, Current, Voltage) &&
                                    !string.IsNullOrWhiteSpace(SelectedResistanceUnit) &&
                                    !string.IsNullOrWhiteSpace(SelectedCurrentUnit) &&
                                    !string.IsNullOrWhiteSpace(SelectedVoltageUnit);
        #endregion

        #region Методы Валидации
        private bool InputsOhmTab(double resistance, double current, double voltage)
        {
            int filledFieldsCount = 0;
            if (resistance != 0) filledFieldsCount++;
            if (current != 0) filledFieldsCount++;
            if (voltage != 0) filledFieldsCount++;

            return filledFieldsCount >= 2;
        }
        #endregion

        #region Методы Расчета
        private void Calculate()
        {
            try
            {
                double resistanceOhm = Convert(Resistance, SelectedResistanceUnit, "Ω", PhysicalQuantity.Resistance);
                double currentAmp = Convert(Current, SelectedCurrentUnit, "A", PhysicalQuantity.Current);
                double voltageVolt = Convert(Voltage, SelectedVoltageUnit, "V", PhysicalQuantity.Voltage);

                string emptyField = GetEmptyField();
                if (string.IsNullOrEmpty(emptyField))
                {
                    ResultText = Info.FieldAreFull;
                    return;
                }

                PerformCalculation(emptyField, voltageVolt, currentAmp, resistanceOhm);
            }
            catch (Exception ex)
            {
                ResultText = $"Ошибка: {ex.Message}";
            }
        }

        private void PerformCalculation(string emptyField, double voltage, double current, double resistance)
        {
            switch (emptyField)
            {
                case "Resistance":
                    CalculateResistance(voltage, current);
                    break;
                case "Current":
                    CalculateCurrent(voltage, resistance);
                    break;
                case "Voltage":
                    CalculateVoltage(resistance, current);
                    break;
            }
        }

        private void CalculateResistance(double voltage, double current)
        {
            var resistance = _calculationService.CalculateResistance(voltage, current);
            Resistance = resistance;
            UpdateSelectedResistanceUnit(resistance);
            ResultText = $"Сопротивление: {AutoFormat(resistance, PhysicalQuantity.Resistance)}";
        }

        private void CalculateCurrent(double voltage, double resistance)
        {
            var current = _calculationService.CalculateCurrent(voltage, resistance);
            Current = current;
            UpdateSelectedCurrentUnit(current);
            ResultText = $"Ток: {AutoFormat(current, PhysicalQuantity.Current)}";
        }

        private void CalculateVoltage(double resistance, double current)
        {
            var voltage = _calculationService.CalculateVoltage(resistance, current);
            Voltage = voltage;
            UpdateSelectedVoltageUnit(voltage);
            ResultText = $"Напряжение: {AutoFormat(voltage, PhysicalQuantity.Voltage)}";
        }
        #endregion

        #region Методы Обновления Единиц Измерения
        private void UpdateSelectedResistanceUnit(double resistance)
        {
            var formattedValue = AutoFormat(resistance, PhysicalQuantity.Resistance);
            var unit = formattedValue.Split(' ')[1];
            if (ResistanceUnits.Contains(unit))
            {
                SelectedResistanceUnit = unit;
            }
            else
            {
                ResultText = $"Недопустимая единица измерения: {unit}";
            }
        }

        private void UpdateSelectedCurrentUnit(double current)
        {
            var formattedValue = AutoFormat(current, PhysicalQuantity.Current);
            var unit = formattedValue.Split(' ')[1];
            if (CurrentUnits.Contains(unit))
            {
                SelectedCurrentUnit = unit;
            }
            else
            {
                ResultText = $"Недопустимая единица измерения: {unit}";
            }
        }

        private void UpdateSelectedVoltageUnit(double voltage)
        {
            var formattedValue = AutoFormat(voltage, PhysicalQuantity.Voltage);
            var unit = formattedValue.Split(' ')[1];
            if (VoltageUnits.Contains(unit))
            {
                SelectedVoltageUnit = unit;
            }
            else
            {
                ResultText = $"Недопустимая единица измерения: {unit}";
            }
        }
        #endregion

        #region Вспомогательные Методы
        private string GetEmptyField()
        {
            if (Resistance == 0) return "Resistance";
            if (Current == 0) return "Current";
            if (Voltage == 0) return "Voltage";
            return null;
        }

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

        #region Реализация INotifyPropertyChanged
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
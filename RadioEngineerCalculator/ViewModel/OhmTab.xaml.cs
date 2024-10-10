using RadioEngineerCalculator.Services;
using RadioEngineerCalculator.Infos;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class OhmTab : UserControl, INotifyPropertyChanged
    {
        private readonly CalculationService _calculationService;
        private string _resultText;

        public ObservableCollection<string> ResistanceUnits { get; }
        public ObservableCollection<string> CurrentUnits { get; }
        public ObservableCollection<string> VoltageUnits { get; }

        public string ResultText
        {
            get => _resultText;
            set
            {
                if (_resultText != value)
                {
                    _resultText = value;
                    OnPropertyChanged();
                }
            }
        }

        public OhmTab()
        {
            InitializeComponent();
            _calculationService = new CalculationService();
            ResistanceUnits = ComboBoxService.GetUnits("Resistance");
            CurrentUnits = ComboBoxService.GetUnits("Current");
            VoltageUnits = ComboBoxService.GetUnits("Voltage");
            DataContext = this;
        }

        private void CalculateOhm(object sender, RoutedEventArgs e)
        {
            if (!TryGetInputValues(out var resistance, out var resistanceUnit,
                                   out var current, out var currentUnit,
                                   out var voltage, out var voltageUnit))
            {
                return;
            }

            var emptyField = GetEmptyField();
            if (string.IsNullOrEmpty(emptyField))
            {
                ResultText = Info.FieldAreFull;
                return;
            }

            // Преобразуем единицы измерения в базовые (Ом, Вольт, Ампер)
            voltage = UnitC.Conv.Voltage(voltage, voltageUnit, "V");
            current = UnitC.Conv.Current(current, currentUnit, "A");
            resistance = UnitC.Conv.Resistance(resistance, resistanceUnit, "Ω");

            PerformCalculation(emptyField, voltage, current, resistance);
        }

        private void PerformCalculation(string emptyField, double voltage, double current, double resistance)
        {
            if (!ValidateValues(voltage, current, resistance, emptyField))
            {
                return;
            }

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

        private bool ValidateValues(double voltage, double current, double resistance, string emptyField)
        {
            // Общая проверка на отрицательные значения
            if (voltage < 0 || current < 0 || resistance < 0)
            {
                switch (emptyField)
                {
                    case "Resistance":
                        ResultText = Info.VIErrorNegative;
                        break;
                    case "Current":
                        ResultText = Info.VRErrorNegative;
                        break;
                    case "Voltage":
                        ResultText = Info.RIErrorNegative;
                        break;
                    default:
                        ResultText = Info.InvalidFieldError;
                        break;
                }
                return false;
            }
            return true;
        }

        private void CalculateResistance(double voltage, double current)
        {
            var resistance = _calculationService.CalculateResistance(voltage, current);
            txtResistance.Text = resistance.ToString();
            ResultText = $"Сопротивление: {UnitC.Form.Resistance(resistance)}";
        }

        private void CalculateCurrent(double voltage, double resistance)
        {
            var current = _calculationService.CalculateCurrent(voltage, resistance);
            txtCurrent.Text = current.ToString();
            ResultText = $"Ток: {UnitC.Form.Current(current)}";
        }

        private void CalculateVoltage(double resistance, double current)
        {
            var voltage = _calculationService.CalculateVoltage(resistance, current);
            txtVoltage.Text = voltage.ToString();
            ResultText = $"Напряжение: {UnitC.Form.Voltage(voltage)}";
        }

        private string GetEmptyField()
        {
            // Используем словарь для сопоставления полей с их значениями
            var fields = new Dictionary<string, string>
            {
                {"Resistance", txtResistance.Text},
                {"Current", txtCurrent.Text},
                {"Voltage", txtVoltage.Text}
            };

            // Возвращаем первое пустое поле
            foreach (var field in fields)
            {
                if (string.IsNullOrEmpty(field.Value))
                {
                    return field.Key;
                }
            }
            return null;
        }

        private bool TryGetInputValues(out double resistance, out string resistanceUnit,
                                       out double current, out string currentUnit,
                                       out double voltage, out string voltageUnit)
        {
            resistance = current = voltage = 0;
            resistanceUnit = cmbResistanceUnit.SelectedItem?.ToString();
            currentUnit = cmbCurrentUnit.SelectedItem?.ToString();
            voltageUnit = cmbVoltageUnit.SelectedItem?.ToString();

            // Проверяем корректность ввода числовых значений
            bool isResistanceValid = double.TryParse(txtResistance.Text, out resistance) || string.IsNullOrEmpty(txtResistance.Text);
            bool isCurrentValid = double.TryParse(txtCurrent.Text, out current) || string.IsNullOrEmpty(txtCurrent.Text);
            bool isVoltageValid = double.TryParse(txtVoltage.Text, out voltage) || string.IsNullOrEmpty(txtVoltage.Text);

            // Проверяем, что единицы измерения выбраны корректно
            if (isResistanceValid && isCurrentValid && isVoltageValid &&
                !string.IsNullOrEmpty(resistanceUnit) &&
                !string.IsNullOrEmpty(currentUnit) &&
                !string.IsNullOrEmpty(voltageUnit))
            {
                return true;
            }

            // Если что-то пошло не так, показываем сообщение об ошибке
            MessageBox.Show(ErrorMessages.InvalidInput, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
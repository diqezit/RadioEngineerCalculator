using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class OhmTab : UserControl
    {
        public ObservableCollection<string> ResistanceUnits { get; set; }
        public ObservableCollection<string> CurrentUnits { get; set; }
        public ObservableCollection<string> VoltageUnits { get; set; }

        private readonly CalculationService _calculationService;

        public OhmTab()
        {
            InitializeComponent();
            ResistanceUnits = ComboBoxService.GetUnits("Resistance");
            CurrentUnits = ComboBoxService.GetUnits("Current");
            VoltageUnits = ComboBoxService.GetUnits("Voltage");
            _calculationService = new CalculationService();
            DataContext = this;
        }

        private void CalculateOhm(object sender, RoutedEventArgs e)
        {
            if (TryGetInputValues(out double resistance, out string resistanceUnit,
                                  out double current, out string currentUnit,
                                  out double voltage, out string voltageUnit))
            {
                string emptyField = GetEmptyField();

                if (!string.IsNullOrEmpty(emptyField))
                {
                    // Конвертируем значения в стандартные единицы
                    voltage = UnitC.Conv.Voltage(voltage, voltageUnit, "V");
                    current = UnitC.Conv.Current(current, currentUnit, "A");
                    resistance = UnitC.Conv.Resistance(resistance, resistanceUnit, "Ω");

                    switch (emptyField)
                    {
                        case "Resistance":
                            resistance = _calculationService.CalculateResistance(voltage, current);
                            txtResistance.Text = resistance.ToString();
                            txtResult.Text = $"Сопротивление: {UnitC.Form.Resistance(resistance)}";
                            break;

                        case "Current":
                            current = _calculationService.CalculateCurrent(voltage, resistance);
                            txtCurrent.Text = current.ToString();
                            txtResult.Text = $"Ток: {UnitC.Form.Current(current)}";
                            break;

                        case "Voltage":
                            voltage = _calculationService.CalculateVoltage(resistance, current);
                            txtVoltage.Text = voltage.ToString();
                            txtResult.Text = $"Напряжение: {UnitC.Form.Voltage(voltage)}";
                            break;

                        default:
                            txtResult.Text = "Ошибка: Оставьте одно поле пустым для расчета.";
                            break;
                    }
                }
                else
                {
                    txtResult.Text = "Ошибка: Все поля заполнены. Оставьте одно поле пустым для расчета.";
                }
            }
        }

        // Возвращает название пустого поля для расчета (Resistance, Current, Voltage)
        private string GetEmptyField()
        {
            if (string.IsNullOrEmpty(txtResistance.Text)) return "Resistance";
            if (string.IsNullOrEmpty(txtCurrent.Text)) return "Current";
            if (string.IsNullOrEmpty(txtVoltage.Text)) return "Voltage";
            return null;
        }

        private bool TryGetInputValues(out double resistance, out string resistanceUnit,
                                       out double current, out string currentUnit,
                                       out double voltage, out string voltageUnit)
        {
            resistance = 0;
            current = 0;
            voltage = 0;
            resistanceUnit = cmbResistanceUnit.SelectedItem?.ToString();
            currentUnit = cmbCurrentUnit.SelectedItem?.ToString();
            voltageUnit = cmbVoltageUnit.SelectedItem?.ToString();

            bool isResistanceValid = double.TryParse(txtResistance.Text, out resistance) || string.IsNullOrEmpty(txtResistance.Text);
            bool isCurrentValid = double.TryParse(txtCurrent.Text, out current) || string.IsNullOrEmpty(txtCurrent.Text);
            bool isVoltageValid = double.TryParse(txtVoltage.Text, out voltage) || string.IsNullOrEmpty(txtVoltage.Text);

            if (isResistanceValid && isCurrentValid && isVoltageValid &&
                !string.IsNullOrEmpty(resistanceUnit) &&
                !string.IsNullOrEmpty(currentUnit) &&
                !string.IsNullOrEmpty(voltageUnit))
            {
                return true;
            }

            MessageBox.Show("Неверный ввод. Пожалуйста, введите корректные числа и выберите единицы измерения.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
    }
}
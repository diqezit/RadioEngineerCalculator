using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class PowerCalculationTab : UserControl
    {
        public ObservableCollection<string> CurrentUnits { get; set; }
        public ObservableCollection<string> ResistanceUnits { get; set; }
        public ObservableCollection<string> VoltageUnits { get; set; }
        public ObservableCollection<string> ApparentPowerUnits { get; set; }
        public ObservableCollection<string> RealPowerUnits { get; set; }

        private readonly CalculationService _calculationService;

        public PowerCalculationTab()
        {
            InitializeComponent();
            CurrentUnits = ComboBoxService.GetUnits("Current");
            ResistanceUnits = ComboBoxService.GetUnits("Resistance");
            VoltageUnits = ComboBoxService.GetUnits("Voltage");
            ApparentPowerUnits = ComboBoxService.GetUnits("Power");
            RealPowerUnits = ComboBoxService.GetUnits("Power");
            _calculationService = new CalculationService();
            DataContext = this;
        }

        private void CalculatePower(object sender, RoutedEventArgs e)
        {
            if (!TryGetInputValues(txtCurrent, cmbCurrentUnit, txtResistance, cmbResistanceUnit,
                                   out double current, out string currentUnit, out double resistance, out string resistanceUnit))
            {
                return;
            }

            try
            {
                current = UnitC.Conv.Current(current, currentUnit, "A");
                resistance = UnitC.Conv.Resistance(resistance, resistanceUnit, "Ω");

                double result = _calculationService.CalculatePower(current, resistance);
                txtPowerResult.Text = $"Power Result: {FormatPower(result)}";
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CalculatePowerVI(object sender, RoutedEventArgs e)
        {
            if (!TryGetInputValues(txtVoltage, cmbVoltageUnit, txtCurrentVI, cmbCurrentUnitVI,
                                   out double voltage, out string voltageUnit, out double current, out string currentUnit))
            {
                return;
            }

            try
            {
                voltage = UnitC.Conv.Voltage(voltage, voltageUnit, "V");
                current = UnitC.Conv.Current(current, currentUnit, "A");

                double result = _calculationService.CalculatePowerVI(voltage, current);
                txtPowerResultVI.Text = $"Power Result: {FormatPower(result)}";
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CalculatePowerFactor(object sender, RoutedEventArgs e)
        {
            if (!TryGetInputValues(txtApparentPower, cmbApparentPowerUnit, txtRealPower, cmbRealPowerUnit,
                                   out double apparentPower, out string apparentPowerUnit, out double realPower, out string realPowerUnit))
            {
                return;
            }

            try
            {
                apparentPower = UnitC.Conv.Power(apparentPower, apparentPowerUnit, "VA");
                realPower = UnitC.Conv.Power(realPower, realPowerUnit, "W");

                double powerFactor = _calculationService.CalculatePowerFactor(realPower, apparentPower);
                txtPowerFactorResult.Text = $"Power Factor: {FormatPowerFactor(powerFactor)}";
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CalculateReactivePower(object sender, RoutedEventArgs e)
        {
            if (!TryGetInputValues(txtApparentPowerReactive, cmbApparentPowerUnitReactive, txtRealPowerReactive, cmbRealPowerUnitReactive,
                                   out double apparentPower, out string apparentPowerUnit, out double realPower, out string realPowerUnit))
            {
                return;
            }

            try
            {
                apparentPower = UnitC.Conv.Power(apparentPower, apparentPowerUnit, "VA");
                realPower = UnitC.Conv.Power(realPower, realPowerUnit, "W");

                double reactivePower = _calculationService.CalculateReactivePower(apparentPower, realPower);
                txtReactivePowerResult.Text = $"Reactive Power: {FormatReactivePower(reactivePower)}";
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private bool TryGetInputValues(TextBox valueBox1, ComboBox unitBox1, TextBox valueBox2, ComboBox unitBox2,
                                       out double value1, out string unit1, out double value2, out string unit2)
        {
            value1 = 0;
            value2 = 0;
            unit1 = unitBox1.SelectedItem?.ToString();
            unit2 = unitBox2.SelectedItem?.ToString();

            if (double.TryParse(valueBox1.Text, out value1) &&
                double.TryParse(valueBox2.Text, out value2) &&
                !string.IsNullOrEmpty(unit1) &&
                !string.IsNullOrEmpty(unit2))
            {
                return true;
            }

            ShowErrorMessage("Invalid input. Please enter valid numbers and select units.");
            return false;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string FormatPower(double power)
        {
            return UnitC.Form.Power(power);
        }

        private string FormatPowerFactor(double powerFactor)
        {
            return $"{powerFactor:F4}";
        }

        private string FormatReactivePower(double reactivePower)
        {
            return UnitC.Form.ReactivePower(reactivePower);
        }
    }
}
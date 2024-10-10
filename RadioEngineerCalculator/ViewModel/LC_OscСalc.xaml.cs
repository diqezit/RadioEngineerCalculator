using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Numerics;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class ResonantCircuitTab : UserControl
    {
        public ObservableCollection<string> InductanceUnits { get; }
        public ObservableCollection<string> CapacitanceUnits { get; }
        public ObservableCollection<string> ResistanceUnits { get; }
        public ObservableCollection<string> FrequencyUnits { get; }

        private readonly CalculationService _calculationService;

        public ResonantCircuitTab()
        {
            InitializeComponent();
            InductanceUnits = ComboBoxService.GetUnits("Inductance");
            CapacitanceUnits = ComboBoxService.GetUnits("Capacitance");
            ResistanceUnits = ComboBoxService.GetUnits("Resistance");
            FrequencyUnits = ComboBoxService.GetUnits("Frequency");
            _calculationService = new CalculationService();
            DataContext = this;
        }

        private bool TryGetInputValues(TextBox txtInductance, ComboBox cmbInductanceUnit,
                                       TextBox txtCapacitance, ComboBox cmbCapacitanceUnit,
                                       out double inductance, out string inductanceUnit,
                                       out double capacitance, out string capacitanceUnit)
        {
            inductance = 0;
            capacitance = 0;
            inductanceUnit = capacitanceUnit = null;

            if (!double.TryParse(txtInductance.Text, out inductance) || inductance <= 0)
            {
                ShowErrorMessage(ErrorMessages.InvalidInductanceInput);
                return false;
            }

            if (!double.TryParse(txtCapacitance.Text, out capacitance) || capacitance <= 0)
            {
                ShowErrorMessage(ErrorMessages.InvalidCapacitanceInput);
                return false;
            }

            inductanceUnit = cmbInductanceUnit.SelectedItem?.ToString();
            capacitanceUnit = cmbCapacitanceUnit.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(inductanceUnit) || string.IsNullOrEmpty(capacitanceUnit))
            {
                ShowErrorMessage(ErrorMessages.InvalidInput);
                return false;
            }

            return true;
        }

        private void CalculateResonanceFrequency(object sender, RoutedEventArgs e)
        {
            if (!TryGetInputValues(txtInductance, cmbInductanceUnit, txtCapacitance, cmbCapacitanceUnit,
                                   out double inductance, out string inductanceUnit,
                                   out double capacitance, out string capacitanceUnit))
            {
                return;
            }

            try
            {
                inductance = UnitC.Conv.Inductance(inductance, inductanceUnit, "H");
                capacitance = UnitC.Conv.Capacitance(capacitance, capacitanceUnit, "F");

                double result = _calculationService.CalculateResonanceFrequency(inductance, capacitance);
                txtResonanceFrequencyResult.Text = $"Резонансная частота: {FormatFrequency(result)}";

                // Обновление полей с единицами измерения для ясности
                txtInductance.Text = inductance.ToString();
                txtCapacitance.Text = capacitance.ToString();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }


        private void CalculateImpedance(object sender, RoutedEventArgs e)
        {
            if (!TryGetInputValues(txtResistance, cmbResistanceUnit, txtInductance, cmbInductanceUnit,
                                   txtCapacitance, cmbCapacitanceUnit, txtFrequency, cmbFrequencyUnit,
                                   out double resistance, out string resistanceUnit,
                                   out double inductance, out string inductanceUnit,
                                   out double capacitance, out string capacitanceUnit,
                                   out double frequency, out string frequencyUnit))
            {
                return;
            }

            try
            {
                resistance = UnitC.Conv.Resistance(resistance, resistanceUnit, "Ohm");
                inductance = UnitC.Conv.Inductance(inductance, inductanceUnit, "H");
                capacitance = UnitC.Conv.Capacitance(capacitance, capacitanceUnit, "F");
                frequency = UnitC.Conv.Frequency(frequency, frequencyUnit, "Hz");

                Complex seriesImpedance = _calculationService.CalculateSeriesImpedance(resistance, inductance, capacitance, frequency);
                Complex parallelImpedance = _calculationService.CalculateParallelImpedance(resistance, inductance, capacitance, frequency);

                txtSeriesImpedanceResult.Text = $"Series Impedance: {FormatComplex(seriesImpedance)} Ohm";
                txtParallelImpedanceResult.Text = $"Parallel Impedance: {FormatComplex(parallelImpedance)} Ohm";
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CalculateQFactor(object sender, RoutedEventArgs e)
        {
            if (!TryGetInputValues(txtInductance, cmbInductanceUnit, txtResistance, cmbResistanceUnit,
                                   txtFrequency, cmbFrequencyUnit,
                                   out double inductance, out string inductanceUnit,
                                   out double resistance, out string resistanceUnit,
                                   out double frequency, out string frequencyUnit))
            {
                return;
            }

            try
            {
                inductance = UnitC.Conv.Inductance(inductance, inductanceUnit, "H");
                resistance = UnitC.Conv.Resistance(resistance, resistanceUnit, "Ohm");
                frequency = UnitC.Conv.Frequency(frequency, frequencyUnit, "Hz");

                double seriesQFactor = _calculationService.CalculateSeriesQFactor(inductance, resistance, frequency);
                double parallelQFactor = _calculationService.CalculateParallelQFactor(inductance, resistance, frequency);

                txtSeriesQFactorResult.Text = $"Series Q Factor: {seriesQFactor:F2}";
                txtParallelQFactorResult.Text = $"Parallel Q Factor: {parallelQFactor:F2}";
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private string FormatFrequency(double frequency) => UnitC.Form.Frequency(frequency);

        private string FormatComplex(Complex complex) => $"{complex.Real:F2} + {complex.Imaginary:F2}j";

        private void ShowErrorMessage(string message) =>
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
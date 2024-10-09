using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class AmplifierTab : UserControl
    {
        public ObservableCollection<string> PowerInUnits { get; set; }
        public ObservableCollection<string> PowerOutUnits { get; set; }

        private readonly CalculationService _calculationService;

        public AmplifierTab()
        {
            InitializeComponent();
            PowerInUnits = ComboBoxService.GetUnits("Power");
            PowerOutUnits = ComboBoxService.GetUnits("Power");
            _calculationService = new CalculationService();
            DataContext = this;
        }

        private void CalculateGain(object sender, RoutedEventArgs e)
        {
            if (!TryGetPowerValues(out double powerIn, out string powerInUnit, out double powerOut, out string powerOutUnit))
            {
                txtGainResult.Text = "Invalid input";
                return;
            }

            try
            {
                powerIn = UnitC.Conv.Power(powerIn, powerInUnit, "W");
                powerOut = UnitC.Conv.Power(powerOut, powerOutUnit, "W");

                double gain = _calculationService.CalculateGain(powerIn, powerOut);
                txtGainResult.Text = $"Gain: {FormatGain(gain)}";
            }
            catch (Exception ex)
            {
                txtGainResult.Text = $"Error: {ex.Message}";
            }
        }

        private void CalculateNoiseFigure(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtNoiseFactor.Text, out double noiseFactor))
            {
                txtNoiseFigureResult.Text = "Invalid input";
                return;
            }

            try
            {
                double noiseFigure = _calculationService.CalculateNoiseFigure(noiseFactor);
                txtNoiseFigureResult.Text = $"Noise Figure: {FormatNoiseFigure(noiseFigure)}";
            }
            catch (Exception ex)
            {
                txtNoiseFigureResult.Text = $"Error: {ex.Message}";
            }
        }

        private bool TryGetPowerValues(out double powerIn, out string powerInUnit, out double powerOut, out string powerOutUnit)
        {
            powerIn = 0;
            powerOut = 0;
            powerInUnit = cmbPowerInUnit.SelectedItem?.ToString();
            powerOutUnit = cmbPowerOutUnit.SelectedItem?.ToString();

            if (double.TryParse(txtPowerIn.Text, out powerIn) &&
                double.TryParse(txtPowerOut.Text, out powerOut) &&
                !string.IsNullOrEmpty(powerInUnit) &&
                !string.IsNullOrEmpty(powerOutUnit))
            {
                return true;
            }

            return false;
        }

        private string FormatGain(double gain)
        {
            return $"{gain:F2} dB";
        }

        private string FormatNoiseFigure(double noiseFigure)
        {
            return $"{noiseFigure:F2} dB";
        }

        private void CalculateEfficiency(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtOutputPower.Text, out double outputPower) &&
                double.TryParse(txtInputDCPower.Text, out double inputDCPower))
            {
                try
                {
                    double efficiency = _calculationService.CalculateAmplifierEfficiency(outputPower, inputDCPower);
                    txtEfficiencyResult.Text = $"Efficiency: {FormatEfficiency(efficiency)}";
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter valid numbers for Output Power and Input DC Power.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Calculate1dBCompressionPoint(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtInputPower.Text, out double inputPower) &&
                double.TryParse(txtOutputPowerDBm.Text, out double outputPower) &&
                double.TryParse(txtSmallSignalGain.Text, out double smallSignalGain))
            {
                try
                {
                    double compressionPoint = _calculationService.Calculate1dBCompressionPoint(inputPower, outputPower, smallSignalGain);
                    txt1dBCompressionResult.Text = $"1dB Compression Point: {FormatCompressionPoint(compressionPoint)}";
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter valid numbers for Input Power, Output Power, and Small Signal Gain.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CalculateIP3(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtFundamentalPower.Text, out double fundamentalPower) &&
                double.TryParse(txtThirdOrderPower.Text, out double thirdOrderPower))
            {
                try
                {
                    double ip3 = _calculationService.CalculateIP3(fundamentalPower, thirdOrderPower);
                    txtIP3Result.Text = $"Third-Order Intercept Point (IP3): {FormatIP3(ip3)}";
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter valid numbers for Fundamental Power and Third-Order Product Power.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private string FormatEfficiency(double efficiency)
        {
            return $"{efficiency:F2}%";
        }

        private string FormatCompressionPoint(double compressionPoint)
        {
            return $"{compressionPoint:F2} dBm";
        }

        private string FormatIP3(double ip3)
        {
            return $"{ip3:F2} dBm";
        }
    }
}
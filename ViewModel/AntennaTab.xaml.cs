using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class AntennaTab : UserControl
    {
        public ObservableCollection<string> FrequencyUnits { get; set; }
        public ObservableCollection<string> ForwardPowerUnits { get; set; }
        public ObservableCollection<string> ReflectedPowerUnits { get; set; }

        private readonly CalculationService _calculationService;

        public AntennaTab()
        {
            InitializeComponent();
            FrequencyUnits = ComboBoxService.GetUnits("Frequency");
            ForwardPowerUnits = ComboBoxService.GetUnits("Power");
            ReflectedPowerUnits = ComboBoxService.GetUnits("Power");
            _calculationService = new CalculationService();
            DataContext = this;
        }

        private void CalculateWavelength(object sender, RoutedEventArgs e)
        {
            if (!TryGetFrequency(out double frequency, out string frequencyUnit))
            {
                txtWavelengthResult.Text = "Invalid input";
                return;
            }

            try
            {
                frequency = UnitC.Conv.Frequency(frequency, frequencyUnit, "Hz");
                double wavelength = _calculationService.CalculateWavelength(frequency);
                txtWavelengthResult.Text = $"Wavelength: {FormatWavelength(wavelength)}";
            }
            catch (Exception ex)
            {
                txtWavelengthResult.Text = $"Error: {ex.Message}";
            }
        }


        private void CalculateVSWR(object sender, RoutedEventArgs e)
        {
            if (!TryGetPowerValues(out double forwardPower, out string forwardPowerUnit, out double reflectedPower, out string reflectedPowerUnit))
            {
                txtVSWRResult.Text = "Invalid input";
                return;
            }

            try
            {
                forwardPower = UnitC.Conv.Power(forwardPower, forwardPowerUnit, "W");
                reflectedPower = UnitC.Conv.Power(reflectedPower, reflectedPowerUnit, "W");

                double vswr = _calculationService.CalculateVSWR(forwardPower, reflectedPower);
                txtVSWRResult.Text = $"VSWR: {FormatVSWR(vswr)}";
            }
            catch (Exception ex)
            {
                txtVSWRResult.Text = $"Error: {ex.Message}";
            }
        }

        private bool TryGetFrequency(out double frequency, out string frequencyUnit)
        {
            frequency = 0;
            frequencyUnit = cmbFrequencyUnit.SelectedItem?.ToString();

            if (double.TryParse(txtFrequency.Text, out frequency) &&
                !string.IsNullOrEmpty(frequencyUnit))
            {
                return true;
            }

            return false;
        }

        private bool TryGetPowerValues(out double forwardPower, out string forwardPowerUnit, out double reflectedPower, out string reflectedPowerUnit)
        {
            forwardPower = 0;
            reflectedPower = 0;
            forwardPowerUnit = cmbForwardPowerUnit.SelectedItem?.ToString();
            reflectedPowerUnit = cmbReflectedPowerUnit.SelectedItem?.ToString();

            if (double.TryParse(txtForwardPower.Text, out forwardPower) &&
                double.TryParse(txtReflectedPower.Text, out reflectedPower) &&
                !string.IsNullOrEmpty(forwardPowerUnit) &&
                !string.IsNullOrEmpty(reflectedPowerUnit))
            {
                return true;
            }

            return false;
        }

        private string FormatWavelength(double wavelength)
        {
            return UnitC.Form.Length(wavelength);
        }

        private string FormatVSWR(double vswr)
        {
            return $"{vswr:F2}";
        }
    }
}
using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using static RadioEngineerCalculator.Services.UnitC;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class AttenuatorTab : UserControl
    {
        public ObservableCollection<string> InputVoltageUnits { get; set; }
        public ObservableCollection<string> OutputVoltageUnits { get; set; }

        private readonly CalculationService _calculationService;

        public AttenuatorTab()
        {
            InitializeComponent();
            InputVoltageUnits = ComboBoxService.GetUnits("Voltage");
            OutputVoltageUnits = ComboBoxService.GetUnits("Voltage");
            _calculationService = new CalculationService();
            DataContext = this;
        }

        private void CalculateAttenuator(object sender, RoutedEventArgs e)
        {
            if (!TryGetVoltageValues(out double inputVoltage, out string inputVoltageUnit, out double outputVoltage, out string outputVoltageUnit))
            {
                ResultTextBlock.Text = "Invalid input";
                return;
            }

            try
            {
                inputVoltage = Convert(inputVoltage, inputVoltageUnit, "V", PhysicalQuantity.Voltage);
                outputVoltage = Convert(outputVoltage, outputVoltageUnit, "V", PhysicalQuantity.Voltage);

                var result = _calculationService.CalculateAttenuator(inputVoltage, outputVoltage);
                ResultTextBlock.Text = $"Затухание: {FormatAttenuation(result)}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private bool TryGetVoltageValues(out double inputVoltage, out string inputVoltageUnit, out double outputVoltage, out string outputVoltageUnit)
        {
            inputVoltage = 0;
            outputVoltage = 0;
            inputVoltageUnit = InputVoltageUnitComboBox.SelectedItem?.ToString();
            outputVoltageUnit = OutputVoltageUnitComboBox.SelectedItem?.ToString();

            if (double.TryParse(InputVoltageTextBox.Text, out inputVoltage) &&
                double.TryParse(OutputVoltageTextBox.Text, out outputVoltage) &&
                !string.IsNullOrEmpty(inputVoltageUnit) &&
                !string.IsNullOrEmpty(outputVoltageUnit))
            {
                return true;
            }

            return false;
        }

        private string FormatAttenuation(double attenuation)
        {
            return $"{attenuation:F2} дБ";
        }
    }
}
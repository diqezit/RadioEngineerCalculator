using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class CoaxialCableTab : UserControl
    {
        public ObservableCollection<string> InnerDiameterUnits { get; set; }
        public ObservableCollection<string> OuterDiameterUnits { get; set; }

        private readonly CalculationService _calculationService;

        public CoaxialCableTab()
        {
            InitializeComponent();
            InnerDiameterUnits = ComboBoxService.GetUnits("Length");
            OuterDiameterUnits = ComboBoxService.GetUnits("Length");
            _calculationService = new CalculationService(); // Инициализация _calculationService
            DataContext = this;
        }

        private void CalculateCoaxialCable(object sender, RoutedEventArgs e)
        {
            if (!TryGetDiameterValues(out double innerDiameter, out string innerDiameterUnit, out double outerDiameter, out string outerDiameterUnit))
            {
                ResultTextBlock.Text = "Invalid input";
                return;
            }

            try
            {
                innerDiameter = UnitC.Conv.Length(innerDiameter, innerDiameterUnit, "m");
                outerDiameter = UnitC.Conv.Length(outerDiameter, outerDiameterUnit, "m");

                var result = _calculationService.CalculateCoaxialCable(innerDiameter, outerDiameter);
                ResultTextBlock.Text = $"Волновое сопротивление: {FormatImpedance(result)}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private bool TryGetDiameterValues(out double innerDiameter, out string innerDiameterUnit, out double outerDiameter, out string outerDiameterUnit)
        {
            innerDiameter = 0;
            outerDiameter = 0;
            innerDiameterUnit = InnerDiameterUnitComboBox.SelectedItem?.ToString();
            outerDiameterUnit = OuterDiameterUnitComboBox.SelectedItem?.ToString();

            if (double.TryParse(InnerDiameterTextBox.Text, out innerDiameter) &&
                double.TryParse(OuterDiameterTextBox.Text, out outerDiameter) &&
                !string.IsNullOrEmpty(innerDiameterUnit) &&
                !string.IsNullOrEmpty(outerDiameterUnit))
            {
                return true;
            }

            return false;
        }

        private string FormatImpedance(double impedance)
        {
            return $"{impedance:F2} Ом";
        }
    }
}
using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using static RadioEngineerCalculator.Services.UnitC;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class ImpedanceMatchingTab : UserControl
    {
        public ObservableCollection<string> SourceImpedanceUnits { get; set; }
        public ObservableCollection<string> LoadImpedanceUnits { get; set; }

        private readonly CalculationService _calculationService;

        public ImpedanceMatchingTab()
        {
            InitializeComponent();
            SourceImpedanceUnits = ComboBoxService.GetUnits("Resistance");
            LoadImpedanceUnits = ComboBoxService.GetUnits("Resistance");
            _calculationService = new CalculationService();
            DataContext = this;
        }

        private void CalculateImpedanceMatching(object sender, RoutedEventArgs e)
        {
            if (!TryGetImpedanceValues(out double sourceImpedance, out string sourceImpedanceUnit, out double loadImpedance, out string loadImpedanceUnit))
            {
                ResultTextBlock.Text = "Invalid input";
                return;
            }

            try
            {
                sourceImpedance = Convert(sourceImpedance, sourceImpedanceUnit, "Ω", PhysicalQuantity.Resistance);
                loadImpedance = Convert(loadImpedance, loadImpedanceUnit, "Ω", PhysicalQuantity.Resistance);

                var result = _calculationService.CalculateImpedanceMatching(sourceImpedance, loadImpedance);
                ResultTextBlock.Text = $"Согласование импедансов: {FormatImpedanceMatching(result)}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private bool TryGetImpedanceValues(out double sourceImpedance, out string sourceImpedanceUnit, out double loadImpedance, out string loadImpedanceUnit)
        {
            sourceImpedance = 0;
            loadImpedance = 0;
            sourceImpedanceUnit = SourceImpedanceUnitComboBox.SelectedItem?.ToString();
            loadImpedanceUnit = LoadImpedanceUnitComboBox.SelectedItem?.ToString();

            if (double.TryParse(SourceImpedanceTextBox.Text, out sourceImpedance) &&
                double.TryParse(LoadImpedanceTextBox.Text, out loadImpedance) &&
                !string.IsNullOrEmpty(sourceImpedanceUnit) &&
                !string.IsNullOrEmpty(loadImpedanceUnit))
            {
                return true;
            }

            return false;
        }

        private string FormatImpedanceMatching(double matchingValue)
        {
            return AutoFormat(matchingValue, PhysicalQuantity.Resistance);
        }
    }
}
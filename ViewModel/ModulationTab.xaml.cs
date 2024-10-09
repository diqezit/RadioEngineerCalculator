using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class ModulationTab : UserControl
    {
        public ObservableCollection<string> AmplitudeUnits { get; set; }
        public ObservableCollection<string> FrequencyUnits { get; set; }
        public ObservableCollection<string> AngleUnits { get; set; }

        private readonly CalculationService _calculationService;

        public ModulationTab()
        {
            InitializeComponent();
            AmplitudeUnits = ComboBoxService.GetUnits("Voltage");
            FrequencyUnits = ComboBoxService.GetUnits("Frequency");
            AngleUnits = ComboBoxService.GetUnits("Angle");
            _calculationService = new CalculationService();
            DataContext = this;
        }

        private void CalculateAMIndex(object sender, RoutedEventArgs e)
        {
            if (!TryGetAmplitudeValues(out double carrierAmplitude, out string carrierAmplitudeUnit, out double modulatingAmplitude, out string modulatingAmplitudeUnit))
            {
                txtAMIndexResult.Text = "Неверный ввод";
                return;
            }

            try
            {
                double amIndex = _calculationService.CalculateAMIndex(carrierAmplitude, carrierAmplitudeUnit, modulatingAmplitude, modulatingAmplitudeUnit);
                txtAMIndexResult.Text = $"Индекс модуляции AM: {amIndex:F2}";
            }
            catch (Exception ex)
            {
                txtAMIndexResult.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculateFMIndex(object sender, RoutedEventArgs e)
        {
            if (!TryGetFrequencyValues(out double carrierFrequency, out string carrierFrequencyUnit, out double frequencyDeviation, out string frequencyDeviationUnit))
            {
                txtFMIndexResult.Text = "Неверный ввод";
                return;
            }

            try
            {
                double fmIndex = _calculationService.CalculateFMIndex(carrierFrequency, carrierFrequencyUnit, frequencyDeviation, frequencyDeviationUnit);
                txtFMIndexResult.Text = $"Индекс модуляции FM: {fmIndex:F2}";
            }
            catch (Exception ex)
            {
                txtFMIndexResult.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculatePMIndex(object sender, RoutedEventArgs e)
        {
            if (!TryGetAngleValues(out double carrierPhase, out string carrierPhaseUnit, out double phaseDeviation, out string phaseDeviationUnit))
            {
                txtPMIndexResult.Text = "Неверный ввод";
                return;
            }

            try
            {
                double pmIndex = _calculationService.CalculatePMIndex(carrierPhase, carrierPhaseUnit, phaseDeviation, phaseDeviationUnit);
                txtPMIndexResult.Text = $"Индекс модуляции PM: {pmIndex:F2}";
            }
            catch (Exception ex)
            {
                txtPMIndexResult.Text = $"Ошибка: {ex.Message}";
            }
        }

        private bool TryGetAmplitudeValues(out double carrierAmplitude, out string carrierAmplitudeUnit, out double modulatingAmplitude, out string modulatingAmplitudeUnit)
        {
            carrierAmplitude = 0;
            modulatingAmplitude = 0;
            carrierAmplitudeUnit = cmbCarrierAmplitudeUnit.SelectedItem?.ToString();
            modulatingAmplitudeUnit = cmbModulatingAmplitudeUnit.SelectedItem?.ToString();

            if (double.TryParse(txtCarrierAmplitude.Text, out carrierAmplitude) &&
                double.TryParse(txtModulatingAmplitude.Text, out modulatingAmplitude) &&
                !string.IsNullOrEmpty(carrierAmplitudeUnit) &&
                !string.IsNullOrEmpty(modulatingAmplitudeUnit))
            {
                return true;
            }

            return false;
        }

        private bool TryGetFrequencyValues(out double carrierFrequency, out string carrierFrequencyUnit, out double frequencyDeviation, out string frequencyDeviationUnit)
        {
            carrierFrequency = 0;
            frequencyDeviation = 0;
            carrierFrequencyUnit = cmbCarrierFrequencyUnit.SelectedItem?.ToString();
            frequencyDeviationUnit = cmbFrequencyDeviationUnit.SelectedItem?.ToString();

            if (double.TryParse(txtCarrierFrequency.Text, out carrierFrequency) &&
                double.TryParse(txtFrequencyDeviation.Text, out frequencyDeviation) &&
                !string.IsNullOrEmpty(carrierFrequencyUnit) &&
                !string.IsNullOrEmpty(frequencyDeviationUnit))
            {
                return true;
            }

            return false;
        }

        private bool TryGetAngleValues(out double carrierPhase, out string carrierPhaseUnit, out double phaseDeviation, out string phaseDeviationUnit)
        {
            carrierPhase = 0;
            phaseDeviation = 0;
            carrierPhaseUnit = cmbCarrierPhaseUnit.SelectedItem?.ToString();
            phaseDeviationUnit = cmbPhaseDeviationUnit.SelectedItem?.ToString();

            if (double.TryParse(txtCarrierPhase.Text, out carrierPhase) &&
                double.TryParse(txtPhaseDeviation.Text, out phaseDeviation) &&
                !string.IsNullOrEmpty(carrierPhaseUnit) &&
                !string.IsNullOrEmpty(phaseDeviationUnit))
            {
                return true;
            }

            return false;
        }
    }
}
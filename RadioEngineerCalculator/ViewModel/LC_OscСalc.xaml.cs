using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Numerics;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class ResonantCircuitTab : UserControl, INotifyPropertyChanged
    {
        private readonly CalculationService _calculationService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> InductanceUnits { get; }
        public ObservableCollection<string> CapacitanceUnits { get; }
        public ObservableCollection<string> ResistanceUnits { get; }
        public ObservableCollection<string> FrequencyUnits { get; }

        #region Свойства для привязки данных
        private string _inductance;
        public string Inductance
        {
            get => _inductance;
            set { _inductance = value; OnPropertyChanged(nameof(Inductance)); }
        }

        private string _capacitance;
        public string Capacitance
        {
            get => _capacitance;
            set { _capacitance = value; OnPropertyChanged(nameof(Capacitance)); }
        }

        private string _resistance;
        public string Resistance
        {
            get => _resistance;
            set { _resistance = value; OnPropertyChanged(nameof(Resistance)); }
        }

        private string _frequency;
        public string Frequency
        {
            get => _frequency;
            set { _frequency = value; OnPropertyChanged(nameof(Frequency)); }
        }

        public string InductanceUnit { get; set; }
        public string CapacitanceUnit { get; set; }
        public string ResistanceUnit { get; set; }
        public string FrequencyUnit { get; set; }

        private string _resonanceFrequencyResult;
        public string ResonanceFrequencyResult
        {
            get => _resonanceFrequencyResult;
            set { _resonanceFrequencyResult = value; OnPropertyChanged(nameof(ResonanceFrequencyResult)); }
        }

        private string _seriesImpedanceResult;
        public string SeriesImpedanceResult
        {
            get => _seriesImpedanceResult;
            set { _seriesImpedanceResult = value; OnPropertyChanged(nameof(SeriesImpedanceResult)); }
        }

        private string _parallelImpedanceResult;
        public string ParallelImpedanceResult
        {
            get => _parallelImpedanceResult;
            set { _parallelImpedanceResult = value; OnPropertyChanged(nameof(ParallelImpedanceResult)); }
        }

        private string _seriesQFactorResult;
        public string SeriesQFactorResult
        {
            get => _seriesQFactorResult;
            set { _seriesQFactorResult = value; OnPropertyChanged(nameof(SeriesQFactorResult)); }
        }

        private string _parallelQFactorResult;
        public string ParallelQFactorResult
        {
            get => _parallelQFactorResult;
            set { _parallelQFactorResult = value; OnPropertyChanged(nameof(ParallelQFactorResult)); }
        }
        #endregion

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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool TryGetInputValues(out double inductance, out string inductanceUnit,
                                       out double capacitance, out string capacitanceUnit)
        {
            inductance = 0;
            capacitance = 0;
            inductanceUnit = InductanceUnit;
            capacitanceUnit = CapacitanceUnit;

            if (!double.TryParse(Inductance, out inductance) || inductance <= 0)
            {
                ShowErrorMessage(ErrorMessages.InvalidInductanceInput);
                return false;
            }

            if (!double.TryParse(Capacitance, out capacitance) || capacitance <= 0)
            {
                ShowErrorMessage(ErrorMessages.InvalidCapacitanceInput);
                return false;
            }

            if (string.IsNullOrEmpty(inductanceUnit) || string.IsNullOrEmpty(capacitanceUnit))
            {
                ShowErrorMessage(ErrorMessages.InvalidInput);
                return false;
            }

            return true;
        }

        private void CalculateResonanceFrequency(object sender, RoutedEventArgs e)
        {
            if (!TryGetInputValues(out double inductance, out string inductanceUnit,
                                   out double capacitance, out string capacitanceUnit))
            {
                return;
            }

            try
            {
                inductance = UnitC.Conv.Inductance(inductance, inductanceUnit, "H");
                capacitance = UnitC.Conv.Capacitance(capacitance, capacitanceUnit, "F");

                double result = _calculationService.CalculateResonanceFrequency(inductance, capacitance);
                ResonanceFrequencyResult = $"Резонансная частота: {FormatFrequency(result)}";
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CalculateImpedance(object sender, RoutedEventArgs e)
        {
            if (!TryGetDoubleValue(Resistance, out double resistance) ||
                !TryGetDoubleValue(Inductance, out double inductance) ||
                !TryGetDoubleValue(Capacitance, out double capacitance) ||
                !TryGetDoubleValue(Frequency, out double frequency))
            {
                ShowErrorMessage(ErrorMessages.InvalidInput);
                return;
            }

            try
            {
                resistance = UnitC.Conv.Resistance(resistance, ResistanceUnit, "Ω");
                inductance = UnitC.Conv.Inductance(inductance, InductanceUnit, "H");
                capacitance = UnitC.Conv.Capacitance(capacitance, CapacitanceUnit, "F");
                frequency = UnitC.Conv.Frequency(frequency, FrequencyUnit, "Hz");

                Complex seriesImpedance = _calculationService.CalculateSeriesImpedance(resistance, inductance, capacitance, frequency);
                Complex parallelImpedance = _calculationService.CalculateParallelImpedance(resistance, inductance, capacitance, frequency);

                SeriesImpedanceResult = $"Последовательный импеданс: {FormatComplex(seriesImpedance)} Ом";
                ParallelImpedanceResult = $"Параллельный импеданс: {FormatComplex(parallelImpedance)} Ом";
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void CalculateQFactor(object sender, RoutedEventArgs e)
        {
            if (!TryGetDoubleValue(Inductance, out double inductance) ||
                !TryGetDoubleValue(Resistance, out double resistance) ||
                !TryGetDoubleValue(Frequency, out double frequency))
            {
                ShowErrorMessage(ErrorMessages.InvalidInput);
                return;
            }

            try
            {
                inductance = UnitC.Conv.Inductance(inductance, InductanceUnit, "H");
                resistance = UnitC.Conv.Resistance(resistance, ResistanceUnit, "Ohm");
                frequency = UnitC.Conv.Frequency(frequency, FrequencyUnit, "Hz");

                double seriesQFactor = _calculationService.CalculateSeriesQFactor(inductance, resistance, frequency);
                double parallelQFactor = _calculationService.CalculateParallelQFactor(inductance, resistance, frequency);

                SeriesQFactorResult = $"Последовательный Q-фактор: {seriesQFactor:F2}";
                ParallelQFactorResult = $"Параллельный Q-фактор: {parallelQFactor:F2}";
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private bool TryGetDoubleValue(string input, out double result)
        {
            return double.TryParse(input, out result) && result > 0;
        }

        private string FormatFrequency(double frequency) => UnitC.Form.Frequency(frequency);

        private string FormatComplex(Complex complex) => $"{complex.Real:F2} + {complex.Imaginary:F2}j";

        private void ShowErrorMessage(string message) =>
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
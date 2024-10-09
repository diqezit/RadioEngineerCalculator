using OxyPlot;
using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using static RadioEngineerCalculator.Services.FiltersCalculationService;


namespace RadioEngineerCalculator.ViewModel
{
    public partial class LRCFilterTab : UserControl
    {
        public ObservableCollection<string> CapacitanceUnits { get; }
        public ObservableCollection<string> InductanceUnits { get; }
        public ObservableCollection<string> ResistanceUnits { get; }
        public ObservableCollection<string> FrequencyUnits { get; }
        public ObservableCollection<string> FilterTypes { get; }

        private FilterType selectedFilterType;
        private readonly CalculationService _calculationService;
        private readonly FiltersCalculationService _filtersCalculationService;
        private PlotModel _filterResponseModel;
        private Graph _graph;

        private double _capacitance;
        private double _inductance;
        private double _resistance;
        private double _frequency;


        public LRCFilterTab()
        {
            InitializeComponent();
            CapacitanceUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Capacitance"));
            InductanceUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Inductance"));
            ResistanceUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Resistance"));
            FrequencyUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Frequency"));
            FilterTypes = new ObservableCollection<string>(Enum.GetNames(typeof(FilterType)));
            _calculationService = new CalculationService();

            // Инициализация _filterResponseModel перед _graph
            _filterResponseModel = new PlotModel { Title = "Характеристика фильтра" };
            _filtersCalculationService = new FiltersCalculationService();
            _graph = new Graph(_filterResponseModel, _filtersCalculationService);

            FilterResponsePlot.Model = _filterResponseModel;
            DataContext = this;

            cmbFilterType.SelectionChanged += OnFilterTypeChanged;

            // Установить значения по умолчанию для ComboBox
            cmbFilterType.SelectedIndex = 0;
            cmbFrequencyUnit.SelectedIndex = 0;
            cmbCapacitanceUnit.SelectedIndex = 0;
            cmbInductanceUnit.SelectedIndex = 0;
            cmbResistanceUnit.SelectedIndex = 0;
        }


        private void CalculateFilterParameters(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateInputValues();

                if (!TryGetInputValues(out var inputValues))
                {
                    return;
                }

                var results = CalculateFilterResults(inputValues);
                DisplayResults(results);
                _graph.UpdateFilterResponsePlot(results); // Обновление графика через новый класс
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool TryGetInputValues(out FilterInputValues inputValues)
        {
            inputValues = new FilterInputValues();

            if (!double.TryParse(txtFrequency.Text, out _frequency) ||
                cmbFilterType.SelectedItem == null ||
                cmbFrequencyUnit.SelectedItem == null)
            {
                MessageBox.Show(ErrorMessages.InvalidFrequencyInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var selectedFilterType = cmbFilterType.SelectedItem.ToString();
            if (!Enum.TryParse(selectedFilterType, out FilterType filterType))
            {
                MessageBox.Show(ErrorMessages.InvalidFilterType, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            inputValues.Frequency = _frequency;
            inputValues.FilterType = filterType;
            inputValues.FrequencyUnit = cmbFrequencyUnit.SelectedItem.ToString();

            if (!TryParseComponentValues(filterType, out _capacitance, out _inductance, out _resistance))
            {
                return false;
            }

            inputValues.Capacitance = _capacitance;
            inputValues.Inductance = _inductance;
            inputValues.Resistance = _resistance;
            inputValues.CapacitanceUnit = cmbCapacitanceUnit.SelectedItem?.ToString() ?? "F";
            inputValues.InductanceUnit = cmbInductanceUnit.SelectedItem?.ToString() ?? "H";
            inputValues.ResistanceUnit = cmbResistanceUnit.SelectedItem?.ToString() ?? "Ω";

            return true;
        }

        private bool TryParseComponentValues(FilterType filterType, out double capacitance, out double inductance, out double resistance)
        {
            capacitance = inductance = resistance = 0;

            bool isValid = true;

            if (filterType != FilterType.RL && (!double.TryParse(txtCapacitance.Text, out capacitance) || cmbCapacitanceUnit.SelectedItem == null || capacitance <= 0))
            {
                MessageBox.Show(ErrorMessages.InvalidCapacitanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (filterType != FilterType.RC && (!double.TryParse(txtInductance.Text, out inductance) || cmbInductanceUnit.SelectedItem == null || inductance <= 0))
            {
                MessageBox.Show(ErrorMessages.InvalidInductanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (filterType != FilterType.Quartz && (!double.TryParse(txtResistance.Text, out resistance) || cmbResistanceUnit.SelectedItem == null || resistance <= 0))
            {
                MessageBox.Show(ErrorMessages.InvalidResistanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            return isValid;
        }


        private FilterResults CalculateFilterResults(FilterInputValues inputValues)
        {
            ValidateInputValues(inputValues);

            double capacitance = UnitC.Convert(inputValues.Capacitance, inputValues.CapacitanceUnit, "F", UnitC.PhysicalQuantity.Capacitance);
            double inductance = UnitC.Convert(inputValues.Inductance, inputValues.InductanceUnit, "H", UnitC.PhysicalQuantity.Inductance);
            double resistance = UnitC.Convert(inputValues.Resistance, inputValues.ResistanceUnit, "Ω", UnitC.PhysicalQuantity.Resistance);
            double frequency = UnitC.Convert(inputValues.Frequency, inputValues.FrequencyUnit, "Hz", UnitC.PhysicalQuantity.Frequency);

            if (!AreValuesPositive(capacitance, inductance, resistance, frequency))
            {
                throw new ArgumentException(ErrorMessages.InvalidComponentValue);
            }

            var results = new FilterResults
            {
                FilterType = inputValues.FilterType,
                CutoffFrequency = _filtersCalculationService.CalculateFilterCutoffFrequency(inputValues.FilterType, resistance, inductance, capacitance),
                QualityFactor = _filtersCalculationService.CalculateQualityFactor(inputValues.FilterType, inductance, capacitance, resistance),
                Impedance = _filtersCalculationService.CalculateFilterImpedance(inputValues.FilterType, resistance, inductance, capacitance, frequency)
            };

            results.Bandwidth = results.CutoffFrequency / (results.QualityFactor > 0 ? results.QualityFactor : 1);
            results.PhaseShift = _filtersCalculationService.CalculateFilterPhaseResponse(inputValues.FilterType, frequency, results.CutoffFrequency, results.Bandwidth);
            results.GroupDelay = _filtersCalculationService.CalculateGroupDelay(inputValues.FilterType, frequency, results.CutoffFrequency, results.Bandwidth);
            results.Attenuation = _filtersCalculationService.CalculateFilterAttenuation(inputValues.FilterType, frequency, results.CutoffFrequency, results.Bandwidth);

            return results;
        }


        private void ValidateInputValues(FilterInputValues inputValues)
        {
            if (inputValues == null)
            {
                throw new ArgumentException(ErrorMessages.InvalidInputValues);
            }

            if (string.IsNullOrEmpty(inputValues.FrequencyUnit) || string.IsNullOrEmpty(inputValues.CapacitanceUnit) ||
                string.IsNullOrEmpty(inputValues.InductanceUnit) || string.IsNullOrEmpty(inputValues.ResistanceUnit))
            {
                throw new ArgumentException(ErrorMessages.InvalidInputValues);
            }

            if (!Enum.IsDefined(typeof(FilterType), inputValues.FilterType))
            {
                throw new ArgumentException(ErrorMessages.InvalidFilterType);
            }
        }

        private bool AreValuesPositive(params double[] values)
        {
            foreach (var value in values)
            {
                if (value <= 0)
                    return false;
            }
            return true;
        }



        private void DisplayResults(FilterResults results)
        {
            txtCutoffFrequencyResult.Text = $"Частота среза: {UnitC.Form.Frequency(results.CutoffFrequency)}";
            txtQualityFactorResult.Text = $"Добротность (Q): {results.QualityFactor:F2}";
            txtBandwidthResult.Text = $"Полоса пропускания: {UnitC.Form.Frequency(results.Bandwidth)}";
            txtImpedanceResult.Text = $"Импеданс: {UnitC.Form.Resistance(results.Impedance)}";
            txtPhaseShiftResult.Text = $"Сдвиг фазы: {UnitC.Form.Angle(results.PhaseShift)}";
            txtGroupDelayResult.Text = $"Групповая задержка: {UnitC.Form.Time(results.GroupDelay)}";
            txtAttenuationResult.Text = $"Ослабление: {results.Attenuation:F2} дБ";

            txtAdditionalInfo.Text = Info.GetAdditionalInfo(results.FilterType, results.CutoffFrequency, results.Bandwidth);

            // Показать график, если результаты есть
            if (_filterResponseModel.Series.Count > 0)
            {
                FilterResponsePlot.Visibility = Visibility.Visible; // Показать график
            }
        }


        private void OnFilterTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFilterType.SelectedItem != null)
            {
                if (Enum.TryParse(cmbFilterType.SelectedItem.ToString(), out FilterType filterType))
                {
                    UpdateInputFieldsVisibility(filterType);
                    ResetInputFields();
                    ClearResults(); // Очистить результаты при изменении типа фильтра
                    UpdateHelpText(filterType);
                }
                else
                {
                    MessageBox.Show(ErrorMessages.InvalidFilterType, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void UpdateInputFieldsVisibility(FilterType filterType)
        {
            var visibilityMap = new Dictionary<FilterType, (bool capacitance, bool inductance, bool resistance)>
    {
        { FilterType.RC, (true, false, true) },
        { FilterType.RL, (false, true, true) },
        { FilterType.Quartz, (true, true, false) },
        { FilterType.LowPass, (true, true, true) },
        { FilterType.HighPass, (true, true, true) },
        { FilterType.BandPass, (true, true, true) },
        { FilterType.BandStop, (true, true, true) },
        { FilterType.PassiveBandPass, (true, true, true) },
        { FilterType.PassiveBandStop, (true, true, true) }
    };

            DisableAllInputFields(); // Сначала отключить все поля

            if (visibilityMap.TryGetValue(filterType, out var visibility))
            {
                txtCapacitance.IsEnabled = visibility.capacitance;
                cmbCapacitanceUnit.IsEnabled = visibility.capacitance;
                txtInductance.IsEnabled = visibility.inductance;
                cmbInductanceUnit.IsEnabled = visibility.inductance;
                txtResistance.IsEnabled = visibility.resistance;
                cmbResistanceUnit.IsEnabled = visibility.resistance;

                // Установка фокуса на первое доступное поле ввода
                if (visibility.capacitance) txtCapacitance.Focus();
                else if (visibility.inductance) txtInductance.Focus();
                else if (visibility.resistance) txtResistance.Focus();
            }
        }




        private void DisableAllInputFields()
        {
            txtCapacitance.IsEnabled = false;
            cmbCapacitanceUnit.IsEnabled = false;
            txtInductance.IsEnabled = false;
            cmbInductanceUnit.IsEnabled = false;
            txtResistance.IsEnabled = false;
            cmbResistanceUnit.IsEnabled = false;
        }


        private void ValidateInputValues()
        {
            if (cmbFilterType.SelectedItem == null || cmbFrequencyUnit.SelectedItem == null ||
                cmbCapacitanceUnit.SelectedItem == null || cmbInductanceUnit.SelectedItem == null ||
                cmbResistanceUnit.SelectedItem == null)
            {
                throw new ArgumentException(ErrorMessages.InvalidInputValues);
            }

            string selectedFilterType = cmbFilterType.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedFilterType) || !Enum.IsDefined(typeof(FilterType), selectedFilterType))
            {
                throw new ArgumentException(ErrorMessages.InvalidFilterType);
            }

            if (!double.TryParse(txtFrequency.Text, out var frequency) || frequency <= 0)
            {
                throw new ArgumentException(ErrorMessages.InvalidFrequencyValue);
            }

            FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), selectedFilterType);

            if (filterType != FilterType.RL && (!double.TryParse(txtCapacitance.Text, out var capacitance) || capacitance <= 0))
            {
                throw new ArgumentException(ErrorMessages.InvalidCapacitanceValue);
            }

            if (filterType != FilterType.RC && (!double.TryParse(txtInductance.Text, out var inductance) || inductance <= 0))
            {
                throw new ArgumentException(ErrorMessages.InvalidInductanceValue);
            }

            if (filterType != FilterType.Quartz && (!double.TryParse(txtResistance.Text, out var resistance) || resistance <= 0))
            {
                throw new ArgumentException(ErrorMessages.InvalidResistanceValue);
            }
        }



        // New method to reset input fields
        private void ResetInputFields()
        {
            txtCapacitance.Text = string.Empty;
            txtInductance.Text = string.Empty;
            txtResistance.Text = string.Empty;
            txtFrequency.Text = string.Empty;

            cmbCapacitanceUnit.SelectedIndex = 0;
            cmbInductanceUnit.SelectedIndex = 0;
            cmbResistanceUnit.SelectedIndex = 0;
            cmbFrequencyUnit.SelectedIndex = 0;
        }

        private void ClearResults()
        {
            txtCutoffFrequencyResult.Text = string.Empty;
            txtQualityFactorResult.Text = string.Empty;
            txtBandwidthResult.Text = string.Empty;
            txtImpedanceResult.Text = string.Empty;
            txtPhaseShiftResult.Text = string.Empty;
            txtGroupDelayResult.Text = string.Empty;
            txtAttenuationResult.Text = string.Empty;
            txtAdditionalInfo.Text = string.Empty;

            _filterResponseModel.Series.Clear();
            _filterResponseModel.InvalidatePlot(true);

            // Скрыть график, если результатов нет
            if (_filterResponseModel.Series.Count == 0)
            {
                FilterResponsePlot.Visibility = Visibility.Collapsed; // Скрыть график
            }
        }


        //// New method to handle the Reset button click
        //private void ResetButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ResetInputFields();
        //    ClearResults();
        //}

        private void UpdateHelpText(FilterType filterType)
        {
            switch (filterType)
            {
                case FilterType.LowPass:
                    txtHelpText.Text = Info.LowPassFilterDescription;
                    break;
                case FilterType.HighPass:
                    txtHelpText.Text = Info.HighPassFilterDescription;
                    break;
                case FilterType.BandPass:
                case FilterType.PassiveBandPass:
                    txtHelpText.Text = Info.BandPassFilterDescription;
                    break;
                case FilterType.BandStop:
                case FilterType.PassiveBandStop:
                    txtHelpText.Text = Info.BandStopFilterDescription;
                    break;
                case FilterType.RC:
                    txtHelpText.Text = Info.RCFilterDescription;
                    break;
                case FilterType.RL:
                    txtHelpText.Text = Info.RLFilterDescription;
                    break;
                case FilterType.Quartz:
                    txtHelpText.Text = Info.QuartzFilterDescription;
                    break;
                default:
                    txtHelpText.Text = "Выберите тип фильтра, чтобы просмотреть дополнительную информацию.";
                    break;
            }
        }
    }
}
using OxyPlot;
using OxyPlot.Series;
using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using static RadioEngineerCalculator.Services.FiltersCalculationService;


namespace RadioEngineerCalculator.ViewModel
{
    public partial class LRCFilterTab : UserControl, INotifyPropertyChanged
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
        private double _passbandRipple;
        private double _stopbandAttenuation;
        private double _stopbandFrequency;

        private string _selectedCapacitanceUnit;
        private string _selectedFilterType;
        private string _selectedFrequencyUnit;
        private string _selectedInductanceUnit;
        private string _selectedResistanceUnit;

        public string SelectedCapacitanceUnit
        {
            get => _selectedCapacitanceUnit;
            set => SetProperty(ref _selectedCapacitanceUnit, value);
        }

        public string SelectedFilterType
        {
            get => _selectedFilterType;
            set => SetProperty(ref _selectedFilterType, value);
        }

        public string SelectedFrequencyUnit
        {
            get => _selectedFrequencyUnit;
            set => SetProperty(ref _selectedFrequencyUnit, value);
        }

        public string SelectedInductanceUnit
        {
            get => _selectedInductanceUnit;
            set => SetProperty(ref _selectedInductanceUnit, value);
        }

        public string SelectedResistanceUnit
        {
            get => _selectedResistanceUnit;
            set => SetProperty(ref _selectedResistanceUnit, value);
        }

        public double PassbandRipple
        {
            get => _passbandRipple;
            set => SetProperty(ref _passbandRipple, value);
        }

        public double StopbandAttenuation
        {
            get => _stopbandAttenuation;
            set => SetProperty(ref _stopbandAttenuation, value);
        }

        public double StopbandFrequency
        {
            get => _stopbandFrequency;
            set => SetProperty(ref _stopbandFrequency, value);
        }

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        public LRCFilterTab()
        {
            InitializeComponent();
            CapacitanceUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Capacitance"));
            InductanceUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Inductance"));
            ResistanceUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Resistance"));
            FrequencyUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Frequency"));
            FilterTypes = new ObservableCollection<string>(Enum.GetNames(typeof(FilterType)));

            _calculationService = new CalculationService();
            _filtersCalculationService = new FiltersCalculationService();
            _filterResponseModel = new PlotModel { Title = "Амплитудно-частотная характеристика" };
            _graph = new Graph(_filterResponseModel, _filtersCalculationService);

            FilterResponsePlot.Model = _filterResponseModel;
            DataContext = this;

            txtCapacitance.TextChanged += OnParameterChanged;
            txtInductance.TextChanged += OnParameterChanged;
            txtResistance.TextChanged += OnParameterChanged;
            txtFrequency.TextChanged += OnParameterChanged;
            cmbCapacitanceUnit.SelectionChanged += OnUnitChanged;
            cmbInductanceUnit.SelectionChanged += OnUnitChanged;
            cmbResistanceUnit.SelectionChanged += OnUnitChanged;
            cmbFrequencyUnit.SelectionChanged += OnUnitChanged;
            cmbFilterType.SelectionChanged += OnFilterTypeChanged;
            InitializeComboBoxes();
        }

        private void OnParameterChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                switch (textBox.Name)
                {
                    case "txtCapacitance":
                        AutoCalculateParameters(CalculationType.Capacitance);
                        break;
                    case "txtInductance":
                        AutoCalculateParameters(CalculationType.Inductance);
                        break;
                    case "txtResistance":
                        AutoCalculateParameters(CalculationType.Resistance);
                        break;
                    case "txtFrequency":
                        AutoCalculateParameters(CalculationType.Frequency);
                        break;
                }
            }
        }

        private enum CalculationType
        {
            Capacitance,
            Inductance,
            Resistance,
            Frequency
        }



        private void OnUnitChanged(object sender, SelectionChangedEventArgs e)
        {
            AutoCalculateParameters();
        }

        private void AutoCalculateParameters(CalculationType calcType = CalculationType.Frequency)
        {
            if (cmbFilterType.SelectedItem == null) return;

            var filterType = (FilterType)Enum.Parse(typeof(FilterType), cmbFilterType.SelectedItem.ToString());

            if (TryGetInputValues(out var inputValues))
            {
                try
                {
                    switch (calcType)
                    {
                        case CalculationType.Capacitance:
                            inputValues.Inductance = _filtersCalculationService.RecalculateInductance(inputValues);
                            inputValues.Resistance = _filtersCalculationService.RecalculateResistance(inputValues);
                            break;
                        case CalculationType.Inductance:
                            inputValues.Capacitance = _filtersCalculationService.RecalculateCapacitance(inputValues);
                            inputValues.Resistance = _filtersCalculationService.RecalculateResistance(inputValues);
                            break;
                        case CalculationType.Resistance:
                            inputValues.Capacitance = _filtersCalculationService.RecalculateCapacitance(inputValues);
                            inputValues.Inductance = _filtersCalculationService.RecalculateInductance(inputValues);
                            break;
                        case CalculationType.Frequency:
                            inputValues.Capacitance = _filtersCalculationService.RecalculateCapacitance(inputValues);
                            inputValues.Inductance = _filtersCalculationService.RecalculateInductance(inputValues);
                            inputValues.Resistance = _filtersCalculationService.RecalculateResistance(inputValues);
                            break;
                    }

                    UpdateUIWithCalculatedValues(inputValues);

                    var results = _filtersCalculationService.CalculateFilterResults(inputValues);
                    UpdateUIWithResults(results);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ErrorMessages.EAC}\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateUIWithCalculatedValues(FilterInputValues inputValues)
        {
            txtCapacitance.Text = UnitC.Form.Capacitance(inputValues.Capacitance);
            txtInductance.Text = UnitC.Form.Inductance(inputValues.Inductance);
            txtResistance.Text = UnitC.Form.Resistance(inputValues.Resistance);
            txtFrequency.Text = UnitC.Form.Frequency(inputValues.Frequency);
        }

        private void UpdateUIWithResults(FilterResults results)
        {
            // Обновляем UI с новыми результатами
            txtCutoffFrequencyResult.Text = $"Частота среза: {UnitC.Form.Frequency(results.CutoffFrequency)}";
            txtQualityFactorResult.Text = $"Добротность (Q): {results.QualityFactor:F2}";
            txtBandwidthResult.Text = $"Полоса пропускания: {UnitC.Form.Frequency(results.Bandwidth)}";
            txtImpedanceResult.Text = $"Импеданс: {UnitC.Form.Resistance(results.Impedance)}";
            txtPhaseShiftResult.Text = $"Сдвиг фазы: {UnitC.Form.Angle(results.PhaseShift)}";
            txtGroupDelayResult.Text = $"Групповая задержка: {UnitC.Form.Time(results.GroupDelay)}";
            txtAttenuationResult.Text = $"Ослабление: {results.Attenuation:F2} дБ";
            txtFilterOrderResult.Text = $"Порядок фильтра: {results.FilterOrder}";

            UpdateFilterResponsePlot(results);
        }



        private void InitializeComboBoxes()
        {
            cmbFilterType.SelectedIndex = 0;
            cmbFrequencyUnit.SelectedIndex = 0;
            cmbCapacitanceUnit.SelectedIndex = 0;
            cmbInductanceUnit.SelectedIndex = 0;
            cmbResistanceUnit.SelectedIndex = 0;
        }

        private void CalculateFilterParameters(object sender, RoutedEventArgs e)
        {
            AutoCalculateParameters();
        }


        private void UpdateFilterResponsePlot(FilterResults results)
        {
            _filterResponseModel.Series.Clear();
            var frequencyRange = Enumerable.Range(1, 1000).Select(i => i * results.CutoffFrequency / 1000.0).ToArray();
            var magnitudeSeries = new LineSeries { Title = "АЧХ" };
            var phaseSeries = new LineSeries { Title = "ФЧХ" };

            foreach (var freq in frequencyRange)
            {
                (double magnitude, double phase) = CalculateResponse(freq, results);
                magnitudeSeries.Points.Add(new DataPoint(freq, 20 * Math.Log10(magnitude)));
                phaseSeries.Points.Add(new DataPoint(freq, phase * 180 / Math.PI));
            }

            _filterResponseModel.Series.Add(magnitudeSeries);
            _filterResponseModel.Series.Add(phaseSeries);
            _filterResponseModel.InvalidatePlot(true);
        }

        private (double magnitude, double phase) CalculateResponse(double freq, FilterResults results)
        {
            switch (results.FilterType)
            {
                case FilterType.LowPass:
                    return (_filtersCalculationService.CalculateLowPassMagnitudeResponse(freq, results.CutoffFrequency),
                            _filtersCalculationService.CalculateLowPassPhaseResponse(freq, results.CutoffFrequency));
                case FilterType.HighPass:
                    return (_filtersCalculationService.CalculateHighPassMagnitudeResponse(freq, results.CutoffFrequency),
                            _filtersCalculationService.CalculateHighPassPhaseResponse(freq, results.CutoffFrequency));
                case FilterType.BandPass:
                    return (_filtersCalculationService.CalculateBandPassMagnitudeResponse(freq, results.CutoffFrequency, results.Bandwidth),
                            _filtersCalculationService.CalculateBandPassPhaseResponse(freq, results.CutoffFrequency, results.Bandwidth));
                case FilterType.BandStop:
                    return (_filtersCalculationService.CalculateBandStopMagnitudeResponse(freq, results.CutoffFrequency, results.Bandwidth),
                            _filtersCalculationService.CalculateBandStopPhaseResponse(freq, results.CutoffFrequency, results.Bandwidth));
                default:
                    return (0, 0);
            }
        }

        private bool TryGetInputValues(out FilterInputValues inputValues)
        {
            inputValues = new FilterInputValues();

            if (!double.TryParse(txtFrequency.Text, out _frequency) ||
                !double.TryParse(txtPassbandRipple.Text, out _passbandRipple) ||
                !double.TryParse(txtStopbandAttenuation.Text, out _stopbandAttenuation) ||
                !double.TryParse(txtStopbandFrequency.Text, out _stopbandFrequency) ||
                cmbFilterType.SelectedItem == null ||
                cmbFrequencyUnit.SelectedItem == null ||
                cmbStopbandFrequencyUnit.SelectedItem == null)
            {
                MessageBox.Show(ErrorMessages.InvalidInputValues, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var selectedFilterType = cmbFilterType.SelectedItem.ToString();
            if (!Enum.TryParse(selectedFilterType, out FilterType filterType))
            {
                MessageBox.Show(ErrorMessages.InvalidFilterType, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Дополнительная проверка валидности значений
            if (!InputValidator.ValidateInputValues(inputValues))
            {
                return false; // Если валидация не прошла, выходим из метода
            }

            // Установка значений в inputValues
            inputValues.Frequency = _frequency;
            inputValues.FilterType = filterType;
            inputValues.FrequencyUnit = cmbFrequencyUnit.SelectedItem.ToString();
            inputValues.PassbandRipple = _passbandRipple;
            inputValues.StopbandAttenuation = _stopbandAttenuation;
            inputValues.StopbandFrequency = _stopbandFrequency;
            inputValues.StopbandFrequencyUnit = cmbStopbandFrequencyUnit.SelectedItem.ToString();

            // Валидация компонентных значений
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

            // Валидация inputValues
            return InputValidator.ValidateInputValues(inputValues);
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


        public static bool AreValuesPositive(params double[] values)
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
            txtFilterOrderResult.Text = $"Порядок фильтра: {results.FilterOrder}";
            _filterResponseModel.Title = $"Амплитудно-частотная характеристика (Порядок: {results.FilterOrder})";

            txtAdditionalInfo.Text = Info.GetAdditionalInfo(results.FilterType, results.CutoffFrequency, results.Bandwidth);

            if (_filterResponseModel.Series.Count > 0)
            {
                FilterResponsePlot.Visibility = Visibility.Visible;
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
                    ClearResults();
                    UpdateHelpText(filterType);
                    UpdateAdditionalInputFields(filterType);
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
                { FilterType.PassiveBandStop, (true, true, true) },
                { FilterType.Pi, (true, true, true) },
                { FilterType.Trap, (true, true, true) }
            };

            DisableAllInputFields();

            if (visibilityMap.TryGetValue(filterType, out var visibility))
            {
                txtCapacitance.IsEnabled = visibility.capacitance;
                cmbCapacitanceUnit.IsEnabled = visibility.capacitance;
                txtInductance.IsEnabled = visibility.inductance;
                cmbInductanceUnit.IsEnabled = visibility.inductance;
                txtResistance.IsEnabled = visibility.resistance;
                cmbResistanceUnit.IsEnabled = visibility.resistance;

                if (visibility.capacitance) txtCapacitance.Focus();
                else if (visibility.inductance) txtInductance.Focus();
                else if (visibility.resistance) txtResistance.Focus();
            }
        }

        private void UpdateAdditionalInputFields(FilterType filterType)
        {
            bool showAdditionalFields = filterType == FilterType.LowPass || filterType == FilterType.HighPass ||
                                        filterType == FilterType.BandPass || filterType == FilterType.BandStop;

            txtPassbandRipple.Visibility = showAdditionalFields ? Visibility.Visible : Visibility.Collapsed;
            txtStopbandAttenuation.Visibility = showAdditionalFields ? Visibility.Visible : Visibility.Collapsed;
            txtStopbandFrequency.Visibility = showAdditionalFields ? Visibility.Visible : Visibility.Collapsed;
            cmbStopbandFrequencyUnit.Visibility = showAdditionalFields ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetInputFields();
            ClearResults();
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
            FilterResponsePlot.InvalidatePlot(true);

            // Скрыть график, если результатов нет
            if (_filterResponseModel.Series.Count == 0)
            {
                FilterResponsePlot.Visibility = Visibility.Collapsed; // Скрыть график
            }
        }

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
                    txtHelpText.Text = Info.BandPassFilterDescription;
                    break;
                case FilterType.BandStop:
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
                case FilterType.Pi:
                    txtHelpText.Text = Info.PiFilterDescription;
                    break;
                case FilterType.Trap:
                    txtHelpText.Text = Info.TrapFilterDescription;
                    break;
                default:
                    txtHelpText.Text = "Выберите тип фильтра, чтобы просмотреть дополнительную информацию.";
                    break;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class AmplifierTab : UserControl, INotifyPropertyChanged
    {
        #region Properties
        private readonly CalculationService _calculationService;
        private string _gainResult;
        private string _noiseFigureResult;
        private string _efficiencyResult;
        private string _compressionPointResult;
        private string _ip3Result;
        private bool _canCalculate = false;

        public ICommand CalculateGainCommand { get; private set; }
        public ICommand CalculateNoiseFigureCommand { get; private set; }
        public ICommand CalculateEfficiencyCommand { get; private set; }
        public ICommand Calculate1dBCompressionPointCommand { get; private set; }
        public ICommand CalculateIP3Command { get; private set; }

        public ObservableCollection<string> PowerInUnits { get; set; }
        public ObservableCollection<string> PowerOutUnits { get; set; }
        #endregion

        #region Public Properties
        public bool CanCalculate
        {
            get => _canCalculate;
            set => SetProperty(ref _canCalculate, value);
        }

        public string GainResult
        {
            get => _gainResult;
            set => SetProperty(ref _gainResult, value);
        }

        public string NoiseFigureResult
        {
            get => _noiseFigureResult;
            set => SetProperty(ref _noiseFigureResult, value);
        }

        public string EfficiencyResult
        {
            get => _efficiencyResult;
            set => SetProperty(ref _efficiencyResult, value);
        }

        public string CompressionPointResult
        {
            get => _compressionPointResult;
            set => SetProperty(ref _compressionPointResult, value);
        }

        public string IP3Result
        {
            get => _ip3Result;
            set => SetProperty(ref _ip3Result, value);
        }
        #endregion

        #region Constructor
        public AmplifierTab()
        {
            InitializeComponent();
            _calculationService = new CalculationService();
            InitializeCollections();
            InitializeCommands();
            DataContext = this;
        }
        #endregion

        #region Initialization Methods
        private void InitializeCollections()
        {
            PowerInUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Power"));
            PowerOutUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Power"));
        }

        private void InitializeCommands()
        {
            CalculateGainCommand = new RelayCommand(CalculateGain, () => CanCalculate);
            CalculateNoiseFigureCommand = new RelayCommand(CalculateNoiseFigure, () => CanCalculate);
            CalculateEfficiencyCommand = new RelayCommand(CalculateEfficiency, () => CanCalculate);
            Calculate1dBCompressionPointCommand = new RelayCommand(Calculate1dBCompressionPoint, () => CanCalculate);
            CalculateIP3Command = new RelayCommand(CalculateIP3, () => CanCalculate);
        }

        #endregion

        #region Event Handlers
        private void OnParameterChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!double.TryParse(textBox.Text, out _))
                {
                    MessageBox.Show(ErrorMessages.InvalidInputFormat, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            UpdateCanCalculate();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem == null)
            {
                MessageBox.Show(ErrorMessages.CheckComboBox, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            UpdateCanCalculate();
        }

        private void UpdateCanCalculate()
        {
            CanCalculate = Validate.ValidateInputs(
                txtPowerIn.Text, txtPowerOut.Text, txtNoiseFactor.Text,
                txtOutputPower.Text, txtInputDCPower.Text, txtInputPower.Text, txtOutputPowerDBm.Text, txtSmallSignalGain.Text,
                txtFundamentalPower.Text, txtThirdOrderPower.Text,
                cmbPowerInUnit.SelectedItem?.ToString(), cmbPowerOutUnit.SelectedItem?.ToString()
            );
        }
        #endregion

        #region Calculation Methods
        private void CalculateGain()
        {
            try
            {
                var powerInValue = double.Parse(txtPowerIn.Text);
                var powerOutValue = double.Parse(txtPowerOut.Text);

                var powerIn = UnitC.Convert(powerInValue, cmbPowerInUnit.SelectedItem.ToString(), "W", UnitC.PhysicalQuantity.Power);
                var powerOut = UnitC.Convert(powerOutValue, cmbPowerOutUnit.SelectedItem.ToString(), "W", UnitC.PhysicalQuantity.Power);

                var gain = _calculationService.CalculateGain(powerIn, powerOut);
                GainResult = $"Усиление: {gain:F2} dB";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ErrorMessages.CalculationError}\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateNoiseFigure()
        {
            try
            {
                var noiseFactor = double.Parse(txtNoiseFactor.Text);

                var noiseFigure = _calculationService.CalculateNoiseFigure(noiseFactor);
                NoiseFigureResult = $"Коэффициент шума: {noiseFigure:F2} dB";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ErrorMessages.CalculationError}\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateEfficiency()
        {
            try
            {
                var outputPower = double.Parse(txtOutputPower.Text);
                var inputDCPower = double.Parse(txtInputDCPower.Text);

                var efficiency = _calculationService.CalculateAmplifierEfficiency(outputPower, inputDCPower);
                EfficiencyResult = $"КПД: {efficiency:F2}%";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ErrorMessages.CalculationError}\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Calculate1dBCompressionPoint()
        {
            try
            {
                var inputPower = double.Parse(txtInputPower.Text);
                var outputPower = double.Parse(txtOutputPowerDBm.Text);
                var smallSignalGain = double.Parse(txtSmallSignalGain.Text);

                var compressionPoint = _calculationService.Calculate1dBCompressionPoint(inputPower, outputPower, smallSignalGain);
                CompressionPointResult = $"Точка компрессии 1 dB: {compressionPoint:F2} dBm";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ErrorMessages.CalculationError}\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateIP3()
        {
            try
            {
                var fundamentalPower = double.Parse(txtFundamentalPower.Text);
                var thirdOrderPower = double.Parse(txtThirdOrderPower.Text);

                var ip3 = _calculationService.CalculateIP3(fundamentalPower, thirdOrderPower);
                IP3Result = $"Точка пересечения третьего порядка (IP3): {ip3:F2} dBm";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ErrorMessages.CalculationError}\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
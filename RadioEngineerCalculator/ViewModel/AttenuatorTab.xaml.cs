using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.ComboBoxService;
using static RadioEngineerCalculator.Services.UnitConverter;
using static RadioEngineerCalculator.Services.Validate;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class AttenuatorTab : UserControl, INotifyPropertyChanged
    {
        #region Приватные поля

        private readonly CalculationService _calculationService;
        private readonly Dictionary<string, ObservableCollection<string>> _unitCollections;

        private string _attenuationResult;
        private double _inputVoltage;
        private double _outputVoltage;
        private string _selectedInputVoltageUnit;
        private string _selectedOutputVoltageUnit;

        #endregion

        #region Конструктор

        public AttenuatorTab()
        {
            InitializeComponent();
            DataContext = this;
            _calculationService = new CalculationService();
            _unitCollections = InitializeUnitCollections();
            InitializeCommands();
            InitializeDefaultUnits();
        }

        #endregion

        #region Публичные свойства

        public ICommand CalculateAttenuationCommand { get; private set; }

        public bool CanCalculateAttenuation =>
            InputsAreValid(InputVoltage, OutputVoltage) &&
            !string.IsNullOrWhiteSpace(SelectedInputVoltageUnit) &&
            !string.IsNullOrWhiteSpace(SelectedOutputVoltageUnit);

        public string AttenuationResult { get => _attenuationResult; set => SetProperty(ref _attenuationResult, value); }
        public double InputVoltage { get => _inputVoltage; set { if (SetProperty(ref _inputVoltage, value)) OnPropertyChanged(nameof(CanCalculateAttenuation)); } }
        public double OutputVoltage { get => _outputVoltage; set { if (SetProperty(ref _outputVoltage, value)) OnPropertyChanged(nameof(CanCalculateAttenuation)); } }
        public string SelectedInputVoltageUnit { get => _selectedInputVoltageUnit; set { if (SetProperty(ref _selectedInputVoltageUnit, value)) { OnPropertyChanged(nameof(CanCalculateAttenuation)); ConvertInputVoltage(); } } }
        public string SelectedOutputVoltageUnit { get => _selectedOutputVoltageUnit; set { if (SetProperty(ref _selectedOutputVoltageUnit, value)) { OnPropertyChanged(nameof(CanCalculateAttenuation)); ConvertOutputVoltage(); } } }


        public ObservableCollection<string> VoltageUnits => _unitCollections["Voltage"];

        #endregion

        #region Приватные методы

        private Dictionary<string, ObservableCollection<string>> InitializeUnitCollections()
        {
            return new()
            {
                ["Voltage"] = new ObservableCollection<string>(GetUnits("Voltage"))
            };
        }

        private void InitializeCommands()
        {
            CalculateAttenuationCommand = new RelayCommand(CalculateAttenuation, () => CanCalculateAttenuation);
        }

        private void InitializeDefaultUnits()
        {
            SelectedInputVoltageUnit = VoltageUnits[0];
            SelectedOutputVoltageUnit = VoltageUnits[0];
        }

        private void CalculateAttenuation()
        {
            try
            {
                double inputVoltageV = Convert(InputVoltage, SelectedInputVoltageUnit, "V", PhysicalQuantity.Voltage);
                double outputVoltageV = Convert(OutputVoltage, SelectedOutputVoltageUnit, "V", PhysicalQuantity.Voltage);
                double attenuation = _calculationService.CalculateAttenuator(inputVoltageV, outputVoltageV);
                AttenuationResult = $"Затухание: {attenuation:F2} дБ";
            }
            catch (Exception ex)
            {
                AttenuationResult = $"Ошибка: {ex.Message}";
            }
        }

        private void ConvertInputVoltage() => ConvertUnit(ref _inputVoltage, "V", SelectedInputVoltageUnit);
        private void ConvertOutputVoltage() => ConvertUnit(ref _outputVoltage, "V", SelectedOutputVoltageUnit);

        private void ConvertUnit(ref double value, string fromUnit, string toUnit)
        {
            if (InputsAreValid(value) && !string.IsNullOrWhiteSpace(toUnit))
            {
                value = Convert(value, fromUnit, toUnit, PhysicalQuantity.Voltage);
            }
        }

        #endregion

        #region Реализация INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
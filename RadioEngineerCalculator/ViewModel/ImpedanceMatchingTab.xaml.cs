using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.UnitC;
using static RadioEngineerCalculator.Services.Validate;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class ImpedanceMatchingTab : UserControl, INotifyPropertyChanged
    {
        private readonly CalculationService _calculationService;
        private readonly ObservableCollection<string> _impedanceUnits;

        private double _sourceImpedance;
        private double _loadImpedance;
        private string _selectedSourceImpedanceUnit;
        private string _selectedLoadImpedanceUnit;
        private string _impedanceMatchingResult;
        private string _reflectionCoefficientResult;
        private string _vswrResult;

        public ImpedanceMatchingTab()
        {
            InitializeComponent();
            DataContext = this;
            _calculationService = new CalculationService();
            _impedanceUnits = new ObservableCollection<string>(ComboBoxService.GetUnits("Resistance"));
            InitializeCommands();
            InitializeDefaultUnits();
        }

        public ICommand CalculateImpedanceMatchingCommand { get; private set; }
        public ICommand CalculateReflectionCoefficientCommand { get; private set; }
        public ICommand CalculateVSWRCommand { get; private set; }

        public bool CanCalculateImpedanceMatching => InputsAreValid(SourceImpedance, LoadImpedance) &&
                                                     !string.IsNullOrWhiteSpace(SelectedSourceImpedanceUnit) &&
                                                     !string.IsNullOrWhiteSpace(SelectedLoadImpedanceUnit);

        public double SourceImpedance
        {
            get => _sourceImpedance;
            set
            {
                if (SetProperty(ref _sourceImpedance, value))
                {
                    OnPropertyChanged(nameof(CanCalculateImpedanceMatching));
                }
            }
        }

        public double LoadImpedance
        {
            get => _loadImpedance;
            set
            {
                if (SetProperty(ref _loadImpedance, value))
                {
                    OnPropertyChanged(nameof(CanCalculateImpedanceMatching));
                }
            }
        }

        public string SelectedSourceImpedanceUnit
        {
            get => _selectedSourceImpedanceUnit;
            set
            {
                if (SetProperty(ref _selectedSourceImpedanceUnit, value))
                {
                    OnPropertyChanged(nameof(CanCalculateImpedanceMatching));
                    ConvertSourceImpedance();
                }
            }
        }

        public string SelectedLoadImpedanceUnit
        {
            get => _selectedLoadImpedanceUnit;
            set
            {
                if (SetProperty(ref _selectedLoadImpedanceUnit, value))
                {
                    OnPropertyChanged(nameof(CanCalculateImpedanceMatching));
                    ConvertLoadImpedance();
                }
            }
        }

        public string ImpedanceMatchingResult
        {
            get => _impedanceMatchingResult;
            set => SetProperty(ref _impedanceMatchingResult, value);
        }

        public string ReflectionCoefficientResult
        {
            get => _reflectionCoefficientResult;
            set => SetProperty(ref _reflectionCoefficientResult, value);
        }

        public string VSWRResult
        {
            get => _vswrResult;
            set => SetProperty(ref _vswrResult, value);
        }

        public ObservableCollection<string> ImpedanceUnits => _impedanceUnits;

        private void InitializeCommands()
        {
            CalculateImpedanceMatchingCommand = new RelayCommand(CalculateImpedanceMatching, () => CanCalculateImpedanceMatching);
            CalculateReflectionCoefficientCommand = new RelayCommand(CalculateReflectionCoefficient, () => CanCalculateImpedanceMatching);
            CalculateVSWRCommand = new RelayCommand(CalculateVSWR, () => CanCalculateImpedanceMatching);
        }

        private void InitializeDefaultUnits()
        {
            SelectedSourceImpedanceUnit = ImpedanceUnits[0];
            SelectedLoadImpedanceUnit = ImpedanceUnits[0];
        }

        private void CalculateImpedanceMatching()
        {
            try
            {
                double sourceImpedanceOhm = Convert(SourceImpedance, SelectedSourceImpedanceUnit, "Ω", PhysicalQuantity.Resistance);
                double loadImpedanceOhm = Convert(LoadImpedance, SelectedLoadImpedanceUnit, "Ω", PhysicalQuantity.Resistance);
                double matchingImpedance = _calculationService.CalculateImpedanceMatching(sourceImpedanceOhm, loadImpedanceOhm);
                ImpedanceMatchingResult = $"Согласованный импеданс: {FormatResult(matchingImpedance, PhysicalQuantity.Resistance)}";
            }
            catch (Exception ex)
            {
                ImpedanceMatchingResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculateReflectionCoefficient()
        {
            try
            {
                double sourceImpedanceOhm = Convert(SourceImpedance, SelectedSourceImpedanceUnit, "Ω", PhysicalQuantity.Resistance);
                double loadImpedanceOhm = Convert(LoadImpedance, SelectedLoadImpedanceUnit, "Ω", PhysicalQuantity.Resistance);
                double reflectionCoefficient = _calculationService.CalculateReflectionCoefficient(sourceImpedanceOhm, loadImpedanceOhm);
                ReflectionCoefficientResult = $"Коэффициент отражения: {reflectionCoefficient:F4}";
            }
            catch (Exception ex)
            {
                ReflectionCoefficientResult = $"Ошибка: {ex.Message}";
            }
        }

        private void CalculateVSWR()
        {
            try
            {
                double sourceImpedanceOhm = Convert(SourceImpedance, SelectedSourceImpedanceUnit, "Ω", PhysicalQuantity.Resistance);
                double loadImpedanceOhm = Convert(LoadImpedance, SelectedLoadImpedanceUnit, "Ω", PhysicalQuantity.Resistance);
                double vswr = _calculationService.CalculateVSWR(sourceImpedanceOhm, loadImpedanceOhm);
                VSWRResult = $"КСВН: {vswr:F2}";
            }
            catch (Exception ex)
            {
                VSWRResult = $"Ошибка: {ex.Message}";
            }
        }

        private void ConvertSourceImpedance() => ConvertUnit(ref _sourceImpedance, "Ω", SelectedSourceImpedanceUnit, PhysicalQuantity.Resistance);
        private void ConvertLoadImpedance() => ConvertUnit(ref _loadImpedance, "Ω", SelectedLoadImpedanceUnit, PhysicalQuantity.Resistance);

        private void ConvertUnit(ref double value, string fromUnit, string toUnit, PhysicalQuantity quantity)
        {
            if (InputsAreValid(value) && !string.IsNullOrWhiteSpace(toUnit))
            {
                value = Convert(value, fromUnit, toUnit, quantity);
            }
        }

        private string FormatResult(double value, PhysicalQuantity quantity) => UnitC.AutoFormat(value, quantity);

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
    }
}
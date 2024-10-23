using RadioEngineerCalculator.Infos;
using RadioEngineerCalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using static RadioEngineerCalculator.Services.UnitConverter;
using static RadioEngineerCalculator.Services.Validate;
using static RadioEngineerCalculator.Services.ComboBoxService;
using static RadioEngineerCalculator.Infos.ErrorMessages;

namespace RadioEngineerCalculator.ViewModel
{
    public partial class ImpedanceMatchingTab : UserControl, INotifyPropertyChanged
    {
        #region Приватные поля

        private readonly CalculationService _calculationService;
        private readonly Dictionary<string, ObservableCollection<string>> _unitCollections;

        private string _impedanceMatchingResult;
        private string _reflectionCoefficientResult;
        private string _vswrResult;
        private double _sourceImpedance;
        private double _loadImpedance;
        private string _selectedSourceImpedanceUnit;
        private string _selectedLoadImpedanceUnit;

        #endregion

        #region Конструктор и инициализация

        public ImpedanceMatchingTab()
        {
            InitializeComponent();
            DataContext = this;
            _calculationService = new CalculationService();
            _unitCollections = InitializeUnitCollections();
            InitializeCommands();
            InitializeDefaultUnits();
        }

        private Dictionary<string, ObservableCollection<string>> InitializeUnitCollections()
        {
            return new Dictionary<string, ObservableCollection<string>>
            {
                ["Impedance"] = new ObservableCollection<string>(ComboBoxService.GetUnits("Resistance"))
            };
        }

        private void InitializeCommands()
        {
            CalculateImpedanceMatchingCommand = new RelayCommand(CalculateImpedanceMatching, () => CanCalculateImpedanceMatching);
            CalculateReflectionCoefficientCommand = new RelayCommand(CalculateReflectionCoefficient, () => CanCalculateReflectionCoefficient);
            CalculateVSWRCommand = new RelayCommand(CalculateVSWR, () => CanCalculateVSWR);
        }

        private void InitializeDefaultUnits()
        {
            if (ImpedanceUnits != null && ImpedanceUnits.Count > 0)
            {
                SelectedSourceImpedanceUnit = ImpedanceUnits[0];
                SelectedLoadImpedanceUnit = ImpedanceUnits[0];
            }
            else
            {
                SelectedSourceImpedanceUnit = "Ω"; 
                SelectedLoadImpedanceUnit = "Ω";
            }
        }

        #endregion

        #region Свойства

        public ICommand CalculateImpedanceMatchingCommand { get; private set; }
        public ICommand CalculateReflectionCoefficientCommand { get; private set; }
        public ICommand CalculateVSWRCommand { get; private set; }

        public bool CanCalculateImpedanceMatching => InputsAreValid(SourceImpedance, LoadImpedance) &&
                                                     !string.IsNullOrWhiteSpace(SelectedSourceImpedanceUnit) &&
                                                     !string.IsNullOrWhiteSpace(SelectedLoadImpedanceUnit);

        public bool CanCalculateReflectionCoefficient => CanCalculateImpedanceMatching;
        public bool CanCalculateVSWR => CanCalculateImpedanceMatching;

        public string ImpedanceMatchingResult { get => _impedanceMatchingResult; set => SetProperty(ref _impedanceMatchingResult, value); }
        public string ReflectionCoefficientResult { get => _reflectionCoefficientResult; set => SetProperty(ref _reflectionCoefficientResult, value); }
        public string VSWRResult { get => _vswrResult; set => SetProperty(ref _vswrResult, value); }

        public double SourceImpedance
        {
            get => _sourceImpedance;
            set
            {
                if (SetProperty(ref _sourceImpedance, value))
                {
                    OnPropertyChanged(nameof(CanCalculateImpedanceMatching));
                    OnPropertyChanged(nameof(CanCalculateReflectionCoefficient));
                    OnPropertyChanged(nameof(CanCalculateVSWR));
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
                    OnPropertyChanged(nameof(CanCalculateReflectionCoefficient));
                    OnPropertyChanged(nameof(CanCalculateVSWR));
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
                    OnPropertyChanged(nameof(CanCalculateReflectionCoefficient));
                    OnPropertyChanged(nameof(CanCalculateVSWR));
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
                    OnPropertyChanged(nameof(CanCalculateReflectionCoefficient));
                    OnPropertyChanged(nameof(CanCalculateVSWR));
                    ConvertLoadImpedance();
                }
            }
        }

        public ObservableCollection<string> ImpedanceUnits => _unitCollections["Impedance"];

        #endregion

        #region Методы расчетов

        private void CalculateImpedanceMatching()
        {
            try
            {
                double sourceImpedanceOhm = Convert(SourceImpedance, SelectedSourceImpedanceUnit, "Ω", PhysicalQuantity.Resistance);
                double loadImpedanceOhm = Convert(LoadImpedance, SelectedLoadImpedanceUnit, "Ω", PhysicalQuantity.Resistance);
                double matchingImpedance = _calculationService.CalculateImpedanceMatching(sourceImpedanceOhm, loadImpedanceOhm);
                ImpedanceMatchingResult = $"Согласующий импеданс: {FormatResult(matchingImpedance, PhysicalQuantity.Resistance)}";
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

        #endregion

        #region Вспомогательные методы

        private void ConvertSourceImpedance() => ConvertUnit(ref _sourceImpedance, "Ω", SelectedSourceImpedanceUnit, PhysicalQuantity.Resistance);
        private void ConvertLoadImpedance() => ConvertUnit(ref _loadImpedance, "Ω", SelectedLoadImpedanceUnit, PhysicalQuantity.Resistance);

        private void ConvertUnit(ref double value, string fromUnit, string toUnit, PhysicalQuantity quantity)
        {
            if (InputsAreValid(value) && !string.IsNullOrWhiteSpace(toUnit))
            {
                value = Convert(value, fromUnit, toUnit, quantity);
            }
        }

        private string FormatResult(double value, PhysicalQuantity quantity) => AutoFormat(value, quantity);

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
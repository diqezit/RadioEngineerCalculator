# Документация приложения RadioEngineerCalculator

## Введение

Приложение **RadioEngineerCalculator** предназначено для инженеров и радиолюбителей, которым необходимо быстро и удобно выполнять различные расчеты в области радиотехники. Приложение позволяет автоматизировать рутинные расчеты, которые обычно выполняются вручную или с использованием таблиц Excel.

## Основные функции

- **Динамическая загрузка вкладок**: Каждая категория расчета загружается в отдельную вкладку, что позволяет пользователю легко переключаться между различными расчетами, сохраняя при этом главное окно аккуратным и удобным для использования.
- **Расширяемость**: Новые вкладки с расчетами могут быть легко добавлены в приложение, что позволяет расширять его функциональность в зависимости от потребностей пользователей.
- **Конвертация единиц измерения**: Приложение использует общий сервис конвертации единиц измерения, что позволяет автоматически подстраивать результаты расчетов под значения СИ.

## Структура приложения

Приложение **RadioEngineerCalculator** имеет следующую структуру:

```
RadioEngineerCalculator
├──	Infos
│	├──	Info.cs (Cодержит описания и дополнительную информацию)
│	├──	Tooltips.cs (Cодержит подсказки для полей ввода в приложении)
│	└──	ErrorMSG.cs (Предоставляет строковые константы с сообщениями об ошибках для различных некорректных входных данных)
├── Resources
│   └── Styles.xaml (Содержит стили и цветовые схемы для элементов управления в приложении, таких как кнопки, текстовые поля, метки, вкладки, комбобоксы, группы и чекбоксы)
├── Services
│   ├── CalculationService.cs (Содержит основные методы для выполнения различных радиоинженерных расчетов)
│	├──	FiltersCalculationService.cs (Предоставляет методы для расчета параметров для LRCFilterTab.xaml.cs. Также определены классы для хранения входных данных и результатов расчетов)
│   ├── ComboBoxService.cs (Содержит класс, который предоставляет коллекции единиц измерения, типы  и компоненты. Метод GetUnits возвращает единицы измерения для указанной величины.)
│   └── UnitConverter.cs (Содержит класс UnitC,предоставляет методы для конвертации и форматирования значений величин. Внутри класса есть вложенные классы Conv и Form, удобно конвертируют и форматируют значения.)
├── ViewModel (некоторые функции еще не реализованы!)
│   ├── AmplifierTab.xaml (Вкладка расчета усилителей)
│   ├── AmplifierTab.xaml.cs (Логика вкладки расчета усилителей)
│   ├── AntennaArrayTab.xaml (Вкладка расчета антенных решеток)
│   ├── AntennaArrayTab.xaml.cs (Логика вкладки расчета антенных решеток)
│   ├── AntennaTab.xaml (Вкладка расчета антенн)
│   ├── AntennaTab.xaml.cs (Логика вкладки расчета антенн)
│   ├── AttenuatorTab.xaml (Вкладка расчета аттенюаторов)
│   ├── AttenuatorTab.xaml.cs (Логика вкладки расчета аттенюаторов)
│   ├── CoaxialCableTab.xaml (Вкладка расчета коаксиальных кабелей)
│   ├── CoaxialCableTab.xaml.cs (Логика вкладки расчета коаксиальных кабелей)
│   ├── DigitalCommunicationTab.xaml (Вкладка расчета цифровой связи)
│   ├── DigitalCommunicationTab.xaml.cs (Логика вкладки расчета цифровой связи)
│   ├── ImpedanceMatchingTab.xaml (Вкладка расчета согласования импедансов)
│   ├── ImpedanceMatchingTab.xaml.cs (Логика вкладки расчета согласования импедансов)
│   ├── LinkBudgetTab.xaml (Вкладка расчета бюджета линии связи)
│   ├── LinkBudgetTab.xaml.cs (Логика вкладки расчета бюджета линии связи)
│   ├── LRCFilterTab.xaml (Вкладка расчета LRC фильтров)
│   ├── LRCFilterTab.xaml.cs (Логика вкладки расчета LRC фильтров)
│   ├── MicrowaveComponentsTab.xaml (Вкладка расчета микроволновых компонентов)
│   ├── MicrowaveComponentsTab.xaml.cs (Логика вкладки расчета микроволновых компонентов)
│   ├── ModulationTab.xaml (Вкладка расчета модуляции)
│   ├── ModulationTab.xaml.cs (Логика вкладки расчета модуляции)
│   ├── NoiseAnalysisTab.xaml (Вкладка анализа шума)
│   ├── NoiseAnalysisTab.xaml.cs (Логика вкладки анализа шума)
│   ├── OhmTab.xaml (Вкладка расчета закона Ома)
│   ├── OhmTab.xaml.cs (Логика вкладки расчета закона Ома)
│   ├── OpAmpTab.xaml (Вкладка расчета операционных усилителей)
│   ├── OpAmpTab.xaml.cs (Логика вкладки расчета операционных усилителей)
│   ├── OscillatorTab.xaml (Вкладка расчета генераторов)
│   ├── OscillatorTab.xaml.cs (Логика вкладки расчета генераторов)
│   ├── PhaseLockedLoopTab.xaml (Вкладка расчета систем фазовой автоподстройки частоты)
│   ├── PhaseLockedLoopTab.xaml.cs (Логика вкладки расчета систем фазовой автоподстройки частоты)
│   ├── PowerCalculationTab.xaml (Вкладка расчета мощности)
│   ├── PowerCalculationTab.xaml.cs (Логика вкладки расчета мощности)
│   ├── SmithChartTab.xaml (Вкладка расчета по диаграмме Смита)
│   └── SmithChartTab.xaml.cs (Логика вкладки расчета по диаграмме Смита)
├── App.xaml (Основные настройки приложения)
├── App.xaml.cs (Логика запуска приложения)
├── MainWindow.xaml (Главное окно приложения)
└── MainWindow.xaml.cs (Логика главного окна приложения)

```

## Основные компоненты

### Главное окно (MainWindow.xaml)

Главное окно приложения содержит `TabControl`, который динамически загружает вкладки с различными расчетами. 
Каждая вкладка соответствует определенной категории расчетов.

```csharp
private void LoadTabs()
{
    MainTabControl.Items.Add(new TabItem { Header = "Filter", Content = new FilterTab() });
    // Добавление других вкладок
}
```

### Вкладки расчетов (ViewModel)

Каждая вкладка расчетов в папке `/ViewModel` содержит XAML-разметку и логику расчета. 
Например, вкладка `AmplifierTab.xaml` включает поля ввода и кнопки для расчета усилителей. 
Все новые вкладки должны обязательно соответствовать такому стилю.

```xml
<UserControl x:Class="RadioEngineerCalculator.ViewModel.LRCFilterTab"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:infos="clr-namespace:RadioEngineerCalculator.Infos"
             xmlns:oxy="http://oxyplot.org/wpf"
             mc:Ignorable="d"
             d:DesignHeight="450" d:DesignWidth="800">
    <ScrollViewer VerticalScrollBarVisibility="Auto" HorizontalScrollBarVisibility="Auto">
        <Grid Margin="10">
            <StackPanel>
                <!-- Заголовок -->
                <TextBlock Text="Расчеты LC фильтра" 
                           HorizontalAlignment="Center" 
                           VerticalAlignment="Center" 
                           FontSize="22" 
                           FontWeight="Bold" 
                           Margin="0,10,0,20"/>

                <!-- Группа расчета фильтра -->
                <GroupBox Header="Расчет LC фильтра" 
                          BorderBrush="{StaticResource DividerBrush}" 
                          BorderThickness="1" 
                          Padding="10">
                    <StackPanel Margin="10">

                        <!-- Тип фильтра -->
                        <Label Content="Тип фильтра" Style="{StaticResource ModernLabel}"/>
                        <ComboBox x:Name="cmbFilterType"
                                  SelectedItem="{Binding SelectedFilterType, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                                  Style="{StaticResource ModernComboBox}"
                                  ItemsSource="{Binding FilterTypes}">
                            <ComboBox.ToolTip>
                                <ToolTip Content="{x:Static infos:Tooltips.LCFilterTab_FilterTypeTooltip}"/>
                            </ComboBox.ToolTip>
                        </ComboBox>

                        <!-- Емкость -->
                        <Label Content="Емкость" Style="{StaticResource ModernLabel}" Margin="10,10,0,0"/>
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="Auto"/>
                            </Grid.ColumnDefinitions>
                            <TextBox x:Name="txtCapacitance" 
                                     Text="{Binding Capacitance, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" 
                                     Style="{StaticResource ModernTextBox}">
                                <TextBox.ToolTip>
                                    <ToolTip Content="{x:Static infos:Tooltips.LCFilterTab_CapacitanceTooltip}"/>
                                </TextBox.ToolTip>
                            </TextBox>
                            <ComboBox x:Name="cmbCapacitanceUnit" 
                                      Grid.Column="1" 
                                      Width="60" 
                                      SelectedItem="{Binding SelectedCapacitanceUnit, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" 
                                      Style="{StaticResource ModernComboBox}"
                                      ItemsSource="{Binding CapacitanceUnits}">
                                <ComboBox.ToolTip>
                                    <ToolTip Content="{x:Static infos:Tooltips.LCFilterTab_CapacitanceUnitTooltip}"/>
                                </ComboBox.ToolTip>
                            </ComboBox>
                        </Grid>

                        <!-- Индуктивность -->
                        <Label Content="Индуктивность" Style="{StaticResource ModernLabel}" Margin="10,10,0,0"/>
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="Auto"/>
                            </Grid.ColumnDefinitions>
                            <TextBox x:Name="txtInductance" 
                                     Text="{Binding Inductance, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" 
                                     Style="{StaticResource ModernTextBox}">
                                <TextBox.ToolTip>
                                    <ToolTip Content="{x:Static infos:Tooltips.LCFilterTab_InductanceTooltip}"/>
                                </TextBox.ToolTip>
                            </TextBox>
                            <ComboBox x:Name="cmbInductanceUnit" 
                                      Grid.Column="1" 
                                      Width="60" 
                                      SelectedItem="{Binding SelectedInductanceUnit, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" 
                                      Style="{StaticResource ModernComboBox}"
                                      ItemsSource="{Binding InductanceUnits}">
                                <ComboBox.ToolTip>
                                    <ToolTip Content="{x:Static infos:Tooltips.LCFilterTab_InductanceUnitTooltip}"/>
                                </ComboBox.ToolTip>
                            </ComboBox>
                        </Grid>

                        <!-- Сопротивление -->
                        <Label Content="Сопротивление" Style="{StaticResource ModernLabel}" Margin="10,10,0,0"/>
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="Auto"/>
                            </Grid.ColumnDefinitions>
                            <TextBox x:Name="txtResistance" 
                                     Text="{Binding Resistance, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" 
                                     Style="{StaticResource ModernTextBox}">
                                <TextBox.ToolTip>
                                    <ToolTip Content="{x:Static infos:Tooltips.LCFilterTab_ResistanceTooltip}"/>
                                </TextBox.ToolTip>
                            </TextBox>
                            <ComboBox x:Name="cmbResistanceUnit" 
                                      Grid.Column="1" 
                                      Width="60" 
                                      SelectedItem="{Binding SelectedResistanceUnit, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" 
                                      Style="{StaticResource ModernComboBox}"
                                      ItemsSource="{Binding ResistanceUnits}">
                                <ComboBox.ToolTip>
                                    <ToolTip Content="{x:Static infos:Tooltips.LCFilterTab_ResistanceUnitTooltip}"/>
                                </ComboBox.ToolTip>
                            </ComboBox>
                        </Grid>

                        <!-- Частота -->
                        <Label Content="Частота" Style="{StaticResource ModernLabel}" Margin="10,10,0,0"/>
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="Auto"/>
                            </Grid.ColumnDefinitions>
                            <TextBox x:Name="txtFrequency" 
                                     Text="{Binding Frequency, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" 
                                     Style="{StaticResource ModernTextBox}">
                                <TextBox.ToolTip>
                                    <ToolTip Content="{x:Static infos:Tooltips.LCFilterTab_FrequencyTooltip}"/>
                                </TextBox.ToolTip>
                            </TextBox>
                            <ComboBox x:Name="cmbFrequencyUnit" 
                                      Grid.Column="1" 
                                      Width="60" 
                                      SelectedItem="{Binding SelectedFrequencyUnit, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" 
                                      Style="{StaticResource ModernComboBox}"
                                      ItemsSource="{Binding FrequencyUnits}">
                                <ComboBox.ToolTip>
                                    <ToolTip Content="{x:Static infos:Tooltips.LCFilterTab_FrequencyUnitTooltip}"/>
                                </ComboBox.ToolTip>
                            </ComboBox>
                        </Grid>

                        <!-- Кнопка расчета -->
                        <Button Content="Рассчитать все параметры" 
                                Click="CalculateFilterParameters" 
                                Style="{StaticResource ModernButton}" 
                                Margin="0,20,0,0"/>

                        <!-- Текст справки -->
                        <TextBlock x:Name="txtHelpText" 
                                   Text="Здесь будут отображаться результаты расчетов." 
                                   FontSize="14" 
                                   Foreground="Gray" 
                                   Margin="0,20,0,0"/>

                        <!-- Результаты расчетов -->
                        <TextBlock Text="Результаты расчетов:" FontWeight="Bold" Margin="0,20,0,10"/>
                        <TextBlock x:Name="txtCutoffFrequencyResult" 
           Text="{Binding CutoffFrequencyResult}" 
           Margin="0,10,0,0" 
           FontWeight="Bold"/>
                        <TextBlock x:Name="txtQualityFactorResult" 
           Text="{Binding QualityFactorResult}" 
           Margin="0,10,0,0" 
           FontWeight="Bold"/>
                        <TextBlock x:Name="txtBandwidthResult" 
           Text="{Binding BandwidthResult}" 
           Margin="0,10,0,0" 
           FontWeight="Bold"/>
                        <TextBlock x:Name="txtImpedanceResult" 
           Text="{Binding ImpedanceResult}" 
           Margin="0,10,0,0" 
           FontWeight="Bold"/>
                        <TextBlock x:Name="txtPhaseShiftResult" 
           Text="{Binding PhaseShiftResult}" 
           Margin="0,10,0,0" 
           FontWeight="Bold"/>
                        <TextBlock x:Name="txtGroupDelayResult" 
           Text="{Binding GroupDelayResult}" 
           Margin="0,10,0,0" 
           FontWeight="Bold"/>
                        <TextBlock x:Name="txtAttenuationResult" 
           Text="{Binding AttenuationResult}" 
           Margin="0,10,0,0" 
           FontWeight="Bold"/>
                        <TextBlock x:Name="txtAdditionalInfo" 
           Text="{Binding AdditionalInfo}" 
           Margin="0,10,0,0" 
           FontWeight="Bold"/>

                        <!-- График фильтра -->
                        <GroupBox Header="График среза фильтра" BorderBrush="{StaticResource DividerBrush}" BorderThickness="1" Margin="0,20,0,0">
                            <oxy:PlotView x:Name="FilterResponsePlot"
                  Model="{Binding FilterResponseModel}"/>
                        </GroupBox>
                    </StackPanel>
                </GroupBox>
            </StackPanel>
        </Grid>
    </ScrollViewer>
</UserControl>
```

### Сервисы (Services)

#### CalculationService.cs

Класс CalculationService содержит методы для выполнения различных инженерных расчетов, таких как расчет усиления, 
шума, импеданса и других параметров, связанных с радиотехникой. 
Этот класс является центральным сервисом для всех математических операций в приложении.

```csharp
public class CalculationService
{
    public double CalculateGain(double powerIn, double powerOut)
        => 10 * Math.Log10(powerOut / powerIn);

    public double CalculateNoiseFigure(double noiseFactor)
        => 10 * Math.Log10(noiseFactor);

    // Другие методы расчета
}
```

#### UnitConverter.cs

Данный файл представляет собой класс UnitConverter, который отвечает за конвертацию физических величин из одной единицы измерения в другую. 
Он содержит:
Перечисление PhysicalQuantity - список различных физических величин, таких как частота, напряжение, сила и т.д.
Словарь UnitFactors - хранит коэффициенты для преобразования между различными единицами для каждой физической величины.
Метод Convert - выполняет конвертацию значения из одной единицы в другую, используя коэффициенты из словаря.
Статические классы Converters и Formatters - предоставляют методы для удобного преобразования и форматирования значений конкретных физических величин

```csharp
using System;
using System.Collections.Generic;

namespace RadioEngineerCalculator.Services
{
    public static class UnitC
    {
        public enum PhysicalQuantity
        {
            Frequency,
            Resistance,
            Capacitance,
            Voltage,
            Current,
            Inductance,
            Length,
            Power,
            Pressure,
            Time,
			// Дальше аналогично
        }

        private static readonly Dictionary<PhysicalQuantity, Dictionary<string, double>> UnitFactors = new Dictionary<PhysicalQuantity, Dictionary<string, double>>
        {
            { PhysicalQuantity.Frequency, new Dictionary<string, double> { { "Hz", 1 }, { "kHz", 1e3 }, { "MHz", 1e6 }, { "GHz", 1e9 }, { "THz", 1e12 } } },
            { PhysicalQuantity.Resistance, new Dictionary<string, double> { { "Ω", 1 }, { "kΩ", 1e3 }, { "MΩ", 1e6 }, { "mΩ", 1e-3 }, { "μΩ", 1e-6 } } },
            { PhysicalQuantity.Capacitance, new Dictionary<string, double> { { "F", 1 }, { "mF", 1e-3 }, { "μF", 1e-6 }, { "nF", 1e-9 }, { "pF", 1e-12 } } },
            { PhysicalQuantity.Voltage, new Dictionary<string, double> { { "V", 1 }, { "mV", 1e-3 }, { "kV", 1e3 }, { "μV", 1e-6 }, { "nV", 1e-9 } } },
            { PhysicalQuantity.Current, new Dictionary<string, double> { { "A", 1 }, { "mA", 1e-3 }, { "μA", 1e-6 }, { "nA", 1e-9 }, { "pA", 1e-12 } } },
            { PhysicalQuantity.Inductance, new Dictionary<string, double> { { "H", 1 }, { "mH", 1e-3 }, { "μH", 1e-6 }, { "nH", 1e-9 }, { "pH", 1e-12 } } },
            { PhysicalQuantity.Length, new Dictionary<string, double> { { "m", 1 }, { "cm", 1e-2 }, { "mm", 1e-3 }, { "km", 1e3 }, { "μm", 1e-6 }, { "nm", 1e-9 } } },
			// Дальше аналогично для других величин	
        };

        public static double Convert(double value, string fromUnit, string toUnit, PhysicalQuantity quantity)
        {
            if (!UnitFactors.TryGetValue(quantity, out var factors))
                throw new ArgumentException($"Invalid quantity: {quantity}");

            if (!factors.TryGetValue(fromUnit, out double fromFactor))
                throw new ArgumentException($"Invalid 'fromUnit': {fromUnit}");

            if (!factors.TryGetValue(toUnit, out double toFactor))
                throw new ArgumentException($"Invalid 'toUnit': {toUnit}");

            return value * (fromFactor / toFactor);
        }

        public static class Conv
        {
            public static double Frequency(double value, string fromUnit, string toUnit) => Convert(value, fromUnit, toUnit, PhysicalQuantity.Frequency);
            public static double Resistance(double value, string fromUnit, string toUnit) => Convert(value, fromUnit, toUnit, PhysicalQuantity.Resistance);
            public static double Capacitance(double value, string fromUnit, string toUnit) => Convert(value, fromUnit, toUnit, PhysicalQuantity.Capacitance);
            public static double Voltage(double value, string fromUnit, string toUnit) => Convert(value, fromUnit, toUnit, PhysicalQuantity.Voltage);
            public static double Current(double value, string fromUnit, string toUnit) => Convert(value, fromUnit, toUnit, PhysicalQuantity.Current);
            public static double Inductance(double value, string fromUnit, string toUnit) => Convert(value, fromUnit, toUnit, PhysicalQuantity.Inductance);
            public static double Length(double value, string fromUnit, string toUnit) => Convert(value, fromUnit, toUnit, PhysicalQuantity.Length);
			// Дальше аналогично для других величин
        }

        public static class Form
        {
            public static string FormatValue(double value, string baseUnit, string middleUnit, string smallUnit, double middleThreshold, double smallThreshold)
            {
                if (Math.Abs(value) >= middleThreshold)
                    return $"{value / middleThreshold:F2} {middleUnit}";
                else if (Math.Abs(value) >= smallThreshold)
                    return $"{value / smallThreshold:F2} {smallUnit}";
                else
                    return $"{value:F2} {baseUnit}";
            }

            public static string Resistance(double value) => FormatValue(value, "Ω", "kΩ", "MΩ", 1e3, 1e6);
            public static string Capacitance(double value) => FormatValue(value, "F", "μF", "nF", 1e-6, 1e-9);
            public static string Frequency(double value) => FormatValue(value, "Hz", "kHz", "MHz", 1e3, 1e6);
            public static string Voltage(double value) => FormatValue(value, "V", "kV", "mV", 1e3, 1e-3);
			// Дальше аналогично для других величин
        }
    }
}
```

#### ComboBoxService.cs

Класс `ComboBoxService` предоставляет методы для заполнения `ComboBox` различными единицами измерения и типов елементов, и их хранения.

```csharp
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RadioEngineerCalculator.Services
{
    public static class ComboBoxService
    {
        private static readonly Dictionary<string, ObservableCollection<string>> units = new Dictionary<string, ObservableCollection<string>>
        {
            { "Current", new ObservableCollection<string> { "A", "mA", "μA", "nA", "pA" } },
            { "Resistance", new ObservableCollection<string> { "Ω", "kΩ", "MΩ", "mΩ", "μΩ" } },
            { "Voltage", new ObservableCollection<string> { "V", "mV", "kV", "μV", "nV" } },
            { "Power", new ObservableCollection<string> { "W", "kW", "MW", "mW", "μW" } },
			// Дальше аналогично для других величин
        };

        public static ObservableCollection<string> GetUnits(string quantity)
        {
            if (units.TryGetValue(quantity, out var unitCollection))
            {
                return unitCollection;
            }
            return new ObservableCollection<string>();
        }

        public static Dictionary<int, string> FilterTypes = new Dictionary<int, string>
        {
          { 0, "Low Pass" },
          { 1, "High Pass" },
          { 2, "Band Pass" },
          { 3, "Band Stop" }
        };


        public static Dictionary<int, string> ComponentTypeLabels { get; } = new Dictionary<int, string>
        {
            { 0, "Capacitance" },
            { 1, "Inductance" },
            { 2, "Resistor" },
            { 3, "Transistor" },
            { 4, "Diode" },
            { 5, "Transformer" },
            { 6, "Operational Amplifier" },
            { 7, "Voltage Regulator" },
            { 8, "Microcontroller" }
        };
    }
}

```

### Информация и подсказки (Infos)

#### Tooltips.cs

Cодержит описания для различных полей ввода, которые отображаются в виде подсказок при наведении курсора на элементы управления.

```csharp
public static class Tooltips
{
    public const string AmplifierTab_PowerInTooltip = "Введите мощность на входе в ваттах (W) или других единицах.";
    public const string AmplifierTab_PowerInUnitTooltip = "Выберите единицу измерения для мощности на входе.";
    // Другие подсказки
}
```
Файл Info.cs содержит статический класс Info, который предоставляет описания и дополнительную 
информацию о различных типах, используемых в радиоинженерных расчетах.

```
using RadioEngineerCalculator.Services;
using static RadioEngineerCalculator.Services.FiltersCalculationService;

namespace RadioEngineerCalculator.Infos
{
    public static class Info
    {
        public static string LowPassFilterDescription => "Low-pass filter attenuates frequencies higher than the cutoff frequency.";

        public static string HighPassFilterDescription => "High-pass filter attenuates frequencies lower than the cutoff frequency.";

        public static string BandPassFilterDescription => "Band-pass filter allows a specific range of frequencies to pass through.";

        public static string BandStopFilterDescription => "Band-stop filter attenuates a specific range of frequencies.";

        public static string RCFilterDescription => "RC filter uses a resistor and capacitor to filter signals.";

        public static string RLFilterDescription => "RL filter uses a resistor and inductor to filter signals.";

        public static string QuartzFilterDescription => "Quartz filter uses a quartz crystal for high precision frequency filtering.";

        public static string GetAdditionalInfo(FilterType filterType, double cutoffFrequency, double bandwidth)
        {
            switch (filterType)
            {
                case FilterType.LowPass:
                    return "Passes frequencies below the cutoff frequency";
                case FilterType.HighPass:
                    return "Passes frequencies above the cutoff frequency";
                case FilterType.BandPass:
                case FilterType.PassiveBandPass:
                    return $"Passes frequencies between {UnitC.Form.Frequency(cutoffFrequency - bandwidth / 2)} and {UnitC.Form.Frequency(cutoffFrequency + bandwidth / 2)}";
                case FilterType.BandStop:
                case FilterType.PassiveBandStop:
                    return $"Attenuates frequencies between {UnitC.Form.Frequency(cutoffFrequency - bandwidth / 2)} and {UnitC.Form.Frequency(cutoffFrequency + bandwidth / 2)}";
                default:
                    return "Additional information not available for this filter type";
            }
        }
    }
}
```

### ErrorMessages.cs 
Cодержит статический класс ErrorMessages, 
который предоставляет строковые константы с сообщениями об ошибках для различных некорректных входных данных.

```
namespace RadioEngineerCalculator.Infos
{
    public static class ErrorMessages
    {
        public static string InvalidFrequencyInput => "Invalid frequency input, filter type, or frequency unit. Please check your input.";
        public static string InvalidFilterType => "Invalid filter type.";
        public static string InvalidCapacitanceInput => "Invalid capacitance input or unit.";
        public static string InvalidInductanceInput => "Invalid inductance input or unit.";
        public static string InvalidResistanceInput => "Invalid resistance input or unit.";
        public static string InvalidInputValues => "Please select filter type and all units of measurement.";
        public static string InvalidFrequencyValue => "Invalid frequency value.";
        public static string InvalidCapacitanceValue => "Invalid capacitance value.";
        public static string InvalidInductanceValue => "Invalid inductance value.";
        public static string InvalidResistanceValue => "Invalid resistance value.";
    }
}
```

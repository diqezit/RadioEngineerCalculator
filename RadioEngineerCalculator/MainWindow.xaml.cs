using System.Windows;
using System.Windows.Controls;
using RadioEngineerCalculator.ViewModel;

namespace RadioEngineerCalculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadTabs();
        }

        private void LoadTabs()
        {
            MainTabControl.Items.Add(new TabItem { Header = "Усилитель", Content = new AmplifierTab() });
            MainTabControl.Items.Add(new TabItem { Header = "Антенна", Content = new AntennaTab() });
            MainTabControl.Items.Add(new TabItem { Header = "Согласование импедансов", Content = new ImpedanceMatchingTab() });
            MainTabControl.Items.Add(new TabItem { Header = "Аттенюатор", Content = new AttenuatorTab() });
            MainTabControl.Items.Add(new TabItem { Header = "Коаксиальный кабель", Content = new CoaxialCableTab() });
            MainTabControl.Items.Add(new TabItem { Header = "LC Фильтр", Content = new LRCFilterTab() });
            MainTabControl.Items.Add(new TabItem { Header = "Калькулятор мощности", Content = new PowerCalculationTab() });
            MainTabControl.Items.Add(new TabItem { Header = "AM, FM, PM", Content = new ModulationTab() });
            MainTabControl.Items.Add(new TabItem { Header = "Закон Ома", Content = new OhmTab() });


        }
    }
}
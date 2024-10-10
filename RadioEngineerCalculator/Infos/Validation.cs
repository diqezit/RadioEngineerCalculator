using RadioEngineerCalculator.Infos;
using System.Windows;
using static RadioEngineerCalculator.Services.FiltersCalculationService;

namespace RadioEngineerCalculator.Services
{
    public class InputValidator
    {
        public static bool ValidateInputValues(FilterInputValues inputValues)
        {
            bool isValid = true;

            // Проверка на положительные значения
            if (!AreValuesPositive(inputValues.Frequency, inputValues.PassbandRipple, inputValues.StopbandAttenuation, inputValues.StopbandFrequency))
            {
                MessageBox.Show(ErrorMessages.InvalidInputValues, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            // Проверка компонентных значений
            if (!ValidateComponentValues(inputValues))  // Вызов исправлен, теперь метод существует
            {
                isValid = false;
            }

            // Проверка диапазонов частот
            if (inputValues.Frequency <= inputValues.StopbandFrequency)
            {
                MessageBox.Show(ErrorMessages.FrequencyTooLow, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            // Проверка валидности значений добротности и полосы пропускания
            if (inputValues.PassbandRipple < 0)
            {
                MessageBox.Show(ErrorMessages.InvalidPassbandRippleValue, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (inputValues.StopbandAttenuation < 0)
            {
                MessageBox.Show(ErrorMessages.InvalidStopbandAttenuationValue, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            return isValid;
        }

        // Метод проверки положительности значений
        private static bool AreValuesPositive(params double[] values)
        {
            foreach (var value in values)
            {
                if (value <= 0)
                    return false;
            }
            return true;
        }

        // Исправленный метод TryParseComponentValues
        private static bool TryParseComponentValues(FilterType filterType, double capacitance, double inductance, double resistance)
        {
            bool isValid = true;

            if (filterType != FilterType.RL && capacitance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidCapacitanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (filterType != FilterType.RC && inductance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidInductanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (filterType != FilterType.Quartz && resistance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidResistanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            return isValid;
        }

        private static bool ValidateComponentValues(FilterInputValues inputValues)
        {
            bool isValid = true;

            if (inputValues.FilterType != FilterType.RL && inputValues.Capacitance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidCapacitanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (inputValues.FilterType != FilterType.RC && inputValues.Inductance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidInductanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (inputValues.FilterType != FilterType.Quartz && inputValues.Resistance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidResistanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            return isValid;
        }
    }
}

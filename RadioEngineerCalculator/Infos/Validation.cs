using RadioEngineerCalculator.Infos;
using System.Windows;

namespace RadioEngineerCalculator.Services
{
    public class InputValidator
    {
        // Метод для проверки положительных значений
        private static bool AreValuesPositive(params double[] values)
        {
            foreach (var value in values)
            {
                if (value <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ValidateInputValues(FilterInputValues inputValues)
        {
            // Проверка на null
            if (inputValues == null)
            {
                MessageBox.Show("Не заданы входные значения", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Если частоты и другие поля еще не заданы, пропускаем валидацию
            if (inputValues.Frequency == 0 && inputValues.PassbandRipple == 0 &&
                inputValues.StopbandAttenuation == 0 && inputValues.StopbandFrequency == 0)
            {
                return true; // Поля еще не были введены
            }

            // Проверка на ненулевые значения только после того, как все поля введены
            if (!AreValuesPositive(inputValues.Frequency, inputValues.PassbandRipple,
                                   inputValues.StopbandAttenuation, inputValues.StopbandFrequency))
            {
                MessageBox.Show(ErrorMessages.InvalidInputValues, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка на корректность частоты
            if (inputValues.Frequency <= inputValues.StopbandFrequency)
            {
                MessageBox.Show(ErrorMessages.FrequencyTooLow, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка на компоненты, если они установлены
            if (inputValues.Capacitance > 0 || inputValues.Inductance > 0 || inputValues.Resistance > 0)
            {
                if (!ValidateComponentValues(inputValues))
                {
                    return false;
                }
            }

            return true;
        }

        // Валидация значений компонентов
        private static bool ValidateComponentValues(FilterInputValues inputValues)
        {
            bool isValid = true;

            if (inputValues.Capacitance < 0)
            {
                MessageBox.Show(ErrorMessages.InvalidCapacitanceInput, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (inputValues.Inductance < 0)
            {
                MessageBox.Show(ErrorMessages.InvalidInductanceInput, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (inputValues.Resistance < 0)
            {
                MessageBox.Show(ErrorMessages.InvalidResistanceInput, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            return isValid;
        }

        #region Метод для контуров

        public static bool ValidateResonantCircuitInput(double inductance, double capacitance, double resistance, double frequency)
        {
            if (inductance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidInductanceInput, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (capacitance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidCapacitanceInput, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (resistance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidResistanceInput, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (frequency <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidFrequencyInput, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion
    }
}

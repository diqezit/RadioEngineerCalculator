using RadioEngineerCalculator.Infos;
using System.Windows;
using static RadioEngineerCalculator.Services.FiltersCalculationService;

namespace RadioEngineerCalculator.Services
{
    public class InputValidator
    {

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
            if (inputValues == null)
            {
                MessageBox.Show(ErrorMessages.InvalidInputValues, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!AreValuesPositive(inputValues.Frequency, inputValues.PassbandRipple, inputValues.StopbandAttenuation, inputValues.StopbandFrequency))
            {
                MessageBox.Show(ErrorMessages.InvalidInputValues, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!ValidateComponentValues(inputValues))
            {
                return false;
            }

            if (inputValues.Frequency <= inputValues.StopbandFrequency)
            {
                MessageBox.Show(ErrorMessages.FrequencyTooLow, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(inputValues.StopbandFrequencyUnit))
            {
                MessageBox.Show(ErrorMessages.InvalidStopbandFrequencyUnit, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public static bool TryParseDouble(string input, out double result)
        {
            return double.TryParse(input, out result);
        }

        public static bool ValidatePositiveValue(double value, out string errorMessage)
        {
            if (value <= 0)
            {
                errorMessage = "Value must be positive.";
                return false;
            }
            errorMessage = null;
            return true;
        }


        private static bool ValidateComponentValues(FilterInputValues inputValues)
        {
            bool isValid = true;

            if (inputValues.Capacitance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidCapacitanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (inputValues.Inductance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidInductanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (inputValues.Resistance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidResistanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            return isValid;
        }

        #region Метод для контуров

        public static bool ValidateResonantCircuitInput(double inductance, double capacitance, double resistance, double frequency)
        {
            if (inductance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidInductanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (capacitance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidCapacitanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (resistance <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidResistanceInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (frequency <= 0)
            {
                MessageBox.Show(ErrorMessages.InvalidFrequencyInput, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RadioEngineerCalculator.Services
{
    public static class Validate
    {
        #region Object Validation

        public static void ValidateObject<T>(T obj) where T : class
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                ValidateProperty(property.Name, property.GetValue(obj));
            }
        }

        public static void ValidateProperty(string propertyName, object value)
        {
            _ = value ?? throw new ArgumentNullException(propertyName);

            switch (value)
            {
                case double doubleValue:
                    EnsurePositive(doubleValue, propertyName);
                    break;
                case string stringValue:
                    ValidateStringNotEmpty(stringValue, propertyName);
                    break;
            }
        }

        #endregion

        #region Input Validation

        public static void InputValues(FilterInputValues inputValues)
        {
            _ = inputValues ?? throw new ArgumentNullException(nameof(inputValues));
            EnsurePositive(inputValues.Capacitance, nameof(inputValues.Capacitance));
            EnsurePositive(inputValues.Inductance, nameof(inputValues.Inductance));
            EnsurePositive(inputValues.Resistance, nameof(inputValues.Resistance));
            EnsurePositive(inputValues.Frequency, nameof(inputValues.Frequency));
        }

        public static bool ValidateInputs(params string[] inputs) =>
            inputs.All(input => double.TryParse(input, out var value) && value > 0);

        public static bool InputsAreValid(params double[] values) =>
            values.All(value => value > 0 && !double.IsNaN(value) && !double.IsInfinity(value));

        public static bool InputsAreValid(double capacitance, double inductance, double resistance, double frequency) =>
            capacitance > 0 && inductance > 0 && resistance > 0 && frequency > 0;

        #endregion

        #region Value Validation

        public static void EnsurePositive(double value, string paramName)
        {
            if (value <= 0)
                throw new ArgumentException($"{paramName} должен быть положительным числом");
        }

        public static bool IsInRange(double value, double min, double max) =>
            value >= min && value <= max;

        public static void ValidateRange(double value, double min, double max, string paramName)
        {
            if (!IsInRange(value, min, max))
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} должен быть между {min} и {max}.");
        }

        public static void ValidateNonNegative(double value, string paramName)
        {
            if (value < 0)
                throw new ArgumentException($"{paramName} не может быть отрицательным.");
        }

        public static void ValidateStringNotEmpty(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} не может быть пустым или состоять только из пробелов.");
        }

        public static void ValidateEnumValue<T>(T value, string paramName) where T : struct, Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new ArgumentException($"Недопустимое значение для {paramName}.");
        }

        public static void ValidateListNotEmpty<T>(IEnumerable<T> list, string paramName)
        {
            if (list is null || !list.Any())
                throw new ArgumentException($"{paramName} не может быть пустым или null.");
        }

        #endregion
    }
}
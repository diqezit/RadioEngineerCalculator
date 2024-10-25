using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RadioEngineerCalculator.Services
{
    /// <summary>
    /// Статический класс, предоставляющий методы для валидации различных типов данных и объектов.
    /// </summary>
    public static class Validate
    {
        #region Object Validation

        /// <summary>
        /// Валидирует объект, проверяя все его публичные свойства.
        /// </summary>
        /// <typeparam name="T">Тип объекта, который нужно валидировать.</typeparam>
        /// <param name="obj">Объект для валидации.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если объект равен null.</exception>
        public static void ValidateObject<T>(T obj) where T : class
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                ValidateProperty(property.Name, property.GetValue(obj));
            }
        }

        /// <summary>
        /// Валидирует значение свойства объекта.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        /// <param name="value">Значение свойства.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если значение свойства равно null.</exception>
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

        /// <summary>
        /// Валидирует входные значения фильтра.
        /// </summary>
        /// <param name="inputValues">Объект, содержащий входные значения фильтра.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если объект равен null.</exception>
        public static void InputValues(FilterInputValues inputValues)
        {
            _ = inputValues ?? throw new ArgumentNullException(nameof(inputValues));
            EnsurePositive(inputValues.Capacitance, nameof(inputValues.Capacitance));
            EnsurePositive(inputValues.Inductance, nameof(inputValues.Inductance));
            EnsurePositive(inputValues.Resistance, nameof(inputValues.Resistance));
            EnsurePositive(inputValues.Frequency, nameof(inputValues.Frequency));
        }

        /// <summary>
        /// Проверяет, что все входные строки могут быть преобразованы в положительные числа типа double.
        /// </summary>
        /// <param name="inputs">Массив строк для валидации.</param>
        /// <returns>True, если все строки могут быть преобразованы в положительные числа типа double; в противном случае — False.</returns>
        public static bool ValidateInputs(params string[] inputs) =>
            inputs.All(input => double.TryParse(input, out var value) && value > 0);

        /// <summary>
        /// Проверяет, что все значения в массиве являются положительными числами типа double и не равны NaN или Infinity.
        /// </summary>
        /// <param name="values">Массив значений для валидации.</param>
        /// <returns>True, если все значения являются положительными числами типа double и не равны NaN или Infinity; в противном случае — False.</returns>
        public static bool InputsAreValid(params double[] values) =>
            values.All(value => value > 0 && !double.IsNaN(value) && !double.IsInfinity(value));

        /// <summary>
        /// Проверяет, что все указанные значения являются положительными числами типа double.
        /// </summary>
        /// <param name="capacitance">Значение емкости.</param>
        /// <param name="inductance">Значение индуктивности.</param>
        /// <param name="resistance">Значение сопротивления.</param>
        /// <param name="frequency">Значение частоты.</param>
        /// <returns>True, если все значения являются положительными числами типа double; в противном случае — False.</returns>
        public static bool InputsAreValid(double capacitance, double inductance, double resistance, double frequency) =>
            capacitance > 0 && inductance > 0 && resistance > 0 && frequency > 0;

        #endregion

        #region Value Validation

        /// <summary>
        /// Проверяет, что значение типа double является положительным.
        /// </summary>
        /// <param name="value">Значение для проверки.</param>
        /// <param name="paramName">Имя параметра, который проверяется.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если значение не является положительным.</exception>
        public static void EnsurePositive(double value, string paramName)
        {
            if (value <= 0)
                throw new ArgumentException($"{paramName} должен быть положительным числом");
        }

        /// <summary>
        /// Проверяет, что значение типа double находится в указанном диапазоне.
        /// </summary>
        /// <param name="value">Значение для проверки.</param>
        /// <param name="min">Минимальное значение диапазона.</param>
        /// <param name="max">Максимальное значение диапазона.</param>
        /// <returns>True, если значение находится в указанном диапазоне; в противном случае — False.</returns>
        public static bool IsInRange(double value, double min, double max) =>
            value >= min && value <= max;

        /// <summary>
        /// Проверяет, что значение типа double находится в указанном диапазоне.
        /// </summary>
        /// <param name="value">Значение для проверки.</param>
        /// <param name="min">Минимальное значение диапазона.</param>
        /// <param name="max">Максимальное значение диапазона.</param>
        /// <param name="paramName">Имя параметра, который проверяется.</param>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если значение не находится в указанном диапазоне.</exception>
        public static void ValidateRange(double value, double min, double max, string paramName)
        {
            if (!IsInRange(value, min, max))
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} должен быть между {min} и {max}.");
        }

        /// <summary>
        /// Проверяет, что значение типа double не является отрицательным.
        /// </summary>
        /// <param name="value">Значение для проверки.</param>
        /// <param name="paramName">Имя параметра, который проверяется.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если значение является отрицательным.</exception>
        public static void ValidateNonNegative(double value, string paramName)
        {
            if (value < 0)
                throw new ArgumentException($"{paramName} не может быть отрицательным.");
        }

        /// <summary>
        /// Проверяет, что строка не является пустой или состоит только из пробелов.
        /// </summary>
        /// <param name="value">Строка для проверки.</param>
        /// <param name="paramName">Имя параметра, который проверяется.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если строка пуста или состоит только из пробелов.</exception>
        public static void ValidateStringNotEmpty(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} не может быть пустым или состоять только из пробелов.");
        }

        /// <summary>
        /// Проверяет, что значение является допустимым значением перечисления.
        /// </summary>
        /// <typeparam name="T">Тип перечисления.</typeparam>
        /// <param name="value">Значение для проверки.</param>
        /// <param name="paramName">Имя параметра, который проверяется.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если значение не является допустимым значением перечисления.</exception>
        public static void ValidateEnumValue<T>(T value, string paramName) where T : struct, Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new ArgumentException($"Недопустимое значение для {paramName}.");
        }

        /// <summary>
        /// Проверяет, что список не является пустым или null.
        /// </summary>
        /// <typeparam name="T">Тип элементов списка.</typeparam>
        /// <param name="list">Список для проверки.</param>
        /// <param name="paramName">Имя параметра, который проверяется.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если список пуст или равен null.</exception>
        public static void ValidateListNotEmpty<T>(IEnumerable<T> list, string paramName)
        {
            if (list is null || !list.Any())
                throw new ArgumentException($"{paramName} не может быть пустым или null.");
        }

        /// <summary>
        /// Проверяет, что значение типа double является положительным.
        /// </summary>
        /// <param name="value">Значение для проверки.</param>
        /// <param name="paramName">Имя параметра, который проверяется (опционально).</param>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если значение не является положительным и имя параметра не указано.</exception>
        /// <exception cref="ArgumentException">Выбрасывается, если значение не является положительным и имя параметра указано.</exception>
        public static void ValidatePositive(double value, string paramName = null)
        {
            if (value <= 0)
            {
                if (string.IsNullOrEmpty(paramName))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Значение должно быть положительным.");
                }
                else
                {
                    throw new ArgumentException($"{paramName} должно быть положительным числом.", paramName);
                }
            }
        }
        #endregion
    }
}
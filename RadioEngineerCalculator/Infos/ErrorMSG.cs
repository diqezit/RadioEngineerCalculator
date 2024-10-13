namespace RadioEngineerCalculator.Infos
{
    public static class ErrorMessages
    {
        #region Общие ошибки
        public const string InvalidFormat = "Некорректный формат данных.";
        public const string InvalidInputFormat = "Некорректный формат входных данных.";
        public const string EAC = "Ошибка калькуляции.";
        public const string CalculationError = "Произошла ошибка при расчете:";
        public const string InvalidInput = "Проверьте ввод данных: все значения должны быть положительными.";
        public const string FieldsAreEmpty = "Пожалуйста, заполните все поля.";
        public const string OperationFailed = "Операция завершилась неудачно.";
        public const string UnknownError = "Произошла неизвестная ошибка.";
        public const string ResourceNotFound = "Запрашиваемый ресурс не найден.";
        public const string DataCorruption = "Обнаружена ошибка в данных.";
        public const string InsufficientPermissions = "Недостаточно прав для выполнения операции.";
        public const string FileNotFound = "Файл не найден.";
        public const string OperationCanceled = "Операция отменена пользователем.";
        public const string ConfigurationError = "Ошибка конфигурации приложения.";
        public const string InitializationFailed = "Не удалось инициализировать приложение.";
        public const string OperationTimeout = "Время выполнения операции превышено.";
        public const string UnsupportedOperation = "Операция не поддерживается.";
        public const string InvalidParameter = "Некорректный параметр.";
        public const string UnexpectedError = "Неожиданная ошибка.";
        public const string UserNotAuthenticated = "Пользователь не аутентифицирован.";
        public const string UserSessionExpired = "Сессия пользователя истекла.";
        #endregion


        #region Ошибки частоты
        public const string FrequencyTooLow = "Частота очень мала.";
        public const string InvalidFrequencyInput = "Неверный ввод частоты, тип фильтра или единица измерения частоты. Пожалуйста, проверьте ваш ввод.";
        public const string InvalidFrequencyValue = "Неверное значение частоты.";
        public const string InvalidStopbandFrequencyUnit = "Неверное значение частоты полосы заграждения.";
        public const string FrequencyOutOfRange = "Частота вне допустимого диапазона.";
        public const string FrequencyUnitMismatch = "Несоответствие единицы измерения частоты.";
        public const string FrequencyCalculationError = "Ошибка вычисления частоты.";
        public const string FrequencyAlreadyUsed = "Эта частота уже используется.";
        public const string FrequencyResolutionError = "Ошибка разрешения частоты.";
        public const string FrequencySpectrumError = "Ошибка спектра частот.";
        public const string FrequencyDependencyMissing = "Отсутствует зависимость для частоты.";
        #endregion

        #region Ошибки емкости
        public const string InvalidCapacitanceInput = "Неверный ввод емкости или единицы измерения.";
        public const string InvalidCapacitanceValue = "Неверное значение емкости.";
        public const string CapacitanceTooHigh = "Значение емкости слишком велико.";
        public const string CapacitanceOutOfRange = "Емкость вне допустимого диапазона.";
        public const string InvalidCapacitanceUnit = "Неверная единица измерения емкости.";
        public const string CapacitanceFrequencyError = "Ошибка частоты при расчете емкости.";
        public const string CapacitanceComponentMismatch = "Несоответствие компонентов емкости.";
        public const string CapacitanceConfigurationError = "Ошибка конфигурации емкости.";
        public const string CapacitanceCalculationError = "Ошибка вычисления емкости.";
        public const string CapacitanceDependencyMissing = "Отсутствует зависимость для емкости.";
        #endregion

        #region Ошибки индуктивности
        public const string InvalidInductanceInput = "Неверный ввод индуктивности или единицы измерения.";
        public const string InvalidInductanceValue = "Неверное значение индуктивности.";
        public const string InductanceTooLow = "Значение индуктивности слишком мало.";
        public const string InductanceOutOfRange = "Индуктивность вне допустимого диапазона.";
        public const string InvalidInductanceUnit = "Неверная единица измерения индуктивности.";
        public const string InductanceFrequencyError = "Ошибка частоты при расчете индуктивности.";
        public const string InductanceComponentMismatch = "Несоответствие компонентов индуктивности.";
        public const string InductanceConfigurationError = "Ошибка конфигурации индуктивности.";
        public const string InductanceCalculationError = "Ошибка вычисления индуктивности.";
        public const string InductanceDependencyMissing = "Отсутствует зависимость для индуктивности.";
        #endregion

        #region Ошибки сопротивления
        public const string InvalidResistanceInput = "Неверный ввод сопротивления или единицы измерения.";
        public const string InvalidResistanceValue = "Неверное значение сопротивления.";
        public const string ResistanceTooHigh = "Значение сопротивления слишком велико.";
        public const string ResistanceOutOfRange = "Сопротивление вне допустимого диапазона.";
        public const string InvalidResistanceUnit = "Неверная единица измерения сопротивления.";
        public const string ResistanceFrequencyError = "Ошибка частоты при расчете сопротивления.";
        public const string ResistanceComponentMismatch = "Несоответствие компонентов сопротивления.";
        public const string ResistanceConfigurationError = "Ошибка конфигурации сопротивления.";
        public const string ResistanceCalculationError = "Ошибка вычисления сопротивления.";
        public const string ResistanceDependencyMissing = "Отсутствует зависимость для сопротивления.";
        #endregion

        #region Ошибки фильтра
        public const string InvalidComponentValue = "Компоненты фильтра и частота должны иметь положительные значения.";
        public const string InvalidFilterType = "Неверный тип фильтра.";
        public const string InvalidStopbandAttenuationValue = "Неверное значение затухания в полосе заграждения.";
        public const string InvalidInputValues = "Пожалуйста, выберите тип фильтра и все единицы измерения.";
        public const string CheckInput = "Неправильный числовой ввод.";
        public const string CheckComboBox = "Пожалуйста, выберите корректное значение в ячейке.";
        public const string FilterDesignError = "Ошибка при проектировании фильтра.";
        public const string FilterComponentMismatch = "Несоответствие компонентов фильтра.";
        public const string FilterOrderError = "Ошибка в порядке фильтра.";
        public const string FilterConfigurationError = "Ошибка конфигурации фильтра.";
        public const string FilterCalculationError = "Ошибка вычисления фильтра.";
        public const string FilterDependencyMissing = "Отсутствует зависимость для фильтра.";
        public const string InvalidFilterSettings = "Некорректные настройки фильтра.";
        public const string FilterDataError = "Ошибка в данных фильтра.";
        public const string FilterResponseError = "Ошибка в ответе фильтра.";
        #endregion

        #region Дополнительные ошибки
        public const string InvalidPositiveNumberInput = "Пожалуйста, заполните все необходимые поля.";
        public const string InvalidConfiguration = "Некорректная конфигурация параметров.";
        public const string DataProcessingError = "Ошибка обработки данных.";
        public const string NetworkError = "Ошибка сети.";
        public const string TimeoutError = "Превышено время ожидания.";
        public const string ServiceUnavailable = "Служба недоступна.";
        public const string OperationNotPermitted = "Данная операция не разрешена.";
        public const string MemoryOverflow = "Переполнение памяти.";
        #endregion
    }
}

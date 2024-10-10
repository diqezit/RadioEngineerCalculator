namespace RadioEngineerCalculator.Infos
{
    public static class ErrorMessages
    {
        public static string InvalidInput => "Неверный ввод. Пожалуйста, введите корректные числа и выберите единицы измерения.";
        public static string InvalidFrequencyInput => "Неверный ввод частоты, тип фильтра или единица измерения частоты. Пожалуйста, проверьте ваш ввод.";
        public static string InvalidFilterType => "Неверный тип фильтра.";
        public static string InvalidCapacitanceInput => "Неверный ввод емкости или единицы измерения.";
        public static string InvalidInductanceInput => "Неверный ввод индуктивности или единицы измерения.";
        public static string InvalidResistanceInput => "Неверный ввод сопротивления или единицы измерения.";
        public static string InvalidInputValues => "Пожалуйста, выберите тип фильтра и все единицы измерения.";
        public static string InvalidFrequencyValue => "Неверное значение частоты.";
        public static string InvalidCapacitanceValue => "Неверное значение емкости.";
        public static string InvalidInductanceValue => "Неверное значение индуктивности.";
        public static string InvalidResistanceValue => "Неверное значение сопротивления.";
        public static string InvalidComponentValue => "Компоненты фильтра и частота должны иметь положительные значения.";
        public static string InvalidPassbandRippleValue => "Неверное значение неравномерности АЧХ в полосе пропускания.";
        public static string InvalidStopbandAttenuationValue => "Неверное значение затухания в полосе заграждения.";
        public static string InvalidStopbandFrequencyValue => "Неверное значение частоты полосы заграждения.";
        public static string MissingInputValues => "Пожалуйста, заполните все необходимые поля";
        public static string EAC => "Ошибка калькуляции.";
        public static string FrequencyTooLow => "Частота очень мала.";



    }
}

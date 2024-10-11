namespace RadioEngineerCalculator.Infos
{
    public static class Info
    {

        public static string InvalidFieldError => "Неизвестная ошибка полей ввода.";
        public static string VIErrorNegative => "Напряжение и ток не могут быть отрицательными.";

        public static string VRErrorNegative => "Ошибка: Напряжение и сопротивление не могут быть отрицательными.";

        public static string RIErrorNegative => "Ошибка: Сопротивление и ток не могут быть отрицательными.";

        public static string FieldAreFull => "Ошибка: Все поля заполнены. Оставьте одно поле пустым для расчета.";
    }
}
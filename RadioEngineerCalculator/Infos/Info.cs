using RadioEngineerCalculator.Services;
using static RadioEngineerCalculator.Services.FiltersCalculationService;

namespace RadioEngineerCalculator.Infos
{
    public static class Info
    {

        public static string InvalidFieldError => "Неизвестная ошибка полей ввода.";
        public static string VIErrorNegative => "Напряжение и ток не могут быть отрицательными.";

        public static string VRErrorNegative => "Ошибка: Напряжение и сопротивление не могут быть отрицательными.";

        public static string RIErrorNegative => "Ошибка: Сопротивление и ток не могут быть отрицательными.";

        public static string FieldAreFull => "Ошибка: Все поля заполнены. Оставьте одно поле пустым для расчета.";


        public static string LowPassFilterDescription => "Фильтр нижних частот ослабляет частоты выше частоты среза.";

        public static string HighPassFilterDescription => "Фильтр верхних частот ослабляет частоты ниже частоты среза.";

        public static string BandPassFilterDescription => "Полосовой фильтр пропускает определенный диапазон частот.";

        public static string BandStopFilterDescription => "Режекторный фильтр ослабляет определенный диапазон частот.";

        public static string RCFilterDescription => "RC-фильтр использует резистор и конденсатор для фильтрации сигналов.";

        public static string RLFilterDescription => "RL-фильтр использует резистор и индуктивность для фильтрации сигналов.";

        public static string QuartzFilterDescription => "Кварцевый фильтр использует кварцевый кристалл для высокоточной фильтрации частот.";
        public static string PiFilterDescription => "Пи-фильтр состоит из двух параллельных конденсаторов и последовательной индуктивности, обеспечивая хорошее подавление высоких частот.";
        public static string TrapFilterDescription => "Режекторный фильтр использует параллельный LC-контур для подавления определенной частоты.";

        public static string GetAdditionalInfo(FilterType filterType, double cutoffFrequency, double bandwidth)
        {
            switch (filterType)
            {
            
                case FilterType.LowPass:
                    return "Пропускает частоты ниже частоты среза";
                case FilterType.HighPass:
                    return "Пропускает частоты выше частоты среза";
                case FilterType.BandPass:
                case FilterType.PassiveBandPass:
                    return $"Пропускает частоты между {UnitC.Form.Frequency(cutoffFrequency - bandwidth / 2)} и {UnitC.Form.Frequency(cutoffFrequency + bandwidth / 2)}";
                case FilterType.BandStop:
                case FilterType.PassiveBandStop:
                    return $"Ослабляет частоты между {UnitC.Form.Frequency(cutoffFrequency - bandwidth / 2)} и {UnitC.Form.Frequency(cutoffFrequency + bandwidth / 2)}";
                case FilterType.Pi:
                    return "Обеспечивает более крутой спад АЧХ по сравнению с простыми RC или RL фильтрами";
                case FilterType.Trap:
                    return $"Подавляет частоту {UnitC.Form.Frequency(cutoffFrequency)}";
                default:
                    return "Дополнительная информация недоступна для этого типа фильтра";
            }
        }
    }
}
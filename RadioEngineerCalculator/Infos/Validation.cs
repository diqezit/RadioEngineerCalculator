using System;

namespace RadioEngineerCalculator.Services
{
    public static class Validate
    {
        public static bool InputsAreValid(double capacitance, double inductance, double resistance, double frequency)
        {
            return capacitance > 0 && inductance > 0 && resistance > 0 && frequency > 0;
        }
        public static void EnsurePositive(double value, string paramName)
        {
            if (value <= 0)
                throw new ArgumentException($"{paramName} должен быть больше нуля.");
        }
    }
}

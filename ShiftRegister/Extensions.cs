namespace ShiftRegister
{
    public static class Extensions
    {
        public static double Map(double value, double inMin, double inMax, double outMin, double outMax)
        {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }
}

namespace Cygnus.Models
{
    namespace Constants
    {
        public static class Velocities{
            public const double MaximumImperial = 0.3543d;
            public const double MinimumImperial = 0.0394d;
            public const double MaximumMetric = 9000d;
            public const double MinimumMetric = 1000d;
        }

        public static class Measurements
        {
            public const int MaxNumberOfMeasurementsInARecord = 5000;
            public const double MaxMeasurementThicknessMetric = 1500d;
            public const double MaxMeasurementThicknessImperial = 4d;
        }
    }
}

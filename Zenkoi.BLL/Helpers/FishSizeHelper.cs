using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.Helpers
{
    public static class FishSizeHelper
    {
        /// <summary>
        /// Returns the representative size in cm for a FishSize enum.
        /// Uses mid-point of each range for more accurate box calculations.
        /// </summary>
        public static int GetSizeInCm(FishSize fishSize)
        {
            return fishSize switch
            {
                FishSize.From0To19cm => 10,                 // Mid-point of 0-19cm
                FishSize.From20To25cm => 22,                // Mid-point of 20-25cm
                FishSize.From25_1To30cm => 28,              // Mid-point of 25.1-30cm
                FishSize.From30_1To40cm => 35,              // Mid-point of 30.1-40cm
                FishSize.From40_1To44cm => 42,              // Mid-point of 40.1-44cm
                FishSize.From44_1To50cm => 47,              // Mid-point of 44.1-50cm
                FishSize.From50_1To55cm => 53,              // Mid-point of 50.1-55cm
                FishSize.From50_1To60cm_Hirenaga => 55,     // Mid-point of 50.1-60cm Hirenaga
                FishSize.From55_1To60cm => 58,              // Mid-point of 55.1-60cm
                FishSize.From60_1To65cm => 63,              // Mid-point of 60.1-65cm
                FishSize.From60_1To65cm_Hirenaga => 63,     // Mid-point of 60.1-65cm Hirenaga
                FishSize.From65_1To73cm => 69,              // Mid-point of 65.1-73cm
                FishSize.From73_1To83cm => 78,              // Mid-point of 73.1-83cm
                FishSize.Over83_1cm => 90,                  // Representative size for 83.1+cm
                _ => 22
            };
        }

        public static decimal GetSizeInInch(FishSize fishSize)
        {
            return GetSizeInCm(fishSize) / 2.54m;
        }

        /// <summary>
        /// Converts a size in cm (double) to the appropriate FishSize enum category.
        /// </summary>
        public static FishSize GetFishSizeFromCm(double sizeCm)
        {
            return sizeCm switch
            {
                >= 0 and <= 19 => FishSize.From0To19cm,
                > 19 and <= 25 => FishSize.From20To25cm,
                > 25 and <= 30 => FishSize.From25_1To30cm,
                > 30 and <= 40 => FishSize.From30_1To40cm,
                > 40 and <= 44 => FishSize.From40_1To44cm,
                > 44 and <= 50 => FishSize.From44_1To50cm,
                > 50 and <= 55 => FishSize.From50_1To55cm,
                > 55 and <= 60 => FishSize.From55_1To60cm,
                > 60 and <= 65 => FishSize.From60_1To65cm,
                > 65 and <= 73 => FishSize.From65_1To73cm,
                > 73 and <= 83 => FishSize.From73_1To83cm,
                > 83 => FishSize.Over83_1cm,
                _ => FishSize.From20To25cm  // Default fallback
            };
        }

        /// <summary>
        /// Converts a size in cm (double) to the appropriate FishSize enum category for Hirenaga/Butterfly types.
        /// </summary>
        public static FishSize GetFishSizeFromCm_Hirenaga(double sizeCm)
        {
            return sizeCm switch
            {
                >= 0 and <= 19 => FishSize.From0To19cm,
                > 19 and <= 25 => FishSize.From20To25cm,
                > 25 and <= 30 => FishSize.From25_1To30cm,
                > 30 and <= 40 => FishSize.From30_1To40cm,
                > 40 and <= 44 => FishSize.From40_1To44cm,
                > 44 and <= 50 => FishSize.From44_1To50cm,
                > 50 and <= 60 => FishSize.From50_1To60cm_Hirenaga,
                > 60 and <= 65 => FishSize.From60_1To65cm_Hirenaga,
                > 65 and <= 73 => FishSize.From65_1To73cm,
                > 73 and <= 83 => FishSize.From73_1To83cm,
                > 83 => FishSize.Over83_1cm,
                _ => FishSize.From20To25cm  // Default fallback
            };
        }
    }
}

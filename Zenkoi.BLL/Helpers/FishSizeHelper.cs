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
                FishSize.Under10cm => 8,        // Representative size for under 10cm
                FishSize.From10To20cm => 15,    // Mid-point of 10-20cm range
                FishSize.From21To25cm => 23,    // Mid-point of 21-25cm range
                FishSize.From26To30cm => 28,    // Mid-point of 26-30cm range
                FishSize.From31To40cm => 35,    // Mid-point of 31-40cm range
                FishSize.From41To45cm => 43,    // Mid-point of 41-45cm range
                FishSize.From46To50cm => 48,    // Mid-point of 46-50cm range
                FishSize.Over50cm => 55,        // Representative size for over 50cm
                _ => 15
            };
        }

        public static decimal GetSizeInInch(FishSize fishSize)
        {
            return GetSizeInCm(fishSize) / 2.54m;
        }
    }
}

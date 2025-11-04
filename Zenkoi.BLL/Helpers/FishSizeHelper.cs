using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.Helpers
{
    public static class FishSizeHelper
    {
        public static int GetSizeInCm(FishSize fishSize)
        {
            return fishSize switch
            {
                FishSize.Under10cm => 10,
                FishSize.From10To20cm => 20,
                FishSize.From21To25cm => 25,
                FishSize.From26To30cm => 30,
                FishSize.From31To40cm => 40,
                FishSize.From41To45cm => 45,
                FishSize.From46To50cm => 50,
                FishSize.Over50cm => 60,
                _ => 10
            };
        }

        public static decimal GetSizeInInch(FishSize fishSize)
        {
            return GetSizeInCm(fishSize) / 2.54m;
        }
    }
}

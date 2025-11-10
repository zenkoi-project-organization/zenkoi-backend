using System.ComponentModel.DataAnnotations;

namespace Zenkoi.DAL.Enums
{
    public enum KoiPatternType
    {
        None,          // Không xác định

        Tancho,        // Đốm đỏ giữa đầu
        Maruten,       // Đốm đầu + thân
        Nidan,         // 2 đốm đỏ
        Sandan,        // 3 đốm đỏ
        Inazuma,       // Dải đỏ hình tia sét
        StraightHi,    // Dải đỏ liền thân
        Menkaburi,     // Đầu đỏ toàn phần
        Bozu           // Đầu trắng
    }
}

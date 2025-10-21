using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.Helpers.Mapper
{

    public class FishSizeConverter : ITypeConverter<double, FishSize>
    {
        public FishSize Convert(double source, FishSize destination, ResolutionContext context)
        {
            if (source < 10) return FishSize.Under10cm;
            if (source <= 20) return FishSize.From10To20cm;
            if (source <= 25) return FishSize.From21To25cm;
            if (source <= 30) return FishSize.From26To30cm;
            if (source <= 40) return FishSize.From31To40cm;
            if (source <= 45) return FishSize.From41To45cm;
            if (source <= 50) return FishSize.From46To50cm;
            return FishSize.Over50cm;
        }
    }
}
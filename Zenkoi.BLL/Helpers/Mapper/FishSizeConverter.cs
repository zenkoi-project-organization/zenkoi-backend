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
            return FishSizeHelper.GetFishSizeFromCm(source);
        }
    }
}
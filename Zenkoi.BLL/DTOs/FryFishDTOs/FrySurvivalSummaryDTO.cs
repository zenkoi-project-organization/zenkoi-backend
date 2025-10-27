using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.FryFishDTOs
{
    public class FrySurvivalSummaryDTO
    {
        public string BreedingProcessCode { get; set; }
        public string PondName { get; set; }
        public int MaleKoi { get; set; }
        public int FemaleKoi { get; set; }
        public DateTime StartDate { get; set; }
        public int InitialCount { get; set; }

        public double? SurvivalRate7Days { get; set; }
        public double? SurvivalRate14Days { get; set; }
        public double? SurvivalRate30Days { get; set; }
        public double? CurrentRate { get; set; }
    }
}

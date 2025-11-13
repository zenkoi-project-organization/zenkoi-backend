namespace Zenkoi.BLL.DTOs.KoiReIDDTOs
{
    public class EnrollFromCloudinaryResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FishId { get; set; }
        public int NumImages { get; set; }
        public int NumUrlsProvided { get; set; }
        public int NumDownloaded { get; set; }
        public int TotalFishInGallery { get; set; }
        public int EnrollmentId { get; set; }
    }
}

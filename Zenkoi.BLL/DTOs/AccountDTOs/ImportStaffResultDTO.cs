namespace Zenkoi.BLL.DTOs.AccountDTOs
{
    public class ImportStaffResultDTO
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<StaffAccountResponseDTO> SuccessfulAccounts { get; set; } = new();
        public List<ImportErrorDTO> Errors { get; set; } = new();
    }

    public class ImportErrorDTO
    {
        public int RowNumber { get; set; }
        public string Email { get; set; }
        public string ErrorMessage { get; set; }
    }
}

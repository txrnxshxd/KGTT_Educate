namespace KGTT_Educate.Services.Account.Models.Dto
{
    public class RefreshTokenValidationResult
    {
        public bool IsValid { get; set; }
        public bool IsExpired { get; set; }
        public string UserId { get; set; }

        public static RefreshTokenValidationResult Invalid => new() { IsValid = false };
        public static RefreshTokenValidationResult Expired => new() { IsValid = false };
    }
}

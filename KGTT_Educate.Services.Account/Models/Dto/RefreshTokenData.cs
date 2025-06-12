namespace KGTT_Educate.Services.Account.Models.Dto
{
    public class RefreshTokenData
    {
        public string UserId { get; set; }
        public DateTime Expires { get; set; }
        public string DeviceFingerprint { get; set; }
    }
}

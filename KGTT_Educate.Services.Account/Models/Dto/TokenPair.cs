namespace KGTT_Educate.Services.Account.Models.Dto
{
    public class TokenPair
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
    }
}

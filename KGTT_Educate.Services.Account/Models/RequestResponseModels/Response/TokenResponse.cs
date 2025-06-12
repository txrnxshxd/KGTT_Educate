namespace KGTT_Educate.Services.Account.Models.RequestResponseModels.Response
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
    }
}

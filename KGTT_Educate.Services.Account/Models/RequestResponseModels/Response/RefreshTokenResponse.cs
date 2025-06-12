namespace KGTT_Educate.Services.Account.Models.RequestResponseModels.Response
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
    }
}

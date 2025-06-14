namespace KGTT_Educate.Services.Account.Models.Dto
{
    public class UserJwtDTO
    {
        public Guid Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Groups { get; set; }
        public IEnumerable<string> GroupNames { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
    }
}

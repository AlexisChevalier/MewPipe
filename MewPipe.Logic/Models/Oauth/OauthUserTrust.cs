namespace MewPipe.Logic.Models.Oauth
{
    public class OauthUserTrust
    {
        public int Id { get; set; }
        public virtual OauthClient OauthClient { get; set; }
        public virtual User User { get; set; }
        public string Scope { get; set; }
    }
}

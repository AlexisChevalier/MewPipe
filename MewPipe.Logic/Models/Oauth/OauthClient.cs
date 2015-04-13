namespace MewPipe.Logic.Models.Oauth
{
    public class OauthClient
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string ImageSrc { get; set; }
        public bool DialogDisabled { get; set; }
    }
}

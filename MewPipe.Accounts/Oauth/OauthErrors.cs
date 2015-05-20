namespace MewPipe.Accounts.Oauth
{
    public static class OauthErrors
    {
        public static OauthError ExpiredRefreshToken = new OauthError("expired_refresh_token", "Refresh tokens expires after 14 days", 481);
        public static OauthError InvalidRedirectionUri = new OauthError ("invalid_request", "Invalid redirection uri, see details", 400);
        public static OauthError RedirectionUriRequired = new OauthError ("invalid_request", "Redirection URI is required", 400);
        public static OauthError InvalidParameter = new OauthError ("invalid_request", "Invalid parameter(s), see details", 400);
        public static OauthError ParameterRequired = new OauthError ("invalid_request", "A/Some required parameter(s) is/are missing, see details", 400);
        public static OauthError InvalidResponseType = new OauthError ("invalid_request", "Response type must be code", 400);
        public static OauthError ResponseTypeIsRequired = new OauthError("invalid_request", "The request is missing a required parameter : response_type", 400);
    }

    public class OauthError
    {
        public OauthError(string textCode, string text, int httpCode)
        {
            TextCode = textCode;
            Text = text;
            HttpCode = httpCode;
        }

        public string TextCode { get; set; }
        public string Text { get; set; }
        public int HttpCode { get; set; }
        public string Details { get; set; }
    }
}
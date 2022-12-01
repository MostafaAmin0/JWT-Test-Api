namespace JWT_Test_Api.Helpers
{
    public class JWT
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInDays { get; set; }

        /*
        private readonly IConfiguration _configuration;
        private static JWT x;
        public JWT(IConfiguration configuration)
        {
            _configuration = configuration;

            x = JsonConvert.DeserializeObject<JWT>(configuration.GetSection("JWT").ToString());
        }
        public string Key { get; set; } = x.Key;
        public string Issuer { get; set; } = x.Issuer;
        public string Audience { get; set; } = x.Audience;
        public double DurationInDays { get; set; } = x.DurationInDays;
        */
    }
}

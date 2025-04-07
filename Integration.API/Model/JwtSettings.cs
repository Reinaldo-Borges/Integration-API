namespace Integration.API.Model
{
    public class JwtSettings
	{
        public string Secret { get; set; }
        public int ExpriresInHours { get; set; }
        public string Issuer { get; set; }
        public string ValidIn { get; set; }    
	}
}


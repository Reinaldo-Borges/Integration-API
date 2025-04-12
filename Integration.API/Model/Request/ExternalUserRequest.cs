using System;
namespace Integration.API.Model.Request
{
	public class ExternalUserRequest
	{
        public string Aud { get; set; }
        public string Azp { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public long Exp { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public long Iat { get; set; }
        public string Iss { get; set; }
        public string Jti { get; set; }
        public string Name { get; set; }
        public long Nbf { get; set; }
        public string Picture { get; set; }
        public string Sub { get; set; }
    }
}


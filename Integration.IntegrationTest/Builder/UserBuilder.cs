using System;
using Microsoft.AspNetCore.Identity;

namespace Integration.IntegrationTest.Builder
{
	public class UserBuilder
	{
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string NormalizedEmail => Email.ToUpper();
        public string UserName => Email;
        public string NormalizedUserName => UserName.ToUpper();
        public int EmailConfirmed => 1;
        public string PasswordHash { get; private set; }
        public string SecurityStamp => "BJAKGDBICMFW7PBYNWW7UB3MVQFTMDG3";
        public int PhoneNumberConfirmed => 0;
        public int TwoFactorEnabled => 0;
        public int LockoutEnabled => 1;
        public int AccessFailedCount => 0;

        public UserBuilder Build(Guid id, string email, string password)
        {
            return new UserBuilder()
                .SetId(id)
                .SetEmail(email)
                .SetPassword(password);
        }

        public UserBuilder SetId(Guid id)
        {
            Id = id;
            return this;
        }

        public UserBuilder SetEmail(string email)
        {
            Email = email;
            return this;
        }

        public UserBuilder SetPassword(string password)
        {
            var user = new IdentityUser
            {
                UserName = Email,
                Email = Email,
                EmailConfirmed = true
            };
            var passwordHasher = new PasswordHasher<IdentityUser>();
            PasswordHash = passwordHasher.HashPassword(user, password);

            return this;
        }
    }
}


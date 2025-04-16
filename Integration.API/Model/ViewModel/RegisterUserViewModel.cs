namespace Integration.API.Model.ViewModel
{
    public class RegisterUserViewModel
	{
        public Guid UserId { get; set; }
        public string Email { get; set; }

        public RegisterUserViewModel() { }

        public RegisterUserViewModel(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}
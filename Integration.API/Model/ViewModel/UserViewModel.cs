using Integration.Domain.Enum;

namespace Integration.API.Model.ViewModel
{
    public class UserViewModel
	{
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SecurityKey { get; set; }
        public String? Name { get; set; }
        public String? Email { get; set; }
        public String? Cellphone { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Document { get; set; }           
        public string? Country { get; set; }
        public int TypeStudentId { get; set; }
        public TypeStudentEnum TypeStudent => (TypeStudentEnum)TypeStudentId;
        public bool Active { get; set; }
        public String Token { get; set; }
        public DateTime ExpiresAt { get; set; }            
	}
}
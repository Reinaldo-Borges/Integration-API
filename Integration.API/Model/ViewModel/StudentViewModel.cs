using Integration.Domain.Enum;

namespace Integration.API.Model.ViewModel
{
    public class StudentViewModel
	{
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SecurityKey { get; set; }
        public string Document { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
        public string Country { get; set; }
        public DateTime? Birthday { get; set; }
        public TypeStudentEnum TypeStudent { get; set; }
        public StatusEntityEnum Active { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
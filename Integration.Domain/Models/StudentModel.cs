namespace Integration.Domain.Models
{
    public class StudentModel 
    {
        public int TypeStudent { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
        public string Country { get; set; }
        public DateTime Birthday { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public Guid SecurityKey { get; set; }
        public DateTime CreationDate { get; set; }
        public int Active { get; set; }
    }
}
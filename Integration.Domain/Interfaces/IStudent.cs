using Integration.Domain.Enum;

namespace Integration.Domain.Interfaces
{
    public interface IStudent
	{
        Guid Id { get; set; }
        string Name { get; set;}
        string? Document { get; set;}
        string Email { get; set;}
        string? Cellphone { get; set;}
        string? Country { get; set;}
        DateTime? Birthday { get; set;}
        Guid UserId { get; set;}
        TypeStudentEnum TypeStudent { get; set;}
        Guid? CourseId { get; set;}
        Guid? SecurityKey { get; set;}
    }
}
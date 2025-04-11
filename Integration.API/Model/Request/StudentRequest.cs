using System.ComponentModel.DataAnnotations;
using Integration.Domain.Enum;
using Integration.Domain.Interfaces;

namespace Integration.API.Model.Request
{
    public class StudentRequest : IStudent
    {
        [CustomAttributeNoGuidEmpty(ErrorMessage = "The field {0} is Guid.Empty")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public string Name { get; set; }

        public string? Document { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "The field {0} has incorrect value")]
        public string Email { get; set; }

        public string? Cellphone { get; set; }
        public string? Country { get; set; }
        public DateTime? Birthday { get; set; }

        [CustomAttributeNoGuidEmpty(ErrorMessage = "The field {0} is Guid.Empty")]
        public Guid UserId { get; set; }

        public Guid? CourseId { get; set; }
        public Guid? SecurityKey { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public TypeStudentEnum TypeStudent { get; set; }       
    }
}


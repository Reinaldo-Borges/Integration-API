using Integration.Domain.Enum;
using Integration.Domain.Validator;

namespace Integration.Domain.Entities
{
    public abstract class Student : Entity
	{
        public abstract TypeStudentEnum StudentType { get; }
        public string Name { get; private set; }
        public string Document { get; private set; }
        public string Email { get; private set; }
        public string Cellphone { get; private set; }
        public string Country { get; private set; }
        public DateTime? Birthday { get; private set; }
        public Guid UserId { get; private set; }

        protected Student(string name, string email, Guid userId)
        {
            Name = name;
            Email = email;
            UserId = userId;

            Validate(this, new StudentValidator());
        }

        public virtual void Activate() => StatusEntity = StatusEntityEnum.Active;
        public virtual void Inactivate() => StatusEntity = StatusEntityEnum.Inactive;
       
        public Student SetDocument(string document)
        {
            Document = document;
            return this;
        }
        public Student SetCellphone(string cellPhone)
        {
            Cellphone = cellPhone;
            return this;
        }
        public Student SetBirthday(DateTime? birthday)
        {
            Birthday = birthday;
            return this;
        }

        public Student SetCountry(string country)
        {
            Country = country;
            return this;
        }        
    }
}


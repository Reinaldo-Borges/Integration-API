
using FluentValidation;
using FluentValidation.Results;
using Integration.Domain.Enum;
using Integration.Domain.Exceptions;

namespace Integration.Domain.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
        public DateTime CreationDate { get; set; }
        public virtual StatusEntityEnum StatusEntity { get; set; }
        public ValidationResult ValidationResult { get; private set; }


        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        public T SetId<T>(Guid id) where T : Entity
        {
            if (id != Guid.Empty) Id = id;
            return this as T;
        }

        public T SetNew<T>() where T : Entity
        {
            CreationDate = DateTime.Now;
            StatusEntity = StatusEntityEnum.Active;

            return this as T;
        }

        public void Validate<T>(T entity, AbstractValidator<T> validator)
        {
            ValidationResult = validator.Validate(entity);
            if(!ValidationResult.IsValid)
                throw new DomainException(ValidationResult.Errors.First().ErrorMessage);
        }

    }
}


using System.ComponentModel.DataAnnotations;

namespace Integration.API.Model.Request
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class CustomAttributeNoGuidEmpty : ValidationAttribute
    {
        public override bool IsValid(Object value)
        {
            bool result = true;

            if ((Guid)value == Guid.Empty)
                result = false;

            return result;
        }
    }   
}


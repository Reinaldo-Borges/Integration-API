using System.Text.RegularExpressions;

namespace Integration.API.Extensions
{
    public static class ValidatorExtensions
	{
		public static bool IsValidEmail(this string email)
		{
            return Regex.IsMatch(email, @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z");
        }

		public static bool IsGuidNotEmpty(this Guid value)
		{
			return value != Guid.Empty ? true : false;
		}
	}
}


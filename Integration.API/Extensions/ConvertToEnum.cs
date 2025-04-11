using Integration.Domain.Enum;

namespace Integration.API.Extensions
{
    public static class ConvertToEnum
	{
		public static TypeStudentEnum ToTypeStudentEnum(this string profile)
		{
			switch (profile.ToUpper())
			{
				case "BASIC":
					return TypeStudentEnum.Basic;
				case "PARTNER":
					return TypeStudentEnum.Partner;
				case "PREMIUM":
					return TypeStudentEnum.Premium;
                default:
					return TypeStudentEnum.None;
			}
		}
	}
}
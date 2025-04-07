using Integration.Domain.Entities;
using Integration.Domain.Enum;
using Integration.Domain.Interfaces;

namespace Integration.Domain.Factories
{
    public class StudentFactory: IStudentFactory
	{
        public IStudent Builder(IStudent student)
        {            
            switch (student.StudentType)
            {
                case TypeStudentEnum.Basic:
                    return (IStudent)new BasicStudent(student.Name, student.Email, student.UserId)
                                .SetId<BasicStudent>(student.Id)
                                .SetBirthday(student.Birthday)
                                .SetCellphone(student.Cellphone)
                                .SetCountry(student.Country);                    
                case TypeStudentEnum.Partner:
                    return (IStudent) new PartnerStudent(student.CourseId, student.Name, student.Email, student.UserId)
                                .SetId<PartnerStudent>(student.Id)
                                .SetBirthday(student.Birthday)
                                .SetCellphone(student.Cellphone)
                                .SetCountry(student.Country);
                case TypeStudentEnum.Premium:
                    return (IStudent) new PremiumStudent(student.SecurityKey, student.Name, student.Email, student.UserId)
                                .SetId<PremiumStudent>(student.Id)
                                .SetBirthday(student.Birthday)
                                .SetCellphone(student.Cellphone)
                                .SetCountry(student.Country);
                default:
                    throw new NotImplementedException("StudentType not implemented");
            }
        }
    }
}


using Integration.Domain.Entities;
using Integration.Domain.Enum;
using Integration.Domain.Interfaces;

namespace Integration.Domain.Factories
{
    public class StudentFactory: IStudentFactory
	{
        public Student Builder(IStudent student)
        {            
            switch (student.TypeStudent)
            {
                case TypeStudentEnum.Basic:
                    return new BasicStudent(student.Name, student.Email, student.UserId)
                                .SetId<BasicStudent>(student.Id)
                                .SetBirthday(student.Birthday)
                                .SetCellphone(student.Cellphone)
                                .SetCountry(student.Country)
                                .SetDocument(student.Document);
                case TypeStudentEnum.Partner:
                    return  new PartnerStudent(student.CourseId.Value, student.Name, student.Email, student.UserId)
                                .SetId<PartnerStudent>(student.Id)
                                .SetBirthday(student.Birthday)
                                .SetCellphone(student.Cellphone)
                                .SetCountry(student.Country)
                                .SetDocument(student.Document);
                case TypeStudentEnum.Premium:
                    return  new PremiumStudent(student.SecurityKey.Value, student.Name, student.Email, student.UserId)
                                .SetId<PremiumStudent>(student.Id)
                                .SetBirthday(student.Birthday)
                                .SetCellphone(student.Cellphone)
                                .SetCountry(student.Country)
                                .SetDocument(student.Document);
                default:
                    throw new NotImplementedException("StudentType not implemented");
            }
        }
    }
}


using AutoMapper;
using Integration.API.Model.Request;
using Integration.API.Model.ViewModel;
using Integration.Domain.Enum;
using Integration.Domain.Models;

namespace Integration.API.Setup
{
    public class AutoMapperConfig : Profile
	{
        public AutoMapperConfig()
        {

            CreateMap<StudentModel, StudentViewModel>().ReverseMap()
                    .ForMember(dest => dest.TypeStudent,
                        opt => opt.MapFrom(source => (TypeStudentEnum)source.TypeStudent))
                    .ForMember(dest => dest.Active,
                        opt => opt.MapFrom(source => (StatusEntityEnum)source.Active));

          //  CreateMap<StudentViewModel, StudentRequest>().ReverseMap();

            //CreateMap<TeacherRequest, TeacherResponse>()
            //    .ForMember(dest => dest.StatusEntity,
            //    e => e.MapFrom(source =>
            //        source.StatusEntity == StatusEntityEnum.Active ? true : false));
        }
    }
}


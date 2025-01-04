using AutoMapper;
using GradeCalculator.Models;
using GradeCalculator.ViewModels;

namespace GradeCalculator.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        { 
            CreateMap<Predmet, PredmetVM>().ReverseMap();
            CreateMap<Godina, GodinaVM>().ReverseMap();
        }
    }
}

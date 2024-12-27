using AutoMapper;
namespace MvcLaptop.Models.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Laptop, LaptopViewModel>().ReverseMap();
        CreateMap<LaptopRequest, Laptop>();
    }
}
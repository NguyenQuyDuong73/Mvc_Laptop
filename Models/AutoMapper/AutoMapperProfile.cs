using AutoMapper;
namespace MvcLaptop.Models.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Product, LaptopViewModel>().ReverseMap();
        CreateMap<LaptopRequest, Product>();
    }
}
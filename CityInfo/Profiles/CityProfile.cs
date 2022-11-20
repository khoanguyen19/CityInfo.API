using AutoMapper;

namespace CityInfo.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City, Models.CityWithoutPointOfInterest>();
            CreateMap<Entities.City, Models.CityDto>();
        }
    }
}

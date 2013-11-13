using Aaron.Admin.Models.Directory;
using Aaron.Admin.Models.Localization;
using Aaron.Core.Domain.Directory;
using Aaron.Core.Domain.Localization;
using Aaron.Core.Infrastructure;
using AutoMapper;

namespace Aaron.Admin.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            //language
            Mapper.CreateMap<Language, LanguageModel>()
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<LanguageModel, Language>()
                .ForMember(dest => dest.LocaleStringResources, mo => mo.Ignore());

            //countries
            Mapper.CreateMap<CountryModel, Country>()
                .ForMember(dest => dest.StateProvinces, mo => mo.Ignore());
            Mapper.CreateMap<Country, CountryModel>()
                .ForMember(dest => dest.NumberOfStates, mo => mo.MapFrom(src => src.StateProvinces != null ? src.StateProvinces.Count : 0))
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //state/provinces
            Mapper.CreateMap<StateProvince, StateProvinceModel>()
                .ForMember(dest => dest.DisplayOrder1, mo => mo.MapFrom(src => src.DisplayOrder))
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<StateProvinceModel, StateProvince>()
                .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.DisplayOrder1))
                .ForMember(dest => dest.Country, mo => mo.Ignore());
        }

        public int Order
        {
            get { return 0; }
        }
    }
}

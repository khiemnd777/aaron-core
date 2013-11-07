using Aaron.Admin.Models.Localization;
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
        }

        public int Order
        {
            get { return 0; }
        }
    }
}

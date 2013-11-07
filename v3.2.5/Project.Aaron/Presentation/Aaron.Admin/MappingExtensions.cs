using Aaron.Admin.Models.Localization;
using Aaron.Core.Domain.Localization;
using AutoMapper;

namespace Aaron.Admin
{
    public static class MappingExtensions
    {
        #region Languages
        public static LanguageModel ToModel(this Language entity)
        {
            return Mapper.Map<Language, LanguageModel>(entity);
        }
        public static Language ToEntity(this LanguageModel model)
        {
            return Mapper.Map<LanguageModel, Language>(model);
        }
        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion
    }
}
using Aaron.Admin.Models.Directory;
using Aaron.Admin.Models.Localization;
using Aaron.Core.Domain.Directory;
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

        #region Countries / states

        public static CountryModel ToModel(this Country entity)
        {
            return Mapper.Map<Country, CountryModel>(entity);
        }

        public static Country ToEntity(this CountryModel model)
        {
            return Mapper.Map<CountryModel, Country>(model);
        }

        public static Country ToEntity(this CountryModel model, Country destination)
        {
            return Mapper.Map(model, destination);
        }

        public static StateProvinceModel ToModel(this StateProvince entity)
        {
            return Mapper.Map<StateProvince, StateProvinceModel>(entity);
        }

        public static StateProvince ToEntity(this StateProvinceModel model)
        {
            return Mapper.Map<StateProvinceModel, StateProvince>(model);
        }

        public static StateProvince ToEntity(this StateProvinceModel model, StateProvince destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion
    }
}
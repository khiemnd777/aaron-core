using FluentValidation;
using Aaron.Admin.Models.Directory;
using Aaron.Core.Services.Localization;

namespace Aaron.Admin.Validators.Directory
{
    public class StateProvinceValidator : AbstractValidator<StateProvinceModel>
    {
        public StateProvinceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Countries.States.Fields.Name.Required"));
        }
    }
}
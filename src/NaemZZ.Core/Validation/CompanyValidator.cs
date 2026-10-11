using FluentValidation;
using NaemZZ.Core.Dto;
using NaemZZ.Core.Validation;
using Tjt.Validators;

namespace NaemZZ.Core.Validators;

public sealed class CompanyValidator : BaseValidator<FirmDto>
{
    public CompanyValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Името на фирмата е задължително поле.");
    }
}

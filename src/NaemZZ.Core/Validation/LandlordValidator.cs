using FluentValidation;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Validation;
using Tjt.Validators;

namespace NaemZZ.Core.Validators;

// Reuses your existing Bulgarian rules so EGN and identity checks are not weakened.
public sealed class LandlordValidator : BaseValidator<NAEMODATELI>
{
    public LandlordValidator(IValidator<IMOTI> propertyValidator)
    {
        RuleFor(x => x).Custom((landlord, context) =>
        {
            foreach (var (field, message) in BulgarianValidation.ValidateLandlord(landlord))
                context.AddFailure(field, message);
        });
        RuleForEach(x => x.IMOTIs).SetValidator(propertyValidator);
    }
}

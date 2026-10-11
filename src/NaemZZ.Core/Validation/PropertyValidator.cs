using FluentValidation;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Validation;
using Tjt.Validators;

namespace NaemZZ.Core.Validators;

public sealed class PropertyValidator : BaseValidator<IMOTI>
{
    public PropertyValidator()
    {
        RuleFor(x => x).Custom((property, context) =>
        {
            foreach (var (field, message) in BulgarianValidation.ValidateProperty(property))
                context.AddFailure(field, message);
        });
    }
}

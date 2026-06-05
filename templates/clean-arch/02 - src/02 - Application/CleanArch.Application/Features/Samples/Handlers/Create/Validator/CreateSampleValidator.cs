using CleanArch.Application.Features.Samples.Handlers.Create.Request;
using CleanArch.Domain.Constants;
using CleanArch.Domain.MessageResource;
using FluentValidation;

namespace CleanArch.Application.Features.Samples.Handlers.Create.Validator;

public sealed class CreateSampleValidator : AbstractValidator<CreateSampleRequest>
{
    public CreateSampleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage(ValidationMessageResource.NAME_REQUIRED)
            .MaximumLength(ValidationConstants.SampleRules.NameMaxLength)
                .WithMessage(ValidationMessageResource.NAME_MAX_LENGTH);

        RuleFor(x => x.Description)
            .NotEmpty()
                .WithMessage(ValidationMessageResource.DESCRIPTION_REQUIRED)
            .MaximumLength(ValidationConstants.SampleRules.DescriptionMaxLength)
                .WithMessage(ValidationMessageResource.DESCRIPTION_MAX_LENGTH);
    }
}

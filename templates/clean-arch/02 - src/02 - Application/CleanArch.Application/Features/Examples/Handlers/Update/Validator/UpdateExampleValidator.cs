using CleanArch.Application.Features.Examples.Handlers.Update.Request;
using CleanArch.Domain.Constants;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Domain.MessageResource;
using FluentValidation;

namespace CleanArch.Application.Features.Examples.Handlers.Update.Validator;

public sealed class UpdateExampleValidator : AbstractValidator<UpdateExampleCommand>
{
    public UpdateExampleValidator(IGetByIdRepository<ExampleEntity> repository)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .MustAsync(async (id, ct) => await repository.GetByIdAsync(id, ct) is not null)
            .WithErrorCode(ExampleErrorCodes.NotFound)
            .WithMessage(ValidationMessageResource.SAMPLE_NOT_FOUND);

        RuleFor(x => x.Dto.Name)
            .NotEmpty()
                .WithMessage(ValidationMessageResource.NAME_REQUIRED)
            .MaximumLength(ValidationConstants.ExampleRules.NameMaxLength)
                .WithMessage(ValidationMessageResource.NAME_MAX_LENGTH)
            .OverridePropertyName(nameof(UpdateExampleRequest.Name));

        RuleFor(x => x.Dto.Description)
            .NotEmpty()
                .WithMessage(ValidationMessageResource.DESCRIPTION_REQUIRED)
            .MaximumLength(ValidationConstants.ExampleRules.DescriptionMaxLength)
                .WithMessage(ValidationMessageResource.DESCRIPTION_MAX_LENGTH)
            .OverridePropertyName(nameof(UpdateExampleRequest.Description));
    }
}

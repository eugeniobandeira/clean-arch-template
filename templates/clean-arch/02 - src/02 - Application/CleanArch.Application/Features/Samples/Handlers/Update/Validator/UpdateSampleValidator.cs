using CleanArch.Application.Features.Samples.Handlers.Update.Request;
using CleanArch.Domain.Constants;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Domain.MessageResource;
using FluentValidation;

namespace CleanArch.Application.Features.Samples.Handlers.Update.Validator;

public sealed class UpdateSampleValidator : AbstractValidator<UpdateSampleCommand>
{
    public UpdateSampleValidator(IGetByIdRepository<SampleEntity> repository)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .MustAsync(async (id, ct) => await repository.GetByIdAsync(id, ct) is not null)
            .WithErrorCode(SampleErrorCodes.NotFound)
            .WithMessage(ValidationMessageResource.SAMPLE_NOT_FOUND);

        RuleFor(x => x.Dto.Name)
            .NotEmpty()
                .WithMessage(ValidationMessageResource.NAME_REQUIRED)
            .MaximumLength(ValidationConstants.SampleRules.NameMaxLength)
                .WithMessage(ValidationMessageResource.NAME_MAX_LENGTH)
            .OverridePropertyName(nameof(UpdateSampleRequest.Name));

        RuleFor(x => x.Dto.Description)
            .NotEmpty()
                .WithMessage(ValidationMessageResource.DESCRIPTION_REQUIRED)
            .MaximumLength(ValidationConstants.SampleRules.DescriptionMaxLength)
                .WithMessage(ValidationMessageResource.DESCRIPTION_MAX_LENGTH)
            .OverridePropertyName(nameof(UpdateSampleRequest.Description));
    }
}

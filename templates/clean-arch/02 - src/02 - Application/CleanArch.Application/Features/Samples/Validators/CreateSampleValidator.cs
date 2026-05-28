using CleanArch.Application.Features.Samples.DTOs;
using FluentValidation;

namespace CleanArch.Application.Features.Samples.Validators;

public sealed class CreateSampleValidator : AbstractValidator<CreateSampleRequest>
{
    public CreateSampleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}

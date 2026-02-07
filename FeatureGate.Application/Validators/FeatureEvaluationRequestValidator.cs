using FeatureGate.Application.DTOs.Evaluations;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Application.Validators
{
    [ExcludeFromCodeCoverage]
    public class FeatureEvaluationRequestValidator
     : AbstractValidator<FeatureEvaluationRequestDto>
    {
        public FeatureEvaluationRequestValidator()
        {
            RuleFor(x => x.FeatureKey)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x)
                .Must(x =>
                    !string.IsNullOrEmpty(x.UserId) ||
                    !string.IsNullOrEmpty(x.GroupId) ||
                    !string.IsNullOrEmpty(x.Region))
                .WithMessage("At least one context (User, Group, Region) must be provided.");
        }
    }
}

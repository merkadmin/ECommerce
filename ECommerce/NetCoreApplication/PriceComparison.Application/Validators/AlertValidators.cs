using FluentValidation;
using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Validators;

public class CreateAlertValidator : AbstractValidator<CreateAlertDto>
{
    public CreateAlertValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product is required");

        RuleFor(x => x.TargetPrice)
            .GreaterThan(0).WithMessage("Target price must be greater than 0");
    }
}

public class UpdateAlertValidator : AbstractValidator<UpdateAlertDto>
{
    public UpdateAlertValidator()
    {
        RuleFor(x => x.TargetPrice)
            .GreaterThan(0).WithMessage("Target price must be greater than 0")
            .When(x => x.TargetPrice.HasValue);
    }
}

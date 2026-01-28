using FluentValidation;
using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Validators;

public class AddToCartValidator : AbstractValidator<AddToCartDto>
{
    public AddToCartValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100");
    }
}

public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemDto>
{
    public UpdateCartItemValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative")
            .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100");
    }
}

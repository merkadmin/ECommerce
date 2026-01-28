using FluentValidation;
using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Validators;

public class CreatePriceValidator : AbstractValidator<CreatePriceDto>
{
    public CreatePriceValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product is required");

        RuleFor(x => x.RetailerId)
            .NotEmpty().WithMessage("Retailer is required");

        RuleFor(x => x.CurrentPrice)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.OriginalPrice)
            .GreaterThanOrEqualTo(x => x.CurrentPrice)
            .When(x => x.OriginalPrice.HasValue)
            .WithMessage("Original price must be greater than or equal to current price");

        RuleFor(x => x.ProductUrl)
            .NotEmpty().WithMessage("Product URL is required")
            .MaximumLength(1000).WithMessage("URL cannot exceed 1000 characters");

        RuleFor(x => x.ShippingCost)
            .GreaterThanOrEqualTo(0).WithMessage("Shipping cost cannot be negative")
            .When(x => x.ShippingCost.HasValue);
    }
}

public class UpdatePriceValidator : AbstractValidator<UpdatePriceDto>
{
    public UpdatePriceValidator()
    {
        RuleFor(x => x.CurrentPrice)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.OriginalPrice)
            .GreaterThanOrEqualTo(x => x.CurrentPrice)
            .When(x => x.OriginalPrice.HasValue)
            .WithMessage("Original price must be greater than or equal to current price");

        RuleFor(x => x.ShippingCost)
            .GreaterThanOrEqualTo(0).WithMessage("Shipping cost cannot be negative")
            .When(x => x.ShippingCost.HasValue);
    }
}

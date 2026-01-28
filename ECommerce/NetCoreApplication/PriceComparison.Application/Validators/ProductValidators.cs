using FluentValidation;
using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Brand)
            .MaximumLength(100).WithMessage("Brand cannot exceed 100 characters");

        RuleFor(x => x.SKU)
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");
    }
}

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters")
            .When(x => x.Name != null);

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters")
            .When(x => x.Description != null);

        RuleFor(x => x.Brand)
            .MaximumLength(100).WithMessage("Brand cannot exceed 100 characters")
            .When(x => x.Brand != null);

        RuleFor(x => x.SKU)
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters")
            .When(x => x.SKU != null);

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters")
            .When(x => x.ImageUrl != null);
    }
}

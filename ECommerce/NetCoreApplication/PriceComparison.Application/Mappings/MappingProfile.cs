using AutoMapper;
using PriceComparison.Application.DTOs;
using PriceComparison.Core.Entities;

namespace PriceComparison.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
            .ForMember(d => d.LowestPrice, opt => opt.MapFrom(s => s.Prices.Where(p => p.IsAvailable).Min(p => (decimal?)p.CurrentPrice)))
            .ForMember(d => d.HighestPrice, opt => opt.MapFrom(s => s.Prices.Where(p => p.IsAvailable).Max(p => (decimal?)p.CurrentPrice)))
            .ForMember(d => d.RetailerCount, opt => opt.MapFrom(s => s.Prices.Count(p => p.IsAvailable)))
            .ForMember(d => d.MaxDiscount, opt => opt.MapFrom(s => s.Prices.Where(p => p.IsAvailable && p.DiscountPercent.HasValue).Max(p => p.DiscountPercent)));

        CreateMap<Product, ProductDetailDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
            .ForMember(d => d.LowestPrice, opt => opt.MapFrom(s => s.Prices.Where(p => p.IsAvailable).Min(p => (decimal?)p.CurrentPrice)))
            .ForMember(d => d.HighestPrice, opt => opt.MapFrom(s => s.Prices.Where(p => p.IsAvailable).Max(p => (decimal?)p.CurrentPrice)))
            .ForMember(d => d.RetailerCount, opt => opt.MapFrom(s => s.Prices.Count(p => p.IsAvailable)))
            .ForMember(d => d.MaxDiscount, opt => opt.MapFrom(s => s.Prices.Where(p => p.IsAvailable && p.DiscountPercent.HasValue).Max(p => p.DiscountPercent)))
            .ForMember(d => d.Prices, opt => opt.Ignore())
            .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category));

        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Price mappings
        CreateMap<Price, PriceDto>()
            .ForMember(d => d.RetailerName, opt => opt.MapFrom(s => s.Retailer != null ? s.Retailer.Name : string.Empty));

        CreateMap<Price, RetailerPriceDto>()
            .ForMember(d => d.RetailerName, opt => opt.MapFrom(s => s.Retailer != null ? s.Retailer.Name : string.Empty))
            .ForMember(d => d.RetailerLogo, opt => opt.MapFrom(s => s.Retailer != null ? s.Retailer.LogoUrl : string.Empty))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.CurrentPrice));

        CreateMap<CreatePriceDto, Price>();

        // Category mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.ParentCategoryName, opt => opt.MapFrom(s => s.ParentCategory != null ? s.ParentCategory.Name : null))
            .ForMember(d => d.ProductCount, opt => opt.MapFrom(s => s.Products != null ? s.Products.Count : 0))
            .ForMember(d => d.SubCategories, opt => opt.MapFrom(s => s.SubCategories));

        CreateMap<CreateCategoryDto, Category>();

        // Retailer mappings
        CreateMap<Retailer, RetailerDto>()
            .ForMember(d => d.ProductCount, opt => opt.MapFrom(s => s.Prices != null ? s.Prices.Select(p => p.ProductId).Distinct().Count() : 0));

        CreateMap<CreateRetailerDto, Retailer>();

        // PriceAlert mappings
        CreateMap<PriceAlert, PriceAlertDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.ProductImage, opt => opt.MapFrom(s => s.Product != null ? s.Product.ImageUrl : string.Empty))
            .ForMember(d => d.CurrentLowestPrice, opt => opt.Ignore());

        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();

        // PriceHistory mappings
        CreateMap<PriceHistory, PriceHistoryDto>()
            .ForMember(d => d.RetailerName, opt => opt.MapFrom(s => s.Retailer != null ? s.Retailer.Name : string.Empty));

        // Wishlist mappings
        CreateMap<WishlistItem, WishlistItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.ProductImage, opt => opt.MapFrom(s => s.Product != null ? s.Product.ImageUrl : string.Empty))
            .ForMember(d => d.AddedAt, opt => opt.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.LowestPrice, opt => opt.Ignore());

        // Cart mappings
        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.ProductImage, opt => opt.MapFrom(s => s.Product != null ? s.Product.ImageUrl : string.Empty))
            .ForMember(d => d.RetailerName, opt => opt.MapFrom(s => s.Retailer != null ? s.Retailer.Name : null))
            .ForMember(d => d.Price, opt => opt.Ignore())
            .ForMember(d => d.ShippingCost, opt => opt.Ignore());
    }
}

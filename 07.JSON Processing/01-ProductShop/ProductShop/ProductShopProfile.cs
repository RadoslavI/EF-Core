using AutoMapper;
using ProductShop.DTOs.Category;
using ProductShop.DTOs.Product;
using ProductShop.DTOs.Users;
using ProductShop.DTOs.Category_Product;
using ProductShop.Models;
using ProductShop.DTOs.User;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<ImportUserDTO, User>();
            this.CreateMap<ImportProductDTO, Product>();
            this.CreateMap<ImportCategoryDTO, Category>();
            this.CreateMap<ImportCategoryProductDTO, CategoryProduct>();

            this.CreateMap<Product, ExportProductsInRangeDTO>()
                .ForMember(d => d.Seller,
                mo => mo.MapFrom(s => $"{s.Seller.FirstName} {s.Seller.LastName}"));

            //Inner DTO
            this.CreateMap<Product, ExportUserSoldProductsDTO>()
                .ForMember(d => d.BuyerFirstName,
                mo => mo.MapFrom(s => s.Buyer.FirstName))
                .ForMember(d => d.BuyerLastName,
                mo => mo.MapFrom(s => s.Buyer.LastName));

            //Outer DTO
            this.CreateMap<User, ExportUsersWithOneDTO>()
                .ForMember(d => d.SoldProducts, 
                mo => mo.MapFrom(s => s.ProductsSold
                    .Where(p => p.BuyerId.HasValue)));

            this.CreateMap<Product, ExportSoldProductShortInfoDto>();
            this.CreateMap<User, ExportProductsFullInfoDto>()
                .ForMember(d => d.SoldProducts, 
                mo => mo.MapFrom(s => s.ProductsSold
                .Where(p => p.BuyerId.HasValue)));
            this.CreateMap<User, ExportUsersWithFullProductInfoDto>()
                .ForMember(d => d.SoldProductsInfo,
                mo => mo.MapFrom(s => s));


        }
    }
}

using System;
using AutoMapper;
using PetStore.Models;
using PetStore.Models.Enumerations;
using PetStore.ViewModels.Product;
using PetStoreServiceModels.Products.ImputModels;
using PetStoreServiceModels.Products.OutputModels;

namespace PetStore.Mapping
{
    public class ProductProfile :Profile
    {
        public ProductProfile()
        {
            this.CreateMap<AddProductInputServiceModel, Product>()
                .ForMember(x=>x.ProductType
                ,y=>y.MapFrom
                    (x=>Enum.Parse(typeof(ProductType),x.ProductType)));

            this.CreateMap<Product, ListAllProductByProductTypeServiceModel>();

            this.CreateMap<Product, ListAllProductsServiceModel>()
                .ForMember(x=>x.ProductType,y=>y.MapFrom(x=>x.ProductType.ToString()));
           
            this.CreateMap<Product, ListAllProductsByNameServiceModel>()
                .ForMember(x => x.ProductType
                    , y => y.MapFrom(x => x.ProductType.ToString()));
           
            this.CreateMap<EditProductInputServiceModel,Product>()
                .ForMember(x => x.ProductType
                    , y => y.MapFrom
                        (x => Enum.Parse(typeof(ProductType), x.ProductType)));
           
            this.CreateMap<ListAllProductsServiceModel, ListAllProductViewModel>();
        }
    }
}

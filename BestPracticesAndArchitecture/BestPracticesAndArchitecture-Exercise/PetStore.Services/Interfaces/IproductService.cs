using System.Collections.Generic;
using PetStoreServiceModels.Products.ImputModels;
using PetStoreServiceModels.Products.OutputModels;

namespace PetStore.Services.Interfaces
{
    public interface IProductService
    {
        void AddProduct(AddProductInputServiceModel model);
        ICollection<ListAllProductByProductTypeServiceModel> ListAllProductType(string type);
        ICollection<ListAllProductsServiceModel> GetAll();

        ICollection<ListAllProductsByNameServiceModel>
            SearchByName(string searchStr, bool caseSensitive);
        bool RemoveById(string id);
        bool RemoveByName(string name);
        void EditProduct(string id, EditProductInputServiceModel model);
    }
}

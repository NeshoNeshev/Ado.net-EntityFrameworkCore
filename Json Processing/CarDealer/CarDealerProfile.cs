using System.Linq;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<Customer, CustomerTotalSalesDTO>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name))
                .ForMember(x => x.CarsBought, y => y.MapFrom(s => s.Sales.Count))
                .ForMember(x => x.SpentMoney, y => y.MapFrom(s => s.Sales
                    .Select(s => s.Car
                        .PartCars
                        .Select(pc => pc.Part)
                        .Sum(pc => pc.Price))
                    .Sum()));
            
        }
    }
}

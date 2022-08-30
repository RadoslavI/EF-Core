using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.DTO.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSuppliersDto, Supplier>();
            this.CreateMap<ImportPartsDto, Part>();
            this.CreateMap<ImportCustomerDto, Customer>();
            this.CreateMap<ImportSaleDto, Sale>();
        }
    }
}

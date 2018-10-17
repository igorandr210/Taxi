using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Entities;
using Taxi.Helpers;
using Taxi.Models.Admins;
using Taxi.Models.Customers;
using Taxi.Models.Drivers;
using Taxi.Models.Trips;

namespace Taxi.Models.MappingProfile
{
    public class ModelToEntityMappingProfile : Profile
    {
        public ModelToEntityMappingProfile()
        {
            CreateMap<CustomerRegistrationDto, AppUser>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));

            CreateMap<DriverRegistrationDto, AppUser>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));

            CreateMap<DriverRegistrationDto, Driver>();

            CreateMap<CustomerRegistrationDto, Customer>();

            CreateMap<Customer, CustomerDto>().ForMember(x => x.Id, map => map.MapFrom(vm => vm.Id));

            CreateMap<AppUser, CustomerDto>().ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<Driver, DriverDto>().ForMember(x => x.Id, map => map.MapFrom(vm => vm.Id));

            CreateMap<AppUser, DriverDto>().ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<CustomerRegistrationDto, CustomerDto>();

            CreateMap<DriverRegistrationDto, DriverDto>();

            CreateMap<CustomerUpdateDto, AppUser>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<DriverUpdateDto, AppUser>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); 

            CreateMap<CustomerUpdateDto, Customer>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); 

            CreateMap<DriverUpdateDto, Driver>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Customer, Driver>().ForMember(x => x.Id, opt => opt.Ignore()); 

            CreateMap<CustomerDriverUpgradeDto, Driver>();

            CreateMap<Driver, Customer>().ForMember(x => x.Id, opt => opt.Ignore());
            
            CreateMap<AddVehicleDto, Vehicle>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Vehicle, VehicleToReturnDto>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<Trip, TripHistory>().ForMember(x => x.Id, opt => opt.Ignore());
            
            CreateMap<LatLonDto, TripRouteNode>();

            CreateMap<LatLonDto, PlaceDto>();

            CreateMap<PlaceDto, TripRouteNode>();//unused

            CreateMap<TripRouteNode, RouteNodeDto>();

            CreateMap<Trip, TripStatusDto>().ForMember(x => x.From, opt => opt.Ignore()).ForMember(x => x.To, opt => opt.Ignore());

            CreateMap<DriverLicense, DriverLicenseDto>();

            CreateMap<TripRouteNode, TripHistoryRouteNode>().ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<RefundMessageDto, RefundRequest>();

            CreateMap<AdminRegistrationDto, AppUser>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));

            CreateMap<AdminRegistrationDto, Admin>();

            CreateMap<Admin, Customer>().ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<AdminRegistrationDto, AdminDto>();

            CreateMap<Admin, AdminDto>().ForMember(x => x.Id, map => map.MapFrom(vm => vm.Id));

            CreateMap<AppUser, AdminDto>().ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<AppUser, UserDto>();

            CreateMap<AdminResponseDto, AdminResponse>();

            CreateMap<RefundRequest, RefundRequestDto>();

            CreateMap<TripHistory, AdminTripHistoryDto>().ForMember(x => x.From, opt => opt.Ignore()).ForMember(x => x.To, opt => opt.Ignore());

            CreateMap<AdminResponse, AdminResponseToReturnDto>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Entities;
using Taxi.Helpers;
using Taxi.Models;
using Taxi.Models.Admins;
using Taxi.Models.Drivers;

namespace Taxi.Services
{
    public interface IUsersRepository
    {
        AdminResponse GetAdminResponse(Guid id);
        Task<bool> RemoveUser(AppUser user);
        Task<bool> RemoveFromAdmins(Admin admin);
        AppUser GetUser(string id);
        PagedList<RefundRequest> GetRefundRequests(RefundResourceParameters resourceParameters);
        bool UpdateRefund(RefundRequest request);
        RefundRequest GetRefundRequest(Guid id);
        PagedList<DriverLicense> GetDriverLicenses(DriverLicenseResourceParameters resourceParameters); 
        Task<bool> AddAdminResponse(AdminResponse response);
        Task<PagedList<AppUser>> GetUsers(UserResourceParameters resourceParameters);
        Admin GetAdminById(Guid adminId);

        PagedList<Admin> GetAdmins(PaginationParameters parameters);

        Task<bool> AddAdmin(Admin admin);

        Task<bool> ApproveAdmin(Admin admin);

        Task<bool> AddCustomer(Customer customer);

        Task<bool> AddDriver(Driver driver);

        Task<bool> UpdateCustomer(Customer customer);

        Task<bool> UpdateDriver(Driver driver);

        Customer GetCustomerByIdentityId(string identityId);

        Driver GetDriverByIdentityId(string identityId);

        Customer GetCustomerById(Guid Id);

        Driver GetDriverById(Guid Id);

        Customer GetCustomerByConnectionId(string connectionId);

        Driver GetDriverByConnectionId(string connectionId);

        IEnumerable<Driver> GetDrivers();

        IEnumerable<Customer> GetCustomers();

        RefreshToken GetRefreshToken(string token);

        Task<bool> DeleteRefleshToken(RefreshToken token);

        Task<bool> AddRefreshToken(RefreshToken token);

        IEnumerable<RefreshToken> GetTokensForUser(string userId);

        Task<bool> AddVehicleToDriver(Guid id, Vehicle vehicle);

        Task<Vehicle> GetVehicle(Guid vehicleId);

        Task<bool> RemoveVehicle(Vehicle vehicle);

        Task<bool> AddPictureToVehicle(Vehicle v,string id);

        Task<bool> RemoveProfilePicture(AppUser user);

        Task<bool> AddProfilePicture(AppUser user, ProfilePicture picture);

        Task<bool> RemoveVehicleImage(Driver driver, string imageId);

        Task<bool> RemoveDriverLicense(DriverLicense license);
        
        Task<bool> AddDriverLicense(DriverLicense driverLicense);

        Task<bool> UpdateDriverLicense(DriverLicense driverLicense);
        PagedList<AdminResponse> GetAdminResponses(string id, PaginationParameters resourceParameters);
    }
}

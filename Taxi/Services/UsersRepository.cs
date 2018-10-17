using Amazon.S3;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Taxi.Data;
using Taxi.Entities;
using Taxi.Helpers;
using Taxi.Models;
using Taxi.Models.Admins;
using Taxi.Models.Drivers;

namespace Taxi.Services
{
    public class UsersRepository : IUsersRepository
    {
        ApplicationDbContext _dataContext;
        private IMapper _mapper;
        private UserManager<AppUser> _userManager;
        private IUploadService _uploadService;

        public UsersRepository(ApplicationDbContext dataContext,
            IMapper mapper, 
            UserManager<AppUser> userManager,
            IUploadService uploadService)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _userManager = userManager;
            _uploadService = uploadService;
        }


        public AdminResponse GetAdminResponse(Guid id)
        {
            return _dataContext.AdminResponces.FirstOrDefault(r => r.Id == id);
        }

        public async Task<bool> RemoveUser(AppUser user)
        {
            var driver = _dataContext.Drivers.Include(d=> d.DriverLicense).Include(dr=>dr.Vehicle).ThenInclude(v=>v.Pictures).FirstOrDefault(d => d.IdentityId == user.Id);
            var customer = _dataContext.Customers.FirstOrDefault(d => d.IdentityId == user.Id);
            var admin = _dataContext.Admins.FirstOrDefault(d => d.IdentityId == user.Id);

            _dataContext.Entry(user).Reference(u => u.ProfilePicture).Load();
            _dataContext.Entry(user).Collection(u => u.RefreshTokens).Load();
            _dataContext.Entry(user).Collection(u => u.AdminResponces).Load();

            if (user.AdminResponces.Count > 0)
                _dataContext.RemoveRange(user.AdminResponces);

            if (user.RefreshTokens.Count > 0)
                _dataContext.RemoveRange(user.RefreshTokens);
            
            if (user.ProfilePicture != null)
                await RemoveProfilePicture(user);

            if (admin != null)
                _dataContext.Remove(admin);

            if (driver != null)
            {
                if (driver.DriverLicense!= null)
                    await RemoveDriverLicense(driver.DriverLicense);

                if (driver.Vehicle != null)
                {
                    await RemoveVehicle(driver.Vehicle);
                }

                _dataContext.Remove(driver);
            }
            
            if (customer != null)
                _dataContext.Remove(customer);
            try
            {
                await _dataContext.SaveChangesAsync();

                await _userManager.DeleteAsync(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public AppUser GetUser(string id)
        {
            return _dataContext.Users.Include(u => u.ProfilePicture).FirstOrDefault(ur => ur.Id == id);
        }

        public PagedList<RefundRequest> GetRefundRequests(RefundResourceParameters resourceParameters)
        {
            IQueryable<RefundRequest> beforePaging = _dataContext.RefundRequests.OrderBy(r => r.CreationTime);

            if (resourceParameters.IsSolved != null)
            {
                beforePaging = beforePaging.Where(p => p.Solved == resourceParameters.IsSolved);
            }
            return PagedList<RefundRequest>.Create(beforePaging, resourceParameters.PageNumber, resourceParameters.PageSize);
        }

        public bool UpdateRefund(RefundRequest request)
        {
            try
            {
                _dataContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public RefundRequest GetRefundRequest(Guid id)
        {
            return _dataContext.RefundRequests.FirstOrDefault(r => r.Id == id);
        }

        public PagedList<DriverLicense> GetDriverLicenses(DriverLicenseResourceParameters resourceParameters)
        {
            IQueryable<DriverLicense> beforePaging = _dataContext.DriverLicenses.OrderBy(l => l.UpdateTime);

            if (resourceParameters.IsApproved != null)
            {
                beforePaging = beforePaging.Where(p => p.IsApproved == resourceParameters.IsApproved);
            }

            return PagedList<DriverLicense>.Create(beforePaging, resourceParameters.PageNumber, resourceParameters.PageSize);

        }

        public async Task<bool> AddAdminResponse(AdminResponse response)
        {
            await _dataContext.AdminResponces.AddAsync(response);
            try
            {
                await _dataContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PagedList<AppUser>> GetUsers(UserResourceParameters paginationParameters)
        {
            IQueryable<AppUser> beforePaging = //(!string.IsNullOrEmpty(paginationParameters.Rol))?
              //  await _userManager.GetUsersForClaimAsync(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol, paginationParameters.Rol)) :
                _userManager.Users.OrderBy(u => u.Email);
            

            if (!string.IsNullOrEmpty(paginationParameters.SearchQuery))
            {
                var searchForWhereClause = paginationParameters.SearchQuery.Trim().ToLowerInvariant();
                 
                beforePaging = beforePaging.Where(a => (a.FirstName + " "+ a.LastName + " " + a.Email + " "+ a.PhoneNumber).ToLowerInvariant().Contains(searchForWhereClause));
            }
            
            if (paginationParameters.EmailConfirmed != null)
            {
                beforePaging = beforePaging.Where(u => u.EmailConfirmed == paginationParameters.EmailConfirmed );
            }

            if (!string.IsNullOrEmpty(paginationParameters.Rol))
            {
                beforePaging = beforePaging.Where(u =>
                    _dataContext.UserClaims.FirstOrDefault(c =>
                        c.UserId == u.Id && c.ClaimValue == paginationParameters.Rol) != null);
            }

            return PagedList<AppUser>.Create(beforePaging.Include(u => u.ProfilePicture), paginationParameters.PageNumber, paginationParameters.PageSize);
        }

        public Admin GetAdminById(Guid adminId)
        {
            return _dataContext.Admins.Include(a => a.Identity).ThenInclude(a => a.ProfilePicture).FirstOrDefault(ad => ad.Id == adminId);
        }

        public async Task<bool> RemoveFromAdmins(Admin admin)
        {
            try
            {
                var claims = (await _userManager.GetClaimsAsync(admin.Identity)).Where(c => c.Value == Constants.Strings.JwtClaims.AdminAccess || 
                                                                                            c.Type == Constants.Strings.JwtClaimIdentifiers.AdminId);

                var res = await _userManager.RemoveClaimsAsync(admin.Identity, claims);

                if (res.Succeeded != true)
                    return false;

                _dataContext.Remove(admin);

                await _dataContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public PagedList<Admin> GetAdmins(PaginationParameters paginationParameters)
        {
            var beforePaging = _dataContext.Admins.OrderBy(a => a.IsApproved).Include(a => a.Identity).ThenInclude(i => i.ProfilePicture);
            return PagedList<Admin>.Create(beforePaging, paginationParameters.PageNumber, paginationParameters.PageSize);
        }

        public async Task<bool> AddAdmin(Admin admin)
        {
            await _dataContext.Admins.AddAsync(admin);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            var claims = new List<Claim> {
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.AdminId, admin.Id.ToString())
            };
            var identity = await _userManager.FindByIdAsync(admin.IdentityId);

            var addClaimRes = await _userManager.AddClaimsAsync(identity, claims);

            if (admin.IsApproved == true)
                await ApproveAdmin(admin);

            return true;
        }

        public async Task<bool> ApproveAdmin(Admin admin)
        {
            admin.IsApproved = true;

            var claims = new List<Claim> {
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol, Helpers.Constants.Strings.JwtClaims.AdminAccess),
            };
            var identity = await _userManager.FindByIdAsync(admin.IdentityId);
            try
            {
                await _dataContext.SaveChangesAsync();
                var addClaimRes = await _userManager.AddClaimsAsync(identity, claims);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> AddCustomer(Customer customer)
        {
            await _dataContext.Customers.AddAsync(customer);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            var claims = new List<Claim> {
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol, Helpers.Constants.Strings.JwtClaims.CustomerAccess),
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.CustomerId, customer.Id.ToString())
            };

            var addClaimRes = await _userManager.AddClaimsAsync(customer.Identity, claims);
            return true;
        }
        
        public async Task<bool> UpdateCustomer(Customer customer)
        {
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateDriver(Driver Driver)
        {
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> AddDriver(Driver driver)
        {
            await _dataContext.Drivers.AddAsync(driver);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            var claims = new List<Claim> {
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol, Helpers.Constants.Strings.JwtClaims.DriverAccess),
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.DriverId, driver.Id.ToString())
            };
            
            var addClaimRes = await _userManager.AddClaimsAsync(driver.Identity, claims);
            return true;
        }

        public Driver GetDriverByIdentityId(string identityId)
        {
            var driver = _dataContext.Drivers.Where(o => o.IdentityId == identityId).Include(d => d.Identity).FirstOrDefault();

            return driver;
        }

        public Customer GetCustomerByIdentityId(string identityId)
        {
            var customer = _dataContext.Customers.Where(o => o.IdentityId == identityId).Include(d => d.Identity).FirstOrDefault();

            return customer;
        }
        
        public Customer GetCustomerById(Guid id)
        {
            var customer = _dataContext.Customers.Include(d => d.Identity).Include(c=>c.CurrentTrip).FirstOrDefault(o => o.Id == id);

            return customer;
        }

        public Customer GetCustomerByConnectionId(string connectionId)
        {
            var customer = _dataContext.Customers.SingleOrDefault(o => o.ConnectionId == connectionId);

            return customer;
        }

        public Driver GetDriverByConnectionId(string connectionId)
        {
            var driver = _dataContext.Drivers.SingleOrDefault(o => o.ConnectionId == connectionId);

            return driver;
        }

        public Driver GetDriverById(Guid id)
        {
            var driver = _dataContext.Drivers.Include(d => d.Identity)
                .Include(dv => dv.DriverLicense)
                .Include(dr => dr.Vehicle)
                .ThenInclude(v=> v.Pictures)
                .SingleOrDefault(o => o.Id == id);

            return driver;
        }

        public IEnumerable<Driver> GetDrivers()
        {
            return _dataContext.Drivers.ToList();
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _dataContext.Customers.ToList();
        }

        public RefreshToken GetRefreshToken(string token)
        {
            return _dataContext.RefreshTokens.FirstOrDefault(t => t.Token == token);
        }

        public async Task<bool> DeleteRefleshToken(RefreshToken token)
        {
            try
            {
                if (await _dataContext.RefreshTokens.AnyAsync(t => t.Token == token.Token))
                {
                    _dataContext.RefreshTokens.Remove(token);
                    await _dataContext.SaveChangesAsync();
                }

            }
            catch (Exception e)
            {
                throw;
                //return false;
            }
            return true;
        }
        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            try
            {
                await _dataContext.RefreshTokens.AddAsync(token);
                await _dataContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<RefreshToken> GetTokensForUser(string userId)
        {
            return _dataContext.RefreshTokens.Where(t => t.IdentityId == userId).ToList();
        }

        public async Task<bool> AddVehicleToDriver(Guid DriverId, Vehicle vehicle)
        {
            try
            {
                var driver = GetDriverById(DriverId);
                if (driver == null)
                    return false;
                if (driver.Vehicle != null)
                    await RemoveVehicle(driver.Vehicle);
                driver.Vehicle = vehicle;
                await _dataContext.SaveChangesAsync();
            } catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveVehicle(Vehicle vehicle)
        {
            foreach (var p in vehicle.Pictures)
            {
                await _uploadService.DeleteObjectAsync(p.Id);
            }
            _dataContext.Remove(vehicle);
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }
        
        public async Task<Vehicle> GetVehicle(Guid vehicleId)
        {
            return await _dataContext.Vehicles.Include(o => o.Pictures).FirstOrDefaultAsync(v => v.Id == vehicleId);
        }

        public async Task<bool> AddPictureToVehicle(Vehicle v, string id)
        {
            v.Pictures.Add(new Picture() { Id = id });
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }  

        public async Task<bool> RemoveProfilePicture(AppUser user)
        {
            await _uploadService.DeleteObjectAsync(user.ProfilePicture.Id);

            _dataContext.ProfilePictures.Remove(user.ProfilePicture);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> AddProfilePicture(AppUser user, ProfilePicture picture)
        {
            user.ProfilePicture = picture;

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveVehicleImage(Driver driver, string imageId)
        {
            var res = driver.Vehicle.Pictures.RemoveAll(p => p.Id == imageId);

            if (res == 0)
                return false;

            await _uploadService.DeleteObjectAsync(imageId);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> AddDriverLicense(DriverLicense driverLicense)
        {
            await _dataContext.DriverLicenses.AddAsync(driverLicense);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateDriverLicense(DriverLicense driverLicense)
        {
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public PagedList<AdminResponse> GetAdminResponses(string id, PaginationParameters resourceParameters)
        {
            var beforePaging = _dataContext.AdminResponces.OrderByDescending(ar => ar.CreationTime).Where(a => a.IdentityId == id);

            return PagedList<AdminResponse>.Create(beforePaging, resourceParameters.PageNumber, resourceParameters.PageSize);
        }

        public async Task<bool> RemoveDriverLicense(DriverLicense license)
        {
            _dataContext.DriverLicenses.Remove(license);

            await _uploadService.DeleteObjectAsync(license.ImageId);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}

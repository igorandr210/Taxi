using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Helpers
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Rol = "rol", Id = "id", CustomerId = "customerId", DriverId = "driverId",AdminId = "adminId";
            }

            public static class JwtClaims
            {
                public const string DriverAccess = "driver_access";

                public const string CustomerAccess = "customer_access";

                public const string RootUserAccess = "root_access";

                public const string AdminAccess = "admin_access";
            }
        }
    }
}

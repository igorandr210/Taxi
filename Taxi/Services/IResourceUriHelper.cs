using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Helpers;
using Taxi.Models;

namespace Taxi.Services
{
    public interface IResourceUriHelper
    {
        string CreateResourceUri(PaginationParameters resourceParameters, ResourceUriType type, string getMethodName);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Taxi.Helpers;
using Taxi.Models;

namespace Taxi.Services
{
    public class ResourceUriHelper:IResourceUriHelper
    {
        private IUrlHelper _urlHelper;

        public ResourceUriHelper(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public string CreateResourceUri(PaginationParameters resourceParameters, ResourceUriType type, string getMethodName)
        {
            switch (type)
            {
                case ResourceUriType.PrevoiusPage:
                    return _urlHelper.Link(getMethodName,
                        new
                        {
                            pageNumber = resourceParameters.PageNumber - 1,
                            pageSize = resourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link(getMethodName,
                        new
                        {
                            pageNumber = resourceParameters.PageNumber + 1,
                            pageSize = resourceParameters.PageSize
                        });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link(getMethodName,
                        new
                        {
                            pageNumber = resourceParameters.PageNumber,
                            pageSize = resourceParameters.PageSize
                        });
            }
        }
    }
}

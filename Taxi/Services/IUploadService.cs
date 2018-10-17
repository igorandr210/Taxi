using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Models.Drivers;

namespace Taxi.Services
{
    public interface IUploadService
    {
        Task PutObjectToStorage(string key, string data);

        Task<FileDto> GetObjectAsync(string key);

        Task<bool> DeleteObjectAsync(string keyName);
    }
}

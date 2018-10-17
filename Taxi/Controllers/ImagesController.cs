using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Models.Drivers;
using Taxi.Services;

namespace Taxi.Controllers
{
    [Route("api/images")]
    public class ImagesController: Controller
    {
        private IUploadService _uploadService;

        public ImagesController(IUploadService  uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            FileDto res = await _uploadService.GetObjectAsync(id);

            if (res == null)
                return NotFound();

            res.Stream.Seek(0, SeekOrigin.Begin);
            return File(res.Stream, res.ContentType);
        }
         
    }
}

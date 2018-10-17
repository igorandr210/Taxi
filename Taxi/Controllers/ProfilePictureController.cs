using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Taxi.Entities;
using Taxi.Helpers;
using Taxi.Models;
using Taxi.Models.Drivers;
using Taxi.Services;

namespace Taxi.Controllers
{
    [Route("api/profilepicture")]
    public class ProfilePictureController: Controller
    {
        private UserManager<AppUser> _userManager;
        private IUploadService _uploadService;
        private IUsersRepository _usersRepository;
        private IHostingEnvironment _hostingEnvironment;

        public ProfilePictureController(UserManager<AppUser> userManager,
            IUploadService uploadService,
            IUsersRepository usersRepository,
            IHostingEnvironment env)
        {
            _userManager = userManager;
            _uploadService = uploadService;
            _usersRepository = usersRepository;
            _hostingEnvironment = env;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProfilePicture()
        {
            var uid = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.Id)?.Value;

            var user = await _userManager.Users.Include(o => o.ProfilePicture).FirstOrDefaultAsync(p => p.Id == uid);

            if (user?.ProfilePicture == null)
                return NotFound();

            FileDto res = await _uploadService.GetObjectAsync(user.ProfilePicture?.Id);

            if (res == null)
                return NotFound();

            res.Stream.Seek(0, SeekOrigin.Begin);
            return File(res.Stream, res.ContentType);
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SetProfilePicture(List<IFormFile> files)
        {
            var uid = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.Id)?.Value;

            var user = await _userManager.Users.Include(o => o.ProfilePicture).FirstOrDefaultAsync(p => p.Id == uid);

            long size = files.Sum(f => f.Length);

            var formFile = files[0];

            if (!formFile.IsImage())
            {
                return BadRequest();
            }
            
            if (user.ProfilePicture != null)
            {
                var res = await _usersRepository.RemoveProfilePicture(user);
                if (!res)
                    return Conflict();
                //remove picture from data context
            }
            if (formFile.Length > 0)
            {
                var filename = ContentDispositionHeaderValue
                        .Parse(formFile.ContentDisposition)
                        .FileName
                        .TrimStart().ToString();
                filename = _hostingEnvironment.WebRootPath + $@"\uploads" + $@"\{formFile.FileName}";
                size += formFile.Length;
                using (var fs = System.IO.File.Create(filename))
                {
                    await formFile.CopyToAsync(fs);
                    fs.Flush();
                }//these code snippets saves the uploaded files to the project directory
                var imageId = Guid.NewGuid().ToString() + Path.GetExtension(filename);
                await _uploadService.PutObjectToStorage(imageId.ToString(), filename);//this is the method to upload saved file to S3
                var res = await _usersRepository.AddProfilePicture(user, new ProfilePicture() { Id = imageId});
                if (!res)
                    return Conflict();
                System.IO.File.Delete(filename);
                return Ok(new ImageToReturnDto() { ImageId = imageId });
            }
            return BadRequest();
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(204)]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            var uid = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.Id)?.Value;

            var user = await _userManager.Users.Include(o => o.ProfilePicture).FirstOrDefaultAsync(p => p.Id == uid);

            if (user.ProfilePicture != null)
            {
                var res = await _usersRepository.RemoveProfilePicture(user);
                if (!res)
                    return Conflict();
                //remove picture from data context
            }
            else return NotFound();

            return NoContent();
        }
    }
}

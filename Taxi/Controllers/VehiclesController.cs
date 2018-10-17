using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Taxi.Entities;
using Taxi.Helpers;
using Taxi.Models;
using Taxi.Models.Drivers;
using Taxi.Services;

namespace Taxi.Controllers
{
    [Route("api/vehicles")]
    public class VehiclesController: Controller
    {
        private IMapper _mapper;
        private UserManager<AppUser> _userManager;
        private IUsersRepository _usersRepository;
        private IUploadService _uploadService;
        private IHostingEnvironment _hostingEnvironment;
   
        public VehiclesController(UserManager<AppUser> userManager, IMapper mapper, IUsersRepository usersRepository, 
            IUploadService uploadService, IHostingEnvironment env, IAmazonS3 amazonS3
            )
        {
            _mapper = mapper;
            _userManager = userManager;
            _usersRepository = usersRepository;
            _uploadService = uploadService;
            _hostingEnvironment = env;
        }
        //addVehicleToDriverById
        //[HttpPost("driver/{id}")]
        //[ProducesResponseType(201)]
        //public async  Task<IActionResult> AddVehicleToDriverById([FromRoute] Guid id ,[FromBody] AddVehicleDto vehicle)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var driver = _usersRepository.GetDriverById(id);

        //    if (driver == null)
        //        return NotFound();
            
        //    if (driver.Vehicle != null)
        //    {
        //        ModelState.AddModelError(nameof(driver.Vehicle), "Driver already has vehicle.");
        //        return BadRequest();
        //    }
        //    var vehicleEntity = _mapper.Map<Vehicle>(vehicle);

        //    var res = await _usersRepository.AddVehicleToDriver(id, vehicleEntity);

        //    if (res != true)
        //    {
        //        return BadRequest();
        //    }

        //    var vehicleToReturn = _mapper.Map<VehicleToReturnDto>(vehicleEntity);

        //    return CreatedAtRoute("GetVehicle", new { id = vehicleEntity.Id }, vehicleToReturn);
        //}

        [HttpPut()]
        [Authorize(Policy = "DriverReg")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddVehicleToDriver([FromBody]AddVehicleDto vehicle)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var driverId = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.DriverId)?.Value;

            var vehicleEntity = _mapper.Map<Vehicle>(vehicle);

            var res = await _usersRepository.AddVehicleToDriver(Guid.Parse(driverId), vehicleEntity);

            if (res != true)
            {
                return Conflict();
            }
            var vehicleToReturn = _mapper.Map<VehicleToReturnDto>(vehicleEntity);
            return CreatedAtRoute("GetVehicle", new { id = vehicleEntity.Id }, vehicleToReturn);
        }

        [HttpDelete()]
        [Authorize(Policy = "Driver")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> RemoveVehicle()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var driverId = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.DriverId)?.Value;

            var driver = _usersRepository.GetDriverById(Guid.Parse(driverId));
            var vehicle = driver?.Vehicle;
            
            if (vehicle == null)
            {
                return NotFound();
            }

            var res = await _usersRepository.RemoveVehicle(vehicle);

            if (!res)
                return Conflict();

            return NoContent();
        }
        
        [HttpGet("{id}", Name = "GetVehicle")]
        [Authorize]
        public async Task<IActionResult> GetVehicle(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var vehicle = await _usersRepository.GetVehicle(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            var vehicleToReturn = _mapper.Map<VehicleToReturnDto>(vehicle);
            vehicleToReturn.Pictures = new List<string>();
            foreach (var p in vehicle.Pictures)
            {
                vehicleToReturn.Pictures.Add(p.Id);
            }

            return Ok(vehicleToReturn);
        }

        [HttpGet( Name = "GetDriverVehicle")]
        [Authorize(Policy = "DriverReg")]
        public IActionResult GetDriverVehicle()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var driverId = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.DriverId)?.Value;

            var driver = _usersRepository.GetDriverById(Guid.Parse(driverId));
            var vehicle = driver?.Vehicle;
            if (vehicle == null)
            {
                return NotFound();
            }
            var vehicleToReturn = _mapper.Map<VehicleToReturnDto>(vehicle);
            vehicleToReturn.Pictures = new List<string>();
            foreach (var p in vehicle.Pictures)
            {
                vehicleToReturn.Pictures.Add(p.Id);
            }

            return Ok(vehicleToReturn);
        }

        [HttpDelete("images/{id}")]
        [Authorize(Policy = "Driver")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> RemoveVehicleImage(string id)
        {
            var driverId = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.DriverId)?.Value;

            var driver = _usersRepository.GetDriverById(Guid.Parse(driverId));

            var res = await _usersRepository.RemoveVehicleImage(driver, id);

            if (res == false)
                return NotFound();
            return NoContent();
        }

        [HttpPut("images")]
        [Authorize(Policy = "DriverReg")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPut(List<IFormFile> files)
        {
            //var files = new List<IFormFile>();

            //files.Add(file);

            long size = files.Sum(f => f.Length);

            var driverId = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.DriverId)?.Value;

            var driver = _usersRepository.GetDriverById(Guid.Parse(driverId));

            if (driver?.Vehicle == null)
            {
                ModelState.AddModelError(nameof(driver.Vehicle), "Driver has no vehicle to add images");

                return BadRequest(ModelState);
            }

            foreach(var v in driver.Vehicle.Pictures.ToList())
            {
                var res = await _usersRepository.RemoveVehicleImage(driver, v.Id);
                if (!res)
                    return Conflict();
            }

            var toReturn = new List<ImageToReturnDto>();
            foreach (var formFile in files)
            {
                if (!formFile.IsImage())
                {
                    return BadRequest();
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
                    var res = await _usersRepository.AddPictureToVehicle(driver.Vehicle, imageId);
                    if (!res)
                        return Conflict();
                    System.IO.File.Delete(filename);
                    toReturn.Add(new ImageToReturnDto() { ImageId = imageId });
                }
            }
            return Ok(toReturn);
        }

        [HttpPost("images")]
        [Authorize(Policy = "DriverReg")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            //var files = new List<IFormFile>();

            //files.Add(file);

            long size = files.Sum(f => f.Length);

            var driverId = User.Claims.FirstOrDefault(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.DriverId)?.Value;

            var driver = _usersRepository.GetDriverById(Guid.Parse(driverId));

            if (driver?.Vehicle == null)
            {
                ModelState.AddModelError(nameof(driver.Vehicle), "Driver has no vehicle to add images");

                return BadRequest(ModelState);
            }
            var toReturn = new List<ImageToReturnDto>();
            foreach (var formFile in files)
            {
                if (!formFile.IsImage())
                {
                    return BadRequest();
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
                    var res = await _usersRepository.AddPictureToVehicle(driver.Vehicle, imageId);
                    if (!res)
                        return Conflict();
                    System.IO.File.Delete(filename);
                    toReturn.Add(new ImageToReturnDto() { ImageId = imageId});
                }
            }
            return Ok(toReturn);
        }
    }
}

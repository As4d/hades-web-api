using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DataAccess;
using HadesDemoWebApi.Models;
using Microsoft.AspNetCore.Cors;

namespace HadesDemoWebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class DataAccessController : ControllerBase
    {
        private readonly IRepository _repository;

        public DataAccessController(IRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet]
        [Route("GetVulnerabilitySummary/{osVersion}")]
        public List<VulnerabilitySummary> GetVulnerabilitySummary(string osVersion)
        {
            return _repository.GetVulnerabilitySummary(osVersion);
        }

        [HttpGet]
        [Route("GetVulnerabilityMetric/{osVersion}")]
        public List<VulnerabilityMetric> VulnerabilityMetric(string osVersion)
        {
            return _repository.GetVulnerabilityMetric(osVersion);
        }

        // POST api/<DataAccessController>
        [Route("createuser")]
        [HttpPost]
        [EnableCors("AllowOrigins")]
        public bool Post([FromBody] User user)
        {
            try
            {
                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    IpAddress = user.IpAddress,
                    LastScan = DateTime.Now, /*user.LastScan,*/
                    NoOfScans = user.NoOfScans,
                    OperatingSystem = user.OperatingSystem,
                    TotalFileCount = user.TotalFileCount
                };

                var isSuccess = _repository.CreateUser(userDto);

                return isSuccess;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        [Route("createuserfiles")]
        [HttpPost]
        [EnableCors("AllowOrigins")]
        public bool PostUserFiles([FromBody] UserFilesInfo userFilesInfo)
        {
            try
            {
                var userFilesInfoDto = new UserFilesInfoDto
                {
                    UserId = userFilesInfo.UserId,
                    FileCount = userFilesInfo.FileCount,
                    FileType = userFilesInfo.FileType
                };

                var isSuccess = _repository.CreateUserFilesInfo(userFilesInfoDto);

                return isSuccess;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [Route("updateuser")]
        [HttpPut]
        public int Put(int id, [FromBody] User user)
        {
            try
            {
                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    IpAddress = user.IpAddress,
                    LastScan = DateTime.Now, /*user.LastScan,*/
                    NoOfScans = user.NoOfScans,
                    OperatingSystem = user.OperatingSystem,
                    TotalFileCount = user.TotalFileCount
                };

                var userUpdated = _repository.UpdateUser(userDto);

                return userUpdated;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [Route("updateuserfiles")]
        [HttpPut]
        [EnableCors("AllowOrigins")]
        public int PutUserFiles([FromBody] UserFilesInfo userFilesInfo)
        {
            try
            {
                var userFilesInfoDto = new UserFilesInfoDto
                {
                    UserId = userFilesInfo.UserId,
                    FileCount = userFilesInfo.FileCount,
                    FileType = userFilesInfo.FileType
                };

                var noOfFilesUpdated = _repository.UpdateUserFilesInfo(userFilesInfoDto);

                return noOfFilesUpdated;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DataAccess;
using HadesDemoWebApi.Models;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        
        // GET: api/<DataAccessController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2", "value3" };
        }

        // may remove
        // GET api/<DataAccessController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
        public bool Put(int id, [FromBody] User user)
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

                var isSuccess = _repository.UpdateUser(userDto);

                return isSuccess;
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

        // DELETE api/<DataAccessController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

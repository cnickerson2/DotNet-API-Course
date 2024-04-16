using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserEFController : ControllerBase
    {
        DataContextEF _entityFramework;
        IMapper _mapper;

        public UserEFController(IConfiguration config) 
        {
            _entityFramework = new DataContextEF(config);
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserToAddDTO, User>();
            }));
        }

        [HttpGet("GetUsers")]
        //public IActionResult Test()
        public IEnumerable<User> GetUsers()
        {
            ;
            IEnumerable<User> users = _entityFramework.Users.ToList();
            return users;
        }

        [HttpGet("GetSingleUser/{userId}")]
        //public IActionResult Test()
        public User GetSingleUser(int userId)
        {

            User? user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault();

            if (user != null)
            {
                return user;
            }

            throw new Exception("Failed to get User");
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {

            User userDb = GetSingleUser(user.UserId);

            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;
            userDb.Active = user.Active;

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserToAddDTO user)
        {
            User userDb = _mapper.Map<User>(user);

            _entityFramework.Add(userDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Add User");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            User userDb = GetSingleUser(userId);


            _entityFramework.Users.Remove(userDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }

        [HttpGet("GetUserSalaries")]
        //public IActionResult Test()
        public IEnumerable<UserSalary> GetUserSalaries()
        {
            ;
            IEnumerable<UserSalary> userSalaries = _entityFramework.UserSalary.ToList();
            return userSalaries;
        }

        [HttpGet("GetSingleUserSalary/{userId}")]
        //public IActionResult Test()
        public UserSalary GetSingleUserSalary(int userId)
        {

            UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault();

            if (userSalary != null)
            {
                return userSalary;
            }

            throw new Exception("Failed to get User Salary");
        }

        [HttpPut("EditUserSalary")]
        public IActionResult EditUserSalary(UserSalary userSalary)
        {

            UserSalary userSalaryDb = GetSingleUserSalary(userSalary.UserId);

            userSalaryDb.Salary = userSalary.Salary;

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Update User Salary");
        }

        [HttpPost("AddUserSalary")]
        public IActionResult AddUserSalary(UserSalary userSalary)
        {
            UserSalary userSalaryDb = _mapper.Map<UserSalary>(userSalary);

            _entityFramework.Add(userSalaryDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Add UserSalary");
        }

        [HttpDelete("DeleteUserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            UserSalary userSalaryDb = GetSingleUserSalary(userId);

            _entityFramework.UserSalary.Remove(userSalaryDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Delete UserSalary");
        }

        [HttpGet("GetUserJobInfos")]
        //public IActionResult Test()
        public IEnumerable<UserJobInfo> GetUserJobInfos()
        {
            ;
            IEnumerable<UserJobInfo> userJobInfos = _entityFramework.UserJobInfo.ToList();
            return userJobInfos;
        }

        [HttpGet("GetSingleUserJobInfo/{userId}")]
        //public IActionResult Test()
        public UserJobInfo GetSingleUserJobInfo(int userId)
        {

            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo.Where(u => u.UserId == userId).FirstOrDefault();

            if (userJobInfo != null)
            {
                return userJobInfo;
            }

            throw new Exception("Failed to get User Job Info");
        }

        [HttpPut("EditUserJobInfo")]
        public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
        {

            UserJobInfo userJobInfoDb = GetSingleUserJobInfo(userJobInfo.UserId);

            userJobInfoDb.JobTitle = userJobInfo.JobTitle;
            userJobInfoDb.Department = userJobInfo.Department;

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Update User Job Info");
        }

        [HttpPost("AddUserJobInfo")]
        public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
        {
            UserJobInfo userJobInfoDb = _mapper.Map<UserJobInfo>(userJobInfo);

            _entityFramework.Add(userJobInfoDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Add User Job Info");
        }

        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            UserJobInfo userJobInfoDb = GetSingleUserJobInfo(userId);

            _entityFramework.UserJobInfo.Remove(userJobInfoDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User Job Info");
        }
    }
}

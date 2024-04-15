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
        {;
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

            if(_entityFramework.SaveChanges() > 0)
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
    }
}

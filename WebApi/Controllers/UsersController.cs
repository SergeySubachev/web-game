using System;
using AutoMapper;
using Game.Domain;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        [HttpGet("{userId}", Name = nameof(GetUserById))]
        public ActionResult<UserDto> GetUserById([FromRoute] Guid userId)
        {
            var userEntity = userRepository.FindById(userId);

            if (userEntity == null)
            {
                return NotFound();
            }

            var userDto = mapper.Map<UserDto>(userEntity);
            return Ok(userDto);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserUpdateDto userUpdateDto)
        {
            var createdUserEntity = mapper.Map<UserEntity>(userUpdateDto);
            userRepository.Insert(createdUserEntity);
            return CreatedAtRoute(nameof(GetUserById), new { userId = createdUserEntity.Id }, userUpdateDto);
        }
    }
}
using ASPNetCoreAPI.Models;
using ASPNetCoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASPNetCoreAPI.Controllers
{
    /// <summary>
    /// User API(s)
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        
        /// <summary>
        /// UserController Constructor
        /// </summary>
        /// <param name="userService"></param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// User login API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /user/login
        ///     {
        ///         "username": "john_doe",
        ///         "password": "secret"
        ///     }
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Authenticate(UserAuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);
            return Ok(response);
        }
        
        /// <summary>
        /// Register a new user API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Register(UserRegisterRequest model)
        {
            _userService.Register(model);
            return Created("Ok", new { message = "Register new user successfully" });
        }

        /// <summary>
        /// Get all users API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetById(long id)
        {
            var user = _userService.GetById(id);
            return Ok(user);
        }

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Update(long id, UserUpdateRequest model)
        {
            _userService.Update(id, model);
            return NoContent();
        }

        /// <summary>
        /// Active or de-active user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("bulk/active")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateActive(UserUpdateActiveRequest model)
        {
            _userService.UpdateActive(model);
            return NoContent();
        }
    }
}
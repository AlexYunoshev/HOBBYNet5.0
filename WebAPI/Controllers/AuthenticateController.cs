//using Domain.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace WebAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthenticateController : ControllerBase
//    {
//        [HttpPost("authenticate")]
//        public IActionResult Authenticate(AuthenticateRequest model)
//        {
//            var response = _userService.Authenticate(model);

//            if (response == null)
//                return BadRequest(new { message = "Username or password is incorrect" });

//            return Ok(response);
//        }

//        [HttpPost("register")]
//        public async Task<IActionResult> Register(User userModel)
//        {
//            var response = await _userService.Register(userModel);

//            if (response == null)
//            {
//                return BadRequest(new { message = "Didn't register!" });
//            }

//            return Ok(response);
//        }

//        [Authorize]
//        [HttpGet]
//        public IActionResult GetAll()
//        {
//            var users = _userService.GetAll();
//            return Ok(users);
//        }
//    }
//}

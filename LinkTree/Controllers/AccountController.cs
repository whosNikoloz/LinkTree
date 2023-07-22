using LinkTree.Models;
using LinkTree.Models.LoginRequest;
using LinkTree.Models.UserDTO.RequestsDTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using LinkTree.Models.UserDTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.SqlServer.Server;
using Azure.Core;
using System.Text.RegularExpressions;

namespace LinkTree.Controllers
{
    public class AccountController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5074");
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;

            _httpContextAccessor = httpContextAccessor;

        }

        [HttpPost]
        public async Task<IActionResult> SingIn(UserLoginEmailRequest userLogin)
        {
            try
            {
                if(userLogin == null)
                {
                    return BadRequest("Fill in");
                }

                var response = await _client.PostAsJsonAsync("/api/User/loginWithEmail", userLogin);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    var User = JsonConvert.DeserializeObject<User>(data);
                    if(User!=null)
                    {
                        var jwtToken = User.VerificationToken;
                        _httpContextAccessor.HttpContext.Session.SetString("AccessToken", jwtToken);
                        _httpContextAccessor.HttpContext.Session.SetString("User", JsonConvert.SerializeObject(User));

                        // Generate the URL for the "Index" action of the "Home" controller
                        var homeIndexUrl = Url.Action("Index", "Home");

                        // Redirect to the dynamically generated URL
                        return Redirect(homeIndexUrl);
                    }
                    else
                    {
                        var error = "no User was Found";
                        return BadRequest(error);
                    }
                    
                }
                else
                {
                    // Handle error response
                    var error = await response.Content.ReadAsStringAsync();
                    return BadRequest(error);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the login process
                // You can log the exception or return a custom error response as needed.
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during login.");
            }
        }


        [HttpPost]
		public async Task<IActionResult> SignUp(UserRegisterRequest userRegister)
		{
            if(userRegister == null)
            {
                return BadRequest("Fill Gap");
            }
            var response = await _client.PostAsJsonAsync("/api/User/register", userRegister);
            if (response.IsSuccessStatusCode)
            {
                var homeIndexUrl = Url.Action("RegisterConfirm", "Account");

                return Redirect(homeIndexUrl);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                // Remove the access token and user information from the session
                _httpContextAccessor.HttpContext.Session.Remove("AccessToken");
                _httpContextAccessor.HttpContext.Session.Remove("User");

                // Redirect to the "Index" action of the "Home" controller
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during logout
                // You can log the exception or return a custom error response as needed.
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during logout.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
			if (string.IsNullOrEmpty(token))
			{
				// Handle the case when the token is missing or invalid
				return BadRequest("Invalid token");
			}

			// Do something with the token, if needed...

			var request = new ResetPasswordRequest()
			{
				Token = token
			};

			return View(request);
        }

        [HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
		{   
            if(request == null)
            {
                return BadRequest("request null");
            }
			var response = await _client.PostAsJsonAsync("/api/User/Reset-password", request);
            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction("ResetPasswordConfirm", "Account");
            }
            else
            {
				var error = await response.Content.ReadAsStringAsync();
				return BadRequest(error);
			}
		}

		public IActionResult ResetPasswordConfirm()
		{
			return View();
		}



		public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> ForgetPassword(string email)
		{

            var response = await _client.PostAsJsonAsync("/api/User/Forgot-password/", email );

			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction("ForgetPasswordConfirm", "Account");
            }
            else
            {
				var error = await response.Content.ReadAsStringAsync();
				return BadRequest(error);
			}
		}




		public IActionResult Authentication()
		{
			return View();
		}

        public IActionResult RegisterConfirm()
        {
            return View();
        }

        public IActionResult ForgetPasswordConfirm()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }
    }
}

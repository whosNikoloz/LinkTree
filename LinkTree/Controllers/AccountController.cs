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
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;
using LinkTree.Models.LinksDTO;

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
                    var UserFromApi = JsonConvert.DeserializeObject<UserForApi>(data);

                    var User = ConvertApiuserToUser(UserFromApi);


                    if(User!=null)
                    {
                        var linkuserresponse = await _client.PostAsJsonAsync("/api/Link/UserWithLinks", User.Email);
                        if (linkuserresponse.IsSuccessStatusCode)
                        {
                            string datawithlinks = await linkuserresponse.Content.ReadAsStringAsync();
                            var UserFromApiLinks = JsonConvert.DeserializeObject<UserForApi>(datawithlinks);
                            var Userwihtlink = ConvertApiuserToUser(UserFromApiLinks);

                            _httpContextAccessor.HttpContext.Session.SetString("User", JsonConvert.SerializeObject(Userwihtlink));
                        }


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
                _httpContextAccessor.HttpContext.Session.Clear();

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



        public async Task<IActionResult> UserControllPanel()
        {
            var serializedUser =  _httpContextAccessor.HttpContext.Session.GetString("User");

            


            // Check if the User data exists in the session
            if (string.IsNullOrEmpty(serializedUser))
            {
                // If not found, handle the situation accordingly (e.g., redirect to an error page)
                return RedirectToAction("Error");
            }

            // Deserialize the User object
            var user = JsonConvert.DeserializeObject<User>(serializedUser);

            var linkuserresponse = await _client.PutAsJsonAsync("/api/Link/UserWithLinks", user.Email);
            if (linkuserresponse.IsSuccessStatusCode)
            {
                string datawithlinks = await linkuserresponse.Content.ReadAsStringAsync();
                var UserFromApiLinks = JsonConvert.DeserializeObject<UserForApi>(datawithlinks);
                var Userwihtlink = ConvertApiuserToUser(UserFromApiLinks);

                _httpContextAccessor.HttpContext.Session.SetString("User", JsonConvert.SerializeObject(Userwihtlink));
                return View(Userwihtlink);
            }
            else
            {
                return BadRequest();
            }

        }


        
        [HttpPost]
        public async  Task<IActionResult> ChangeNumberOrUserName(UserForApi updateduser)
        {

            if(updateduser == null)
            {
                return BadRequest("userNull");
            }

            var serializedUser = _httpContextAccessor.HttpContext.Session.GetString("User");
            var generalUser = JsonConvert.DeserializeObject<User>(serializedUser);

            if(generalUser == null)
            {
                return RedirectToAction("index" ,"home");
            }
            generalUser.PhoneNumber = updateduser.PhoneNumber;
            generalUser.UserName = updateduser.UserName;


            var apiUser = ConvertUserToUserForApi(generalUser);



			var response = await _client.PostAsJsonAsync("/api/User/Change-usernameornumber/", apiUser);
            if(response.IsSuccessStatusCode)
            {
                _httpContextAccessor.HttpContext.Session.Clear();


                _httpContextAccessor.HttpContext.Session.SetString("User", JsonConvert.SerializeObject(generalUser));

                return RedirectToAction("UserControllPanel");
            }

            return RedirectToAction("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Changepassword(string oldpassword, string newpassword)
        {
            if (oldpassword == null || newpassword == null)
            {
                return BadRequest("Null");
            }

            var serializedUser = _httpContextAccessor.HttpContext.Session.GetString("User");
            var generalUser = JsonConvert.DeserializeObject<User>(serializedUser);

            if (generalUser == null)
            {
                return RedirectToAction("index", "home");
            }

            var apiUser = ConvertUserToUserForApi(generalUser);

			var jsonRequestBody = JsonConvert.SerializeObject(apiUser);

            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            // Add the query parameters
            var queryString = $"?oldpassword={oldpassword}&newpassword={newpassword}";
            var fullUrl = "/api/User/Change-password/" + queryString;

            var response = await _client.PostAsync(fullUrl, content);

            if (response.IsSuccessStatusCode)
            {
                // Get the updated user from the API
                var updatedUser = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

                _httpContextAccessor.HttpContext.Session.Clear();


                // Update the user in the session
                _httpContextAccessor.HttpContext.Session.SetString("User", JsonConvert.SerializeObject(updatedUser));

                return RedirectToAction("UserControllPanel");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> uploadimg(IFormFile image)
        {
            if(image == null)
            {
                return BadRequest();
            }
			using var stream = new MemoryStream();
			await image.CopyToAsync(stream);
			var imageData = stream.ToArray();

			var imageDataString = Convert.ToBase64String(imageData);
			var serializedUser = _httpContextAccessor.HttpContext.Session.GetString("User");
			var generalUser = JsonConvert.DeserializeObject<User>(serializedUser);

			if (generalUser == null)
			{
				return RedirectToAction("index", "home");
			}

            var apiuser = ConvertUserToUserForApi(generalUser);
            apiuser.Picture = imageDataString;


			var response = await _client.PostAsJsonAsync("/api/User/userimage/", apiuser);
            if (response.IsSuccessStatusCode)
            {

               
                var getuserResponse = await _client.PostAsJsonAsync("/api/User/UserName/", apiuser.UserName);


                if(getuserResponse.IsSuccessStatusCode)
                {
                    string responednUser = await getuserResponse.Content.ReadAsStringAsync();
                    var UserFromApi = JsonConvert.DeserializeObject<UserForApi>(responednUser);

                    var updatedUser = ConvertApiuserToUser(UserFromApi);
                    _httpContextAccessor.HttpContext.Session.Clear();


                    _httpContextAccessor.HttpContext.Session.SetString("User", JsonConvert.SerializeObject(updatedUser));
                    return RedirectToAction("UserControllPanel", "Account");

                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return BadRequest(error);
                }
			}
            else
            {
				var error = await response.Content.ReadAsStringAsync();
				return BadRequest(error);
			}

		}


        //Linkss,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,

        [HttpPost]
        public async Task<IActionResult> addlink(string url,string name)
        {
            var newlink = new LinkRequest
            {
                Description = "Not filled",
                Name = name,
                link = url,
            };
            var serializedUser = _httpContextAccessor.HttpContext.Session.GetString("User");
            var generalUser = JsonConvert.DeserializeObject<User>(serializedUser);


            var jsonRequestBody = JsonConvert.SerializeObject(newlink);

            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");


            // Add the query parameters
            var queryString = $"?user={generalUser.UserName}";
            var fullUrl = "/api/Link/UploadUserLink/" + queryString;

            var response = await _client.PostAsync(fullUrl, content);

            if(response.IsSuccessStatusCode)
            {
                var linkuserresponse = await _client.PostAsJsonAsync("/api/Link/UserWithLinks", generalUser.Email);
                if (linkuserresponse.IsSuccessStatusCode)
                {
                    string datawithlinks = await linkuserresponse.Content.ReadAsStringAsync();
                    var UserFromApiLinks = JsonConvert.DeserializeObject<UserForApi>(datawithlinks);
                    var Userwihtlink = ConvertApiuserToUser(UserFromApiLinks);

                    _httpContextAccessor.HttpContext.Session.SetString("User", JsonConvert.SerializeObject(Userwihtlink));
                    return RedirectToAction("Main" ,"Home");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return BadRequest(error);
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> deletelink(int linkid)
        {
            var serializedUser = _httpContextAccessor.HttpContext.Session.GetString("User");

            // Check if the User data exists in the session
            if (string.IsNullOrEmpty(serializedUser))
            {
                // If not found, handle the situation accordingly (e.g., redirect to an error page)
                return RedirectToAction("Error");
            }

            // Deserialize the User object
            var user = JsonConvert.DeserializeObject<User>(serializedUser);



            var apiUser = ConvertUserToUserForApi(user);



            // Add the query parameters
            var queryString = $"?user={apiUser.UserName}&id={linkid}";


            var response = await _client.DeleteAsync($"/api/Link/DeleteLink{queryString}");

            if (response.IsSuccessStatusCode)
            {
                // Get the updated user from the API
                var updatedUser = JsonConvert.DeserializeObject<UserForApi>(await response.Content.ReadAsStringAsync());
                var convertedUser = ConvertApiuserToUser(updatedUser);

                // Update the user in the session

                _httpContextAccessor.HttpContext.Session.Clear();


                _httpContextAccessor.HttpContext.Session.SetString("User", JsonConvert.SerializeObject(convertedUser));

                return RedirectToAction("Main" , "Home");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
        }









        private User ConvertApiuserToUser(UserForApi user)
        {
			var mvcUser = new User
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				Picture = Convert.FromBase64String(user.Picture),
				PhoneNumber = user.PhoneNumber,
				PasswordHash = user.PasswordHash,
				PasswordSalt = user.PasswordSalt,
				VerificationToken = user.VerificationToken,
				VerifiedAt = user.VerifiedAt,
				PasswordResetToken = user.PasswordResetToken,
				ResetTokenExpires = user.ResetTokenExpires,
				Role = user.Role,
				Links = user.Links,
			};
            return mvcUser;
		}


		private UserForApi ConvertUserToUserForApi(User user)
		{
			var apiUser = new UserForApi
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				Picture = Convert.ToBase64String(user.Picture),
				PhoneNumber = user.PhoneNumber,
				PasswordHash = user.PasswordHash,
				PasswordSalt = user.PasswordSalt,
				VerificationToken = user.VerificationToken,
				VerifiedAt = user.VerifiedAt,
				PasswordResetToken = user.PasswordResetToken,
				ResetTokenExpires = user.ResetTokenExpires,
				Role = user.Role,
				Links = user.Links,
			};
			return apiUser;
		}

	}
}

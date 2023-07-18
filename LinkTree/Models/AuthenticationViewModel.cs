using LinkTree.Models.LoginRequest;
using LinkTree.Models.UserDTO.RequestsDTO;

namespace LinkTree.Models
{
	public class AuthenticationViewModel
	{
		public UserRegisterRequest userRegister { get; set; }

		public UserLoginEmailRequest userLogin { get; set; }
	}
}

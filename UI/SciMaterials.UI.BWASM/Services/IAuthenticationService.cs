using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services
{
    public interface IAuthenticationService
    {
        Task Logout();
        Task SignIn(SignInForm formData);
        Task<bool> SignUp(SignUpForm formData);
    }
}
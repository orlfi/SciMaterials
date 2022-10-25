using Microsoft.AspNetCore.Identity;
using SciMaterials.Contracts.Auth;

namespace SciMaterials.UI.MVC.Identity.Services;

public interface IAuthUtils : IAuthUtils<IdentityUser> { }

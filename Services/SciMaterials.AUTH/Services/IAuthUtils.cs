using Microsoft.AspNetCore.Identity;
using SciMaterials.Contracts.Auth;

namespace SciMaterials.AUTH.Services;

public interface IAuthUtils : IAuthUtils<IdentityUser> { }

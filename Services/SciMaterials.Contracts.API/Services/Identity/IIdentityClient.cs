using SciMaterials.Contracts.API.DTO.AuthRoles;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.API.DTO.Clients;
using SciMaterials.Contracts.API.DTO.Passwords;

namespace SciMaterials.Contracts.API.Services.Identity;

public interface IIdentityClient : 
    IIdentityUserClient<IdentityClientResponse, AuthUserRequest>,
    IIdentityChangePasswordClient<IdentityClientResponse, ChangePasswordRequest>,
    IIdentityRolesClient<IdentityClientResponse, AuthRoleRequest>,
    IIdentityEditUserClient<IdentityClientResponse, EditUserRequest>
{ }
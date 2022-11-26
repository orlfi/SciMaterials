using SciMaterials.Contracts.Identity.API.Requests.Users;
using SciMaterials.Contracts.Identity.API.Responses.DTO;
using SciMaterials.Contracts.Identity.API.Responses.User;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.API;

public interface IUsersApi
{
    Task<Result<RegisterUserResponse>> RegisterUserAsync(RegisterRequest RegisterRequest, CancellationToken cancel = default);
    
    Task<Result<LoginUserResponse>> LoginUserAsync(LoginRequest LoginRequest, CancellationToken Cancel = default);
    
    Task<Result.Result> LogoutUserAsync(CancellationToken Cancel = default);
    
    Task<Result<AuthUser>> GetUserByEmailAsync(string Email, CancellationToken CancellationToken = default);
    
    Task<Result.Result> ChangePasswordAsync(ChangePasswordRequest ChangePasswordRequest, CancellationToken Cancel = default);
    
    Task<Result<EditUserNameResponse>> EditUserNameByEmailAsync(EditUserNameByEmailRequest editUserRequest, CancellationToken Cancel = default);
    
    Task<Result<IEnumerable<AuthUser>>> GetAllUsersAsync(CancellationToken CancellationToken = default);

    Task<Result.Result> DeleteUserByEmailAsync(string Email, CancellationToken CancellationToken = default);

    Task<Result<RefreshTokenResponse>> GetRefreshTokenAsync(CancellationToken Cancel = default);
}
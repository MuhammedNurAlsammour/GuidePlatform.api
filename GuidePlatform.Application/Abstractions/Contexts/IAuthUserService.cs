namespace GuidePlatform.Application.Abstractions.Contexts
{
	/// <summary>
	/// Auth role management işlemleri için servis interface
	/// </summary>
	public interface IAuthUserService
	{
		Task AssignRoleToUserAsnyc(string userId, string[] roles);
		Task<string[]?> GetRolesToUserAsync(string userIdOrName);
		Task<bool> HasRolePermissionToEndpointAsync(string name, string code);
	}
}

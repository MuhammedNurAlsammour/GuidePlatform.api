
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Abstractions.Services;
using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace GuidePlatform.API.Filters
{
  public class RolePermissionFilter(UserManager<AppUser> userManager) : IAsyncActionFilter
  {
    readonly UserManager<AppUser> _userManager = userManager;


    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      var descriptor = context.ActionDescriptor as ControllerActionDescriptor;

      var attribute = descriptor.MethodInfo.GetCustomAttribute(typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;

      var name = context.HttpContext.User.Identity?.Name;



    }


  }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GuidePlatform.Infrastructure.Filters
{
	/// <summary>
	/// Model validation işlemleri için action filter
	/// </summary>
	public class ValidationFilter : IAsyncActionFilter
	{
		/// <summary>
		/// Action execution sırasında model validation kontrolü yapar
		/// </summary>
		/// <param name="context">Action executing context</param>
		/// <param name="next">Sonraki middleware</param>
		/// <returns>Task</returns>
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			// Model state geçerli değilse hata döndür
			if (!context.ModelState.IsValid)
			{
				var errors = context.ModelState
					.Where(err => err.Value.Errors.Any())
					.ToDictionary(err => err.Key, err => err.Value.Errors.Select(err => err.ErrorMessage))
					.ToArray();

				context.Result = new BadRequestObjectResult(errors);
				return;
			}

			// Model state geçerliyse sonraki middleware'e geç
			await next();
		}
	}
}

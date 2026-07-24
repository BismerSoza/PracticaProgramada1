using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PracticaProgramada1.Filters
{
    public class SessionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var autenticado = context.HttpContext.Session.GetString("Autenticado");

            if (autenticado != "1")
            {
                context.Result = new RedirectToActionResult(
                    "Index",
                    "Home",
                    null);
            }
        }
    }
}
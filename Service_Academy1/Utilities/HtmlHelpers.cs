using Microsoft.AspNetCore.Mvc.Rendering;

namespace Service_Academy1.Utilities
{
    public static class HtmlHelpers
    {
        public static bool IsActive(this IHtmlHelper htmlHelper, string actionName, string controllerName)
        {
            var currentAction = htmlHelper.ViewContext.RouteData.Values["action"]?.ToString();
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"]?.ToString();
            return string.Equals(actionName, currentAction, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(controllerName, currentController, StringComparison.OrdinalIgnoreCase);
        }
    }
}

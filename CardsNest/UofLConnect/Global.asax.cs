using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UofLConnect.Controllers;
using UofLConnect.Utilities;

namespace UofLConnect
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UserInfo.UserID = 0; // Get the user id when the application starts
        }

        // Handle Application Errors
        protected void Application_Error()
        {
            const int INTERNAL_ERROR_CODE = 500;
            const int NOT_FOUND_CODE = 404;
            const int NOT_ALLOWED_CODE = 405;
            const int FORBIDDEN_CODE = 403;

            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            Response.Clear();
            Server.ClearError();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Errors";
            routeData.Values["action"] = "General";
            routeData.Values["exception"] = exception;
            Response.StatusCode = INTERNAL_ERROR_CODE;
            if(httpException != null)
            {
                Response.StatusCode = httpException.GetHttpCode();
                switch (Response.StatusCode)
                {
                    case FORBIDDEN_CODE:
                        routeData.Values["action"] = "Forbidden";
                        break;
                    case NOT_FOUND_CODE:
                        routeData.Values["action"] = "NotFound";
                        break;
                    case NOT_ALLOWED_CODE:
                        routeData.Values["action"] = "NotAllowed";
                        break;
                    case INTERNAL_ERROR_CODE:
                        routeData.Values["action"] = "Internal";
                        break;
                }
            }

            IController errorsController = new ErrorsController();
            var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
            errorsController.Execute(rc);
        }
    }
}

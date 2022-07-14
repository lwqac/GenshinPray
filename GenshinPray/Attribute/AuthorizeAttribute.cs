using GenshinPray.Common;
using GenshinPray.Models;
using GenshinPray.Models.DTO;
using GenshinPray.Models.PO;
using GenshinPray.Service;
using GenshinPray.Type;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace GenshinPray.Attribute
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        private IHttpContextAccessor httpContextAccessor;
        private AuthorizeService authorizeService;
        private RequestRecordService requestRecordService;
        private PrayLimit PrayLimit = PrayLimit.No;
        private PublicLimit PublicLimit = PublicLimit.No;

        public AuthorizeAttribute(IHttpContextAccessor httpContextAccessor, AuthorizeService authorizeService, RequestRecordService requestRecordService, PrayLimit prayLimit = PrayLimit.No, PublicLimit publicLimit = PublicLimit.No)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.authorizeService = authorizeService;
            this.requestRecordService = requestRecordService;
            this.PrayLimit = prayLimit;
            this.PublicLimit = publicLimit;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string authCode = context.HttpContext.Request.Headers["authorzation"];
            if (string.IsNullOrWhiteSpace(authCode))
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new JsonResult(ApiResult.Unauthorized);
                return;
            }

            authCode = authCode.Trim();
            if (PublicLimit == PublicLimit.Yes && authCode == SiteConfig.PublicAuthCode && string.IsNullOrWhiteSpace(SiteConfig.PublicAuthCode) == false)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new JsonResult(ApiResult.PermissionDenied);
                return;
            }

            AuthorizePO authorize = authorizeService.GetAuthorize(authCode);
            if (authorize == null || authorize.IsDisable || authorize.ExpireDate <= DateTime.Now)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new JsonResult(ApiResult.Unauthorized);
                return;
            }
            
            int apiCalledToday = requestRecordService.getRequestTimesToday(authorize.Id);
            if (PrayLimit == PrayLimit.Yes && apiCalledToday >= authorize.DailyCall)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new JsonResult(ApiResult.ApiMaximum);
                return;
            }

            requestRecordService.AddRequestRecord(httpContextAccessor, context.HttpContext.Request, authorize.Id);
            context.ActionArguments["authorizeDto"] = new AuthorizeDto(authorize, apiCalledToday);
        }






    }
}

using GenshinPray.Common;
using GenshinPray.Models;
using GenshinPray.Models.DTO;
using GenshinPray.Models.PO;
using GenshinPray.Service;
using GenshinPray.Type;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace GenshinPray.Attribute
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        private AuthorizeService authorizeService;
        private PrayRecordService prayRecordService;
        private PrayLimit PrayLimit = PrayLimit.No;
        private PublicLimit PublicLimit = PublicLimit.No;


        public AuthorizeAttribute(AuthorizeService authorizeService, PrayRecordService prayRecordService , PrayLimit prayLimit = PrayLimit.No, PublicLimit publicLimit = PublicLimit.No)
        {
            this.authorizeService = authorizeService;
            this.prayRecordService = prayRecordService;
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
            AuthorizePO authorizePO = authorizeService.GetAuthorize(authCode);
            if (authorizePO == null || authorizePO.IsDisable || authorizePO.ExpireDate <= DateTime.Now)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new JsonResult(ApiResult.Unauthorized);
                return;
            }
            if (PublicLimit == PublicLimit.Yes && authCode == SiteConfig.PublicAuthCode)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new JsonResult(ApiResult.PermissionDenied);
                return;
            }
            int prayTimesToday = prayRecordService.GetPrayTimesToday(authorizePO.Id);
            if (PrayLimit == PrayLimit.Yes && prayTimesToday >= authorizePO.DailyCall)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new JsonResult(ApiResult.ApiMaximum);
                return;
            }
            context.ActionArguments["authorize"] = new AuthorizeDTO(authorizePO, prayTimesToday);
        }

    }
}

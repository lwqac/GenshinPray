using GenshinPray.Dao;
using GenshinPray.Models.PO;
using GenshinPray.Util;
using Microsoft.AspNetCore.Http;
using System;

namespace GenshinPray.Service
{
    public class RequestRecordService : BaseService
    {
        private RequestRecordDao requestRecordDao;

        public RequestRecordService(RequestRecordDao requestRecordDao)
        {
            this.requestRecordDao = requestRecordDao;
        }

        public int getRequestTimesToday(int authId)
        {
            DateTime startTime = DateTimeHelper.getTodayStart();
            DateTime endTime = DateTimeHelper.getTodayEnd();
            return requestRecordDao.getRequestTimes(authId, startTime, endTime);
        }

        public RequestRecordPO AddRequestRecord(IHttpContextAccessor httpContextAccessor, HttpRequest request, int authId)
        {
            string ipAddr = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString().ToLower() ?? "";
            RequestRecordPO requestRecord = new RequestRecordPO();
            requestRecord.AuthId = authId;
            requestRecord.Path = request.Path.Value?.Trim()?.CutString(100) ?? "";
            requestRecord.IpAddr = ipAddr?.Trim().CutString(100).ToLower() ?? "";
            requestRecord.CreateDate = DateTime.Now;
            return requestRecordDao.Insert(requestRecord);
        }



    }
}

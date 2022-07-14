using GenshinPray.Models.PO;
using System;

namespace GenshinPray.Dao
{
    public class RequestRecordDao : DbContext<RequestRecordPO>
    {
        public int getRequestTimes(int authId, DateTime startTime, DateTime endTime)
        {
            return Db.Queryable<RequestRecordPO>().Where(o => o.AuthId == authId && o.CreateDate >= startTime && o.CreateDate <= endTime).Count();
        }

    }
}

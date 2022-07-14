using GenshinPray.Models.PO;
using GenshinPray.Type;
using GenshinPray.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshinPray.Dao
{
    public class PrayRecordDao : DbContext<PrayRecordPO>
    {
        public int getPrayTimes(int authId, DateTime startTime, DateTime endTime)
        {
            return Db.Queryable<PrayRecordPO>().Where(o => o.AuthId == authId && o.CreateDate >= startTime && o.CreateDate <= endTime).Count();
        }

        public int getPrayTimes(int authId, string memberCode)
        {
            return Db.Queryable<PrayRecordPO>().Where(o => o.AuthId == authId && o.MemberCode == memberCode).Sum(o => o.PrayCount);
        }

        public int getPrayTimes(int authId, string memberCode, YSPondType pondType)
        {
            return Db.Queryable<PrayRecordPO>().Where(o => o.AuthId == authId && o.MemberCode == memberCode && o.PondType == pondType).Sum(o => o.PrayCount);
        }

        public int clearPrayRecord(int authId, string memberCode)
        {
            return Db.Deleteable<PrayRecordPO>().Where(o => o.AuthId == authId && o.MemberCode == memberCode).ExecuteCommand();
        }


    }
}

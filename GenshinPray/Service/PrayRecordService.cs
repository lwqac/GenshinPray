using GenshinPray.Dao;
using GenshinPray.Models.PO;
using GenshinPray.Type;
using GenshinPray.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinPray.Service
{
    public class PrayRecordService : BaseService
    {
        private PrayRecordDao prayRecordDao;

        public PrayRecordService(PrayRecordDao prayRecordDao)
        {
            this.prayRecordDao = prayRecordDao;
        }

        public int GetPrayTimesToday(int authId)
        {
            DateTime startTime = DateTimeHelper.getTodayStart();
            DateTime endTime = DateTimeHelper.getTodayEnd();
            return prayRecordDao.getPrayTimes(authId, startTime, endTime);
        }

        public PrayRecordPO AddPrayRecord(YSPondType pondType, int authId, int pondIndex, string memberCode, int prayCount)
        {
            PrayRecordPO prayRecord = new PrayRecordPO();
            prayRecord.AuthId = authId;
            prayRecord.MemberCode = memberCode;
            prayRecord.PondType = pondType;
            prayRecord.PondIndex = pondIndex;
            prayRecord.PrayCount = prayCount;
            prayRecord.CreateDate = DateTime.Now;
            return prayRecordDao.Insert(prayRecord);
        }

        public int ClearPrayRecord(int authId, string memberCode)
        {
            return prayRecordDao.clearPrayRecord(authId, memberCode);
        }

    }
}

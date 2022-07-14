﻿using GenshinPray.Models.DTO;
using GenshinPray.Models.PO;
using GenshinPray.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshinPray.Dao
{
    public class MemberGoodsDao : DbContext<MemberGoodsPO>
    {
        public int CountByMember(int authId, string memberCode, YSRareType rareType)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select count(mg.Id) count from member_goods mg");
            sqlBuilder.Append(" inner join goods g on g.Id=mg.GoodsId");
            sqlBuilder.Append(" where mg.AuthId=@AuthId and mg.MemberCode=@MemberCode and g.RareType=@RareType");
            return Db.Ado.SqlQuery<int>(sqlBuilder.ToString(), new { AuthId = authId, MemberCode = memberCode, RareType = rareType }).First();
        }

        public int CountByMember(int authId, string memberCode, YSPondType pondType, YSRareType rareType)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select count(mg.Id) count from member_goods mg");
            sqlBuilder.Append(" inner join goods g on g.Id=mg.GoodsId");
            sqlBuilder.Append(" where mg.AuthId=@AuthId and mg.MemberCode=@MemberCode and g.RareType=@RareType and mg.PondType=@PondType");
            return Db.Ado.SqlQuery<int>(sqlBuilder.ToString(), new { AuthId = authId, MemberCode = memberCode, PondType = pondType, RareType = rareType }).First();
        }

        /// <summary>
        /// 统计一个时间段内，同一个authId下某一个星级的出货率排行
        /// </summary>
        /// <param name="authId"></param>
        /// <param name="top"></param>
        /// <param name="rareType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<LuckRankingDto> getLuckRanking(int authId, int top, YSRareType rareType, DateTime startDate, DateTime endDate)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select temp.AuthId, temp.MemberCode, m.MemberName, temp.RareType, temp.RareCount,");
            sqlBuilder.Append(" temp2.TotalPrayTimes, temp.rareCount/temp2.TotalPrayTimes as RareRate from member m");
            sqlBuilder.Append(" inner join (");
            sqlBuilder.Append(" 	select mg.AuthId,mg.MemberCode,g.RareType,count(g.RareType) RareCount from member_goods mg");
            sqlBuilder.Append("     inner join goods g on g.Id=mg.GoodsId");
            sqlBuilder.Append(" 	where mg.AuthId=@AuthId and g.RareType=@RareType and mg.CreateDate>=@StartDate and mg.CreateDate<@EndDate");
            sqlBuilder.Append(" 	group by mg.AuthId,mg.MemberCode,g.RareType limit @Top");
            sqlBuilder.Append(" ) temp on temp.MemberCode=m.MemberCode");
            sqlBuilder.Append(" inner join (");
            sqlBuilder.Append(" 	select AuthId,MemberCode,sum(PrayCount) TotalPrayTimes from pray_record");
            sqlBuilder.Append(" 	where AuthId=@AuthId and CreateDate>=@StartDate and CreateDate<@EndDate");
            sqlBuilder.Append(" 	group by AuthId,MemberCode");
            sqlBuilder.Append(" ) temp2 on temp2.MemberCode=m.MemberCode");
            sqlBuilder.Append(" where m.AuthId=@AuthId order by temp.RareType desc,rareRate desc,temp.RareCount asc");
            return Db.Ado.SqlQuery<LuckRankingDto>(sqlBuilder.ToString(), new { AuthId = authId, Top = top, RareType = rareType, StartDate = startDate, EndDate = endDate });
        }

        public List<PrayRecordDto> getPrayRecords(int authId, string memberCode, YSRareType rareType, int top)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select mg.GoodsId,g.GoodsName,g.GoodsType,g.GoodsSubType,g.RareType,mg.PondType,mg.Cost,mg.CreateDate from member_goods mg");
            sqlBuilder.Append(" inner join goods g on g.Id=mg.GoodsId");
            sqlBuilder.Append(" where mg.AuthId=@AuthId and mg.MemberCode=@MemberCode and g.RareType=@RareType");
            sqlBuilder.Append(" order by mg.CreateDate desc limit @Top");
            return Db.Ado.SqlQuery<PrayRecordDto>(sqlBuilder.ToString(), new { AuthId = authId, MemberCode = memberCode, RareType = rareType, Top = top });
        }

        public List<PrayRecordDto> getPrayRecords(int authId, string memberCode, YSRareType rareType, YSPondType pondType, int top)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" select mg.GoodsId,g.GoodsName,g.GoodsType,g.GoodsSubType,g.RareType,mg.PondType,mg.Cost,mg.CreateDate from member_goods mg");
            sqlBuilder.Append(" inner join goods g on g.Id=mg.GoodsId");
            sqlBuilder.Append(" where mg.AuthId=@AuthId and mg.MemberCode=@MemberCode and g.RareType=@RareType and mg.PondType=@PondType");
            sqlBuilder.Append(" order by mg.CreateDate desc limit @Top");
            return Db.Ado.SqlQuery<PrayRecordDto>(sqlBuilder.ToString(), new { AuthId = authId, MemberCode = memberCode, RareType = rareType, PondType = pondType, Top = top });
        }

        public int clearMemberGoods(int authId, string memberCode)
        {
            return Db.Deleteable<MemberGoodsPO>().Where(o => o.AuthId == authId && o.MemberCode == memberCode).ExecuteCommand();
        }


    }
}

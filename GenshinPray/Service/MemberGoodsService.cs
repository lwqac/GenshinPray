using GenshinPray.Common;
using GenshinPray.Dao;
using GenshinPray.Models;
using GenshinPray.Models.DTO;
using GenshinPray.Models.PO;
using GenshinPray.Models.VO;
using GenshinPray.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinPray.Service
{
    public class MemberGoodsService : BaseService
    {
        private MemberGoodsDao memberGoodsDao;
        private PrayRecordDao prayRecordDao;

        public MemberGoodsService(MemberGoodsDao memberGoodsDao, PrayRecordDao prayRecordDao)
        {
            this.memberGoodsDao = memberGoodsDao;
            this.prayRecordDao = prayRecordDao;
        }

        public void AddMemberGoods(YSPrayResult ySPrayResult, List<MemberGoodsDto> memberGoods, YSPondType pondType, int authId, string memberCode)
        {
            foreach (var result in ySPrayResult.PrayRecords)
            {
                if (result.GoodsItem.RareType == YSRareType.三星 && memberGoods.Where(m => m.GoodsName == result.GoodsItem.GoodsName).Any()) continue;
                MemberGoodsPO memberGood = new MemberGoodsPO();
                memberGood.AuthId = authId;
                memberGood.PondType = pondType;
                memberGood.GoodsId = result.GoodsItem.GoodsID;
                memberGood.Cost = result.Cost;
                memberGood.MemberCode = memberCode;
                memberGood.CreateDate = DateTime.Now;
                memberGoodsDao.Insert(memberGood);
            }
        }

        public PrayDetailDto GetMemberPrayDetail(int authId, string memberCode)
        {
            PrayDetailDto prayDetail = new PrayDetailDto();
            prayDetail.Star4Count = memberGoodsDao.CountByMember(authId, memberCode, YSRareType.四星);
            prayDetail.Star5Count = memberGoodsDao.CountByMember(authId, memberCode, YSRareType.五星);
            prayDetail.RoleStar4Count = memberGoodsDao.CountByMember(authId, memberCode, YSPondType.角色, YSRareType.四星);
            prayDetail.ArmStar4Count = memberGoodsDao.CountByMember(authId, memberCode, YSPondType.武器, YSRareType.四星);
            prayDetail.PermStar4Count = memberGoodsDao.CountByMember(authId, memberCode, YSPondType.常驻, YSRareType.四星);
            prayDetail.RoleStar5Count = memberGoodsDao.CountByMember(authId, memberCode, YSPondType.角色, YSRareType.五星);
            prayDetail.ArmStar5Count = memberGoodsDao.CountByMember(authId, memberCode, YSPondType.武器, YSRareType.五星);
            prayDetail.PermStar5Count = memberGoodsDao.CountByMember(authId, memberCode, YSPondType.常驻, YSRareType.五星);
            prayDetail.RolePrayTimes = prayRecordDao.getPrayTimes(authId, memberCode, YSPondType.角色);
            prayDetail.ArmPrayTimes = prayRecordDao.getPrayTimes(authId, memberCode, YSPondType.武器);
            prayDetail.PermPrayTimes = prayRecordDao.getPrayTimes(authId, memberCode, YSPondType.常驻);
            prayDetail.TotalPrayTimes = prayRecordDao.getPrayTimes(authId, memberCode);
            return prayDetail;
        }

        public LuckRankingVO getLuckRanking(int authId, int days, int top)
        {
            LuckRankingVO luckRankingCache = DataCache.GetLuckRankingCache(authId);
            if (luckRankingCache != null) return luckRankingCache;
            DateTime startDate = DateTime.Now.AddDays(-1 * days);
            DateTime endDate = DateTime.Now;
            List<LuckRankingDto> star5RankingList = memberGoodsDao.getLuckRanking(authId, top, YSRareType.五星, startDate, endDate);
            List<LuckRankingDto> star4RankingList = memberGoodsDao.getLuckRanking(authId, top, YSRareType.四星, startDate, endDate);
            LuckRankingVO luckRankingVO = new LuckRankingVO();
            luckRankingVO.Top = top;
            luckRankingVO.CountDay= days;
            luckRankingVO.StartDate = startDate;
            luckRankingVO.EndDate = endDate;
            luckRankingVO.Star5Ranking = star5RankingList.Select(m => toRareRanking(m)).ToList();
            luckRankingVO.Star4Ranking = star4RankingList.Select(m => toRareRanking(m)).ToList();
            DataCache.SetLuckRankingCache(authId, luckRankingVO);
            return luckRankingVO;
        }

        private RareRankingVO toRareRanking(LuckRankingDto luckRankingDTO)
        {
            RareRankingVO rareRankingVO = new RareRankingVO();
            rareRankingVO.TotalPrayTimes = luckRankingDTO.TotalPrayTimes;
            rareRankingVO.MemberCode = luckRankingDTO.MemberCode;
            rareRankingVO.MemberName = luckRankingDTO.MemberName;
            rareRankingVO.Count = luckRankingDTO.RareCount;
            rareRankingVO.Rate = Math.Floor(luckRankingDTO.RareRate * 100 * 1000) / 1000;
            return rareRankingVO;
        }

        public List<PrayRecordDto> getPrayRecords(int authId, string memberCode, YSRareType rareType, int top)
        {
            return memberGoodsDao.getPrayRecords(authId, memberCode, rareType, top);
        }

        public List<PrayRecordDto> getPrayRecords(int authId, string memberCode, YSRareType rareType, YSPondType pondType, int top)
        {
            return memberGoodsDao.getPrayRecords(authId, memberCode, rareType, pondType, top);
        }



    }
}

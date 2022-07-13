using GenshinPray.Common;
using GenshinPray.Dao;
using GenshinPray.Models;
using GenshinPray.Models.DTO;
using GenshinPray.Models.PO;
using GenshinPray.Type;
using GenshinPray.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GenshinPray.Service.PrayService
{
    public abstract class BasePrayService : BaseService
    {
        protected MemberDao memberDao;
        protected GoodsDao goodsDao;

        public BasePrayService() { }

        public BasePrayService(MemberDao memberDao, GoodsDao goodsDao)
        {
            this.memberDao = memberDao;
            this.goodsDao = goodsDao;
        }

        /// <summary>
        /// 显示顺序排序
        /// </summary>
        /// <param name="YSPrayRecords"></param>
        /// <returns></returns>
        public YSPrayRecord[] SortRecords(YSPrayRecord[] YSPrayRecords)
        {
            //先按物品种类排序（0->2），相同种类的物品之间按稀有度倒序排序（5->1）,最后New排在前面
            return YSPrayRecords.OrderBy(c => c.GoodsItem.GoodsType).ThenByDescending(c => c.GoodsItem.RareType).ThenBy(c => c.OwnedCount).ToArray();
        }

        /// <summary>
        /// 从物品列表中随机出一个物品
        /// </summary>
        /// <param name="probabilityList"></param>
        /// <returns></returns>
        protected YSProbability GetRandomInList(List<YSProbability> probabilityList)
        {
            List<YSRegion<YSProbability>> regionList = GetRegionList(probabilityList);
            YSRegion<YSProbability> region = GetRandomInRegion(regionList);
            return region.Item;
        }

        /// <summary>
        /// 将概率转化为一个数字区间
        /// </summary>
        /// <param name="probabilityList"></param>
        /// <returns></returns>
        private List<YSRegion<YSProbability>> GetRegionList(List<YSProbability> probabilityList)
        {
            int sumRegion = 0;//总区间
            List<YSRegion<YSProbability>> regionList = new List<YSRegion<YSProbability>>();//区间列表,抽卡时随机获取该区间
            foreach (var item in probabilityList)
            {
                int startRegion = sumRegion;//开始区间
                sumRegion = startRegion + Convert.ToInt32(item.Probability * 10000);//结束区间
                regionList.Add(new YSRegion<YSProbability>(item, startRegion, sumRegion));
            }
            return regionList;
        }

        /// <summary>
        /// 从区间列表中随机出一个区间
        /// </summary>
        /// <param name="regionList"></param>
        /// <returns></returns>
        private YSRegion<YSProbability> GetRandomInRegion(List<YSRegion<YSProbability>> regionList)
        {
            int randomRegion = RandomHelper.getRandomBetween(0, regionList.Last().EndRegion);
            foreach (var item in regionList)
            {
                if (randomRegion >= item.StartRegion && randomRegion < item.EndRegion) return item;
            }
            return regionList.Last();
        }

        /// <summary>
        /// 从物品列表中随机出一个物品
        /// </summary>
        /// <param name="goodsItemList"></param>
        /// <returns></returns>
        protected YSPrayRecord GetRandomInList(List<YSGoodsItem> goodsItemList)
        {
            List<YSRegion<YSGoodsItem>> regionList = GetRegionList(goodsItemList);
            YSRegion<YSGoodsItem> region = GetRandomInRegion(regionList);
            return new YSPrayRecord(region.Item);
        }

        /// <summary>
        /// 将概率转化为一个数字区间
        /// </summary>
        /// <param name="goodsItemList"></param>
        /// <returns></returns>
        private List<YSRegion<YSGoodsItem>> GetRegionList(List<YSGoodsItem> goodsItemList)
        {
            int sumRegion = 0;//总区间
            List<YSRegion<YSGoodsItem>> regionList = new List<YSRegion<YSGoodsItem>>();//区间列表,抽卡时随机获取该区间
            foreach (var item in goodsItemList)
            {
                int startRegion = sumRegion;//开始区间
                sumRegion = startRegion + Convert.ToInt32(item.Probability * 10000);//结束区间
                regionList.Add(new YSRegion<YSGoodsItem>(item, startRegion, sumRegion));
            }
            return regionList;
        }

        /// <summary>
        /// 从区间列表中随机出一个区间
        /// </summary>
        /// <param name="regionList"></param>
        /// <returns></returns>
        private YSRegion<YSGoodsItem> GetRandomInRegion(List<YSRegion<YSGoodsItem>> regionList)
        {
            int randomRegion = RandomHelper.getRandomBetween(0, regionList.Last().EndRegion);
            foreach (var item in regionList)
            {
                if (randomRegion >= item.StartRegion && randomRegion < item.EndRegion) return item;
            }
            return regionList.Last();
        }

        /// <summary>
        /// 判断一个项目是否up项目
        /// </summary>
        /// <param name="ySUpItem"></param>
        /// <param name="goodsItem"></param>
        /// <returns></returns>
        protected bool IsUpItem(YSUpItem ySUpItem, YSGoodsItem goodsItem)
        {
            if (ySUpItem.Star5UpList.Where(m => m.GoodsName == goodsItem.GoodsName).Count() > 0) return true;
            if (ySUpItem.Star4UpList.Where(m => m.GoodsName == goodsItem.GoodsName).Count() > 0) return true;
            return false;
        }

        /// <summary>
        /// 获取一次五星保底内,成员获得5星角色的累计祈愿次数,0代表还未获得S
        /// </summary>
        /// <param name="YSPrayRecords">祈愿结果</param>
        /// <param name="floorSurplus">剩余N次保底</param>
        /// <param name="maxSurplus">抽出5星最多需要N抽</param>
        /// <returns></returns>
        public int GetStar5Cost(YSPrayRecord[] YSPrayRecords, int floorSurplus, int maxSurplus)
        {
            int star5Index = -1;
            for (int i = 0; i < YSPrayRecords.Length; i++)
            {
                YSGoodsItem YSGoodsItem = YSPrayRecords[i].GoodsItem;
                if (YSGoodsItem.RareType != YSRareType.五星) continue;
                star5Index = i;
                break;
            }
            if (star5Index == -1) return 0;
            return maxSurplus - floorSurplus + star5Index + 1;
        }

        /// <summary>
        /// 创建结果集
        /// </summary>
        /// <param name="ySUpItem"></param>
        /// <param name="ySPrayResult"></param>
        /// <param name="authorize"></param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        public ApiPrayResult CreatePrayResult(YSUpItem ySUpItem, YSPrayResult ySPrayResult, AuthorizeDto authorize, bool toBase64, int imgWidth)
        {
            ApiPrayResult apiResult = new ApiPrayResult();
            apiResult.Star5Cost = ySPrayResult.Star5Cost;
            apiResult.PrayCount = ySPrayResult.PrayRecords.Count();
            apiResult.ApiDailyCallSurplus = authorize.ApiCallSurplus;
            apiResult.Role180Surplus = ySPrayResult.MemberInfo.Role180Surplus;
            apiResult.Role90Surplus = ySPrayResult.MemberInfo.Role180Surplus % 90;
            apiResult.Arm80Surplus = ySPrayResult.MemberInfo.Arm80Surplus;
            apiResult.ArmAssignValue = ySPrayResult.MemberInfo.ArmAssignValue;
            apiResult.Perm90Surplus = ySPrayResult.MemberInfo.Perm90Surplus;
            apiResult.FullRole90Surplus = ySPrayResult.MemberInfo.FullRole90Surplus;
            apiResult.FullArm80Surplus = ySPrayResult.MemberInfo.FullArm80Surplus;
            apiResult.Star5Goods = ChangeToGoodsVO(ySPrayResult.PrayRecords.Where(m => m.GoodsItem.RareType == YSRareType.五星).ToArray());
            apiResult.Star4Goods = ChangeToGoodsVO(ySPrayResult.PrayRecords.Where(m => m.GoodsItem.RareType == YSRareType.四星).ToArray());
            apiResult.Star3Goods = ChangeToGoodsVO(ySPrayResult.PrayRecords.Where(m => m.GoodsItem.RareType == YSRareType.三星).ToArray());
            apiResult.Star5Up = ChangeToGoodsVO(ySUpItem.Star5UpList);
            apiResult.Star4Up = ChangeToGoodsVO(ySUpItem.Star4UpList);
            apiResult.Surplus10 = ySPrayResult.Surplus10;

            bool withSkin = authorize.Authorize.SkinRate > 0 && RandomHelper.getRandomBetween(1, 100) <= authorize.Authorize.SkinRate;
            using Bitmap prayImage = DrawPrayImg(ySPrayResult.SortPrayRecords, withSkin, ySPrayResult.MemberInfo.MemberCode);

            if (toBase64)
            {
                apiResult.ImgBase64 = ImageHelper.ToBase64(prayImage);
            }
            else
            {
                FileInfo prayFileInfo = ImageHelper.saveImageToJpg(prayImage, FilePath.getPrayImgSavePath(), imgWidth);
                apiResult.ImgPath = Path.Combine(prayFileInfo.Directory.Parent.Name, prayFileInfo.Directory.Name, prayFileInfo.Name);
                apiResult.ImgHttpUrl = SiteConfig.PrayImgHttpUrl.Replace("{imgPath}", $"{prayFileInfo.Directory.Parent.Name}/{prayFileInfo.Directory.Name}/{prayFileInfo.Name}");
                apiResult.ImgSize = prayFileInfo.Length;
            }

            return apiResult;
        }

        /// <summary>
        /// 创建结果集
        /// </summary>
        /// <param name="generateData"></param>
        /// <param name="SortPrayRecords"></param>
        /// <param name="authorize"></param>
        /// <returns></returns>
        public ApiGenerateResult CreateGenerateResult(GenerateDataDto generateData, YSPrayRecord[] SortPrayRecords, AuthorizeDto authorize)
        {
            ApiGenerateResult apiResult = new ApiGenerateResult();
            apiResult.PrayCount = SortPrayRecords.Count();
            apiResult.ApiDailyCallSurplus = authorize.ApiCallSurplus;
            apiResult.Star5Goods = ChangeToGoodsVO(SortPrayRecords.Where(m => m.GoodsItem.RareType == YSRareType.五星).ToArray());
            apiResult.Star4Goods = ChangeToGoodsVO(SortPrayRecords.Where(m => m.GoodsItem.RareType == YSRareType.四星).ToArray());
            apiResult.Star3Goods = ChangeToGoodsVO(SortPrayRecords.Where(m => m.GoodsItem.RareType == YSRareType.三星).ToArray());
            using Bitmap prayImage = DrawPrayImg(SortPrayRecords, generateData.UseSkin, generateData.Uid);

            if (generateData.ToBase64)
            {
                apiResult.ImgBase64 = ImageHelper.ToBase64(prayImage);
            }
            else
            {
                FileInfo prayFileInfo = ImageHelper.saveImageToJpg(prayImage, FilePath.getPrayImgSavePath(), generateData.ImgWidth);
                apiResult.ImgPath = Path.Combine(prayFileInfo.Directory.Parent.Name, prayFileInfo.Directory.Name, prayFileInfo.Name);
                apiResult.ImgHttpUrl = SiteConfig.PrayImgHttpUrl.Replace("{imgPath}", $"{prayFileInfo.Directory.Parent.Name}/{prayFileInfo.Directory.Name}/{prayFileInfo.Name}");
                apiResult.ImgSize = prayFileInfo.Length;
            }

            return apiResult;
        }

        /// <summary>
        /// 绘制祈愿结果图片,返回Bitmap
        /// </summary>
        /// <param name="sortPrayRecords"></param>
        /// <param name="withSkin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        protected Bitmap DrawPrayImg(YSPrayRecord[] sortPrayRecords, bool withSkin, string uid)
        {
            if (sortPrayRecords.Count() == 1) return DrawHelper.drawOnePrayImg(sortPrayRecords.First(), withSkin, uid);
            return DrawHelper.drawTenPrayImg(sortPrayRecords, withSkin, uid);
        }

        /// <summary>
        /// 获取一个物品的当前已拥有数量
        /// </summary>
        /// <param name="memberGoods"></param>
        /// <param name="records"></param>
        /// <param name="checkRecord"></param>
        /// <returns></returns>
        protected int GetOwnedCount(List<MemberGoodsDto> memberGoods, YSPrayRecord[] records, YSPrayRecord checkRecord)
        {
            MemberGoodsDto ownedGood = memberGoods.Where(m => m.GoodsName == checkRecord.GoodsItem.GoodsName).FirstOrDefault();
            int ownInDatabase = ownedGood == null ? 0 : ownedGood.Count;
            int ownInRecord = records.Where(m => m != null && m.GoodsItem.GoodsName == checkRecord.GoodsItem.GoodsName).Count();
            return ownInDatabase + ownInRecord;
        }

    }
}

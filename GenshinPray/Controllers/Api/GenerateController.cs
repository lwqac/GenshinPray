using GenshinPray.Attribute;
using GenshinPray.Common;
using GenshinPray.Exceptions;
using GenshinPray.Models;
using GenshinPray.Models.DTO;
using GenshinPray.Models.PO;
using GenshinPray.Service;
using GenshinPray.Type;
using GenshinPray.Util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenshinPray.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GenerateController : BaseController
    {
        protected GoodsService goodsService;
        protected GenerateService generateService;

        public GenerateController(GenerateService generateService, GoodsService goodsService)
        {
            this.goodsService = goodsService;
            this.generateService = generateService;
        }

        /// <summary>
        /// 生成自定义十连结果图
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="generateData"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GenerateTen([FromForm] AuthorizeDto authorizeDto, [FromBody] GenerateDataDto generateData)
        {
            try
            {
                if (generateData == null || generateData.GoodsData.Count == 0 || generateData.GoodsData.Count > 10) throw new ParamException("参数错误");

                List<YSPrayRecord> prayRecords = new List<YSPrayRecord>();
                for (int i = 0; i < generateData.GoodsData.Count; i++)
                {
                    GoodsData goodsData = generateData.GoodsData[i];
                    string goodsName = goodsData.GoodsName.Trim();
                    GoodsPO dbGoods = goodsService.GetGoodsByName(goodsName);
                    if (dbGoods == null) return new ApiResult(ResultCode.GoodsNotFound, $"找不到名为{goodsName}的物品");
                    YSGoodsItem goodsItem = new YSGoodsItem(dbGoods);
                    int ownedCount = goodsData.OwnedCount + prayRecords.Where(o => o.GoodsItem.GoodsName == goodsName).Count();
                    YSPrayRecord prayRecord = new YSPrayRecord(goodsItem, ownedCount, i + 1);
                    prayRecords.Add(prayRecord);
                }

                List<YSPrayRecord> star5Records = prayRecords.Where(o => o.GoodsItem.RareType == YSRareType.五星).ToList();
                List<YSPrayRecord> star4Records = prayRecords.Where(o => o.GoodsItem.RareType == YSRareType.四星).ToList();
                if (star5Records.Count < 1 && star4Records.Count < 1) throw new ParamException("必须包含一个或多个五星或者四星物品");

                while (prayRecords.Count < 10)
                {
                    YSGoodsItem randomItem = DataCache.ArmStar3PermList.Random();
                    prayRecords.Add(new YSPrayRecord(randomItem, 2, 0));
                }

                YSPrayRecord[] sortRecords = generateService.SortRecords(prayRecords.ToArray());
                ApiGenerateResult prayResult = generateService.CreateGenerateResult(generateData, sortRecords, authorizeDto);
                return ApiResult.Success(prayResult);
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }


        /// <summary>
        /// 生成自定义单抽结果图
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="generateData"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GenerateOne([FromForm] AuthorizeDto authorizeDto, [FromBody] GenerateDataDto generateData)
        {
            try
            {
                if (generateData == null || generateData.GoodsData.Count == 0) throw new ParamException("参数错误");
                GoodsData goodsData = generateData.GoodsData.First();
                string goodsName = goodsData.GoodsName.Trim();
                GoodsPO dbGoods = goodsService.GetGoodsByName(goodsName);
                if (dbGoods == null) return new ApiResult(ResultCode.GoodsNotFound, $"找不到名为{goodsName}的物品");
                YSGoodsItem goodsItem = new YSGoodsItem(dbGoods);
                int ownedCount = goodsData.OwnedCount + 1;
                YSPrayRecord prayRecord = new YSPrayRecord(goodsItem, ownedCount, 1);
                YSPrayRecord[] sortRecords = new YSPrayRecord[] { prayRecord };
                ApiGenerateResult prayResult = generateService.CreateGenerateResult(generateData, sortRecords, authorizeDto);
                return ApiResult.Success(prayResult);
            }
            catch (BaseException ex)
            {
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ApiResult.ServerError;
            }
        }


    }
}

using GenshinPray.Attribute;
using GenshinPray.Common;
using GenshinPray.Exceptions;
using GenshinPray.Models;
using GenshinPray.Models.DTO;
using GenshinPray.Models.PO;
using GenshinPray.Models.VO;
using GenshinPray.Service;
using GenshinPray.Type;
using GenshinPray.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenshinPray.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PrayInfoController : BaseController
    {
        protected MemberService memberService;
        protected GoodsService goodsService;
        protected MemberGoodsService memberGoodsService;
        protected PrayRecordService prayRecordService;

        public PrayInfoController(MemberService memberService, GoodsService goodsService, MemberGoodsService memberGoodsService, PrayRecordService prayRecordService)
        {
            this.memberService = memberService;
            this.goodsService = goodsService;
            this.memberGoodsService = memberGoodsService;
            this.prayRecordService = prayRecordService;
        }

        /// <summary>
        /// 获取当前所有祈愿池的up内容
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GetPondInfo([FromForm] AuthorizeDto authorizeDto)
        {
            try
            {
                AuthorizePO authorizePO = authorizeDto.Authorize;
                Dictionary<int, YSUpItem> armUpItemDic = DataCache.DefaultArmItem.Merge<int, YSUpItem>(goodsService.LoadArmItem(authorizePO.Id));
                Dictionary<int, YSUpItem> roleUpItemDic = DataCache.DefaultRoleItem.Merge<int, YSUpItem>(goodsService.LoadRoleItem(authorizePO.Id));
                Dictionary<int, YSUpItem> permUpItemDic = new Dictionary<int, YSUpItem>() { { 0, DataCache.DefaultPermItem } };

                return ApiResult.Success(new
                {
                    arm = armUpItemDic.Select(m => new
                    {
                        pondIndex = m.Key,
                        pondInfo = new
                        {
                            Star5UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star5UpList),
                            Star4UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star4UpList)
                        }
                    }),
                    role = roleUpItemDic.Select(m => new
                    {
                        pondIndex = m.Key,
                        pondInfo = new
                        {
                            Star5UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star5UpList),
                            Star4UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star4UpList)
                        }
                    }),
                    perm = permUpItemDic.Select(m => new
                    {
                        pondIndex = m.Key,
                        pondInfo = new
                        {
                            Star5UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star5UpList),
                            Star4UpList = memberGoodsService.ChangeToGoodsVO(m.Value.Star4UpList)
                        }
                    }),
                });
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
        /// 获取成员祈愿详情
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GetMemberPrayDetail([FromForm] AuthorizeDto authorizeDto, string memberCode)
        {
            try
            {
                checkNullParam(memberCode);
                AuthorizePO authorizePO = authorizeDto.Authorize;
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null) return ApiResult.Success();
                PrayDetailDto prayDetail = memberGoodsService.GetMemberPrayDetail(authorizePO.Id, memberCode);
                return ApiResult.Success(new
                {
                    Role180Surplus = memberInfo.Role180Surplus,
                    Role90Surplus = memberInfo.Role180Surplus % 90,
                    Role10Surplus = memberInfo.Role20Surplus % 10,
                    ArmAssignValue = memberInfo.ArmAssignValue,
                    Arm80Surplus = memberInfo.Arm80Surplus,
                    Arm10Surplus = memberInfo.Arm20Surplus % 10,
                    Perm90Surplus = memberInfo.Perm90Surplus,
                    Perm10Surplus = memberInfo.Perm10Surplus,
                    RolePrayTimes = prayDetail.RolePrayTimes,
                    ArmPrayTimes = prayDetail.ArmPrayTimes,
                    PermPrayTimes = prayDetail.PermPrayTimes,
                    TotalPrayTimes = prayDetail.TotalPrayTimes,
                    Star4Count = prayDetail.Star4Count,
                    Star5Count = prayDetail.Star5Count,
                    RoleStar4Count = prayDetail.RoleStar4Count,
                    ArmStar4Count = prayDetail.ArmStar4Count,
                    PermStar4Count = prayDetail.PermStar4Count,
                    RoleStar5Count = prayDetail.RoleStar5Count,
                    ArmStar5Count = prayDetail.ArmStar5Count,
                    PermStar5Count = prayDetail.PermStar5Count,
                    Star4Rate = NumberHelper.GetRate(prayDetail.Star4Count, prayDetail.TotalPrayTimes),
                    Star5Rate = NumberHelper.GetRate(prayDetail.Star5Count, prayDetail.TotalPrayTimes),
                    RoleStar4Rate = NumberHelper.GetRate(prayDetail.RoleStar4Count, prayDetail.RolePrayTimes),
                    ArmStar4Rate = NumberHelper.GetRate(prayDetail.ArmStar4Count, prayDetail.ArmPrayTimes),
                    PermStar4Rate = NumberHelper.GetRate(prayDetail.PermStar4Count, prayDetail.PermPrayTimes),
                    RoleStar5Rate = NumberHelper.GetRate(prayDetail.RoleStar5Count, prayDetail.RolePrayTimes),
                    ArmStar5Rate = NumberHelper.GetRate(prayDetail.ArmStar5Count, prayDetail.ArmPrayTimes),
                    PermStar5Rate = NumberHelper.GetRate(prayDetail.PermStar5Count, prayDetail.PermPrayTimes)
                });
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
        /// 获取群内欧气排行
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GetLuckRanking([FromForm] AuthorizeDto authorizeDto)
        {
            try
            {
                int top = 20;
                int days = 7;
                AuthorizePO authorizePO = authorizeDto.Authorize;
                LuckRankingVO luckRankingVO = memberGoodsService.getLuckRanking(authorizePO.Id, days, top);
                return ApiResult.Success(luckRankingVO);
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
        /// 定轨武器
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode"></param>
        /// <param name="goodsName"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult SetMemberAssign([FromForm] AuthorizeDto authorizeDto, string memberCode, string goodsName, string memberName = "")
        {
            try
            {
                int pondIndex = 0;
                checkNullParam(memberCode, goodsName);
                AuthorizePO authorizePO = authorizeDto.Authorize;
                GoodsPO goodsInfo = goodsService.GetGoodsByName(goodsName.Trim());
                if (goodsInfo == null) return ApiResult.GoodsNotFound;

                Dictionary<int, YSUpItem> upItemDic = goodsService.LoadArmItem(authorizePO.Id);
                YSUpItem ysUpItem = upItemDic.ContainsKey(pondIndex) ? upItemDic[pondIndex] : null;
                if (ysUpItem == null) ysUpItem = DataCache.DefaultArmItem.ContainsKey(pondIndex) ? DataCache.DefaultArmItem[pondIndex] : null;
                if (ysUpItem == null) return ApiResult.PondNotConfigured;

                if (ysUpItem.Star5UpList.Where(o => o.GoodsID == goodsInfo.Id).Any() == false) return ApiResult.AssignNotFound;
                MemberPO memberInfo = memberService.SetArmAssign(goodsInfo, authorizePO.Id, memberCode, memberName);
                return ApiResult.Success();
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
        /// 获取成员定轨信息
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GetMemberAssign([FromForm] AuthorizeDto authorizeDto, string memberCode)
        {
            try
            {
                checkNullParam(memberCode);
                AuthorizePO authorizePO = authorizeDto.Authorize;
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null || memberInfo.ArmAssignId == 0) return ApiResult.Success("未找到定轨信息");
                GoodsPO goodsInfo = goodsService.GetGoodsById(memberInfo.ArmAssignId);
                if (goodsInfo == null) return ApiResult.Success("未找到定轨信息");
                return ApiResult.Success(new
                {
                    GoodsName = goodsInfo.GoodsName,
                    GoodsType = Enum.GetName(typeof(YSGoodsType), goodsInfo.GoodsType),
                    GoodsSubType = Enum.GetName(typeof(YSGoodsSubType), goodsInfo.GoodsSubType),
                    RareType = Enum.GetName(typeof(YSRareType), goodsInfo.RareType),
                    AssignValue = memberInfo.ArmAssignValue
                });
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
        /// 获取成员出货记录
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute))]
        public ApiResult GetMemberPrayRecords([FromForm] AuthorizeDto authorizeDto, string memberCode)
        {
            try
            {
                checkNullParam(memberCode);
                AuthorizePO authorizePO = authorizeDto.Authorize;
                MemberPO memberInfo = memberService.GetByCode(authorizePO.Id, memberCode);
                if (memberInfo == null) return ApiResult.Success();
                List<PrayRecordDto> allStar5List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.五星, 20);
                List<PrayRecordDto> armStar5List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.五星, YSPondType.武器, 20);
                List<PrayRecordDto> roleStar5List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.五星, YSPondType.角色, 20);
                List<PrayRecordDto> permStar5List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.五星, YSPondType.常驻, 20);
                List<PrayRecordDto> allStar4List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.四星, 20);
                List<PrayRecordDto> armStar4List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.四星, YSPondType.武器, 20);
                List<PrayRecordDto> roleStar4List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.四星, YSPondType.角色, 20);
                List<PrayRecordDto> permStar4List = memberGoodsService.getPrayRecords(authorizePO.Id, memberCode, YSRareType.四星, YSPondType.常驻, 20);
                return ApiResult.Success(new
                {
                    star5 = new
                    {
                        arm = memberGoodsService.ChangeToPrayRecordVO(armStar5List),
                        role = memberGoodsService.ChangeToPrayRecordVO(roleStar5List),
                        perm = memberGoodsService.ChangeToPrayRecordVO(permStar5List),
                        all = memberGoodsService.ChangeToPrayRecordVO(allStar5List)
                    },
                    star4 = new
                    {
                        arm = memberGoodsService.ChangeToPrayRecordVO(armStar4List),
                        role = memberGoodsService.ChangeToPrayRecordVO(roleStar4List),
                        perm = memberGoodsService.ChangeToPrayRecordVO(permStar4List),
                        all = memberGoodsService.ChangeToPrayRecordVO(allStar4List)
                    }
                });
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
        /// 设定角色池
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="rolePond"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult SetRolePond([FromForm] AuthorizeDto authorizeDto, [FromBody] RolePondDto rolePond)
        {
            try
            {
                if (rolePond.PondIndex < 0 || rolePond.PondIndex > 10) throw new ParamException("参数错误");
                if (rolePond.UpItems == null || rolePond.UpItems.Count == 0 || rolePond.UpItems.Count > 4) throw new ParamException("参数错误");

                AuthorizePO authorizePO = authorizeDto.Authorize;
                List<GoodsPO> goodsList = new List<GoodsPO>();
                foreach (string item in rolePond.UpItems)
                {
                    string goodsName = item.Trim();
                    GoodsPO goodsInfo = goodsService.GetGoodsByName(goodsName);
                    if (goodsInfo == null) return new ApiResult(ResultCode.GoodsNotFound, $"找不到名为{goodsName}的角色");
                    goodsList.Add(goodsInfo);
                }

                List<GoodsPO> star5Goods = goodsList.Where(m => m.GoodsType == YSGoodsType.角色 && m.RareType == YSRareType.五星).ToList();
                List<GoodsPO> star4Goods = goodsList.Where(m => m.GoodsType == YSGoodsType.角色 && m.RareType == YSRareType.四星).ToList();
                if (star5Goods.Count < 1) throw new ParamException("必须指定一个五星角色");
                if (star5Goods.Count > 1) throw new ParamException("只能指定一个五星角色");
                if (star4Goods.Count < 3) throw new ParamException("必须指定三个四星角色");
                if (star4Goods.Count > 3) throw new ParamException("只能指定三个四星角色");

                goodsService.ClearPondGoods(authorizePO.Id, YSPondType.角色, rolePond.PondIndex);
                goodsService.InsertPondGoods(star5Goods, authorizePO.Id, YSPondType.角色, rolePond.PondIndex);
                goodsService.InsertPondGoods(star4Goods, authorizePO.Id, YSPondType.角色, rolePond.PondIndex);

                return ApiResult.Success();
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
        /// 设定角色池
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="armPond"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult SetArmPond([FromForm] AuthorizeDto authorizeDto, [FromBody] ArmPondDTO armPond)
        {
            try
            {
                if (armPond.UpItems == null || armPond.UpItems.Count == 0 || armPond.UpItems.Count > 7) throw new ParamException("参数错误");

                AuthorizePO authorizePO = authorizeDto.Authorize;
                List<GoodsPO> goodsList = new List<GoodsPO>();
                foreach (string item in armPond.UpItems)
                {
                    string goodsName = item.Trim();
                    GoodsPO goodsInfo = goodsService.GetGoodsByName(goodsName);
                    if (goodsInfo == null) return new ApiResult(ResultCode.GoodsNotFound, $"找不到名为{goodsName}的武器");
                    goodsList.Add(goodsInfo);
                }

                List<GoodsPO> star5Goods = goodsList.Where(m => m.GoodsType == YSGoodsType.武器 && m.RareType == YSRareType.五星).ToList();
                List<GoodsPO> star4Goods = goodsList.Where(m => m.GoodsType == YSGoodsType.武器 && m.RareType == YSRareType.四星).ToList();
                if (star5Goods.Count < 2) throw new ParamException("必须指定两个五星武器");
                if (star5Goods.Count > 2) throw new ParamException("只能指定两个五星武器");
                if (star4Goods.Count < 5) throw new ParamException("必须指定五个四星武器");
                if (star4Goods.Count > 5) throw new ParamException("只能指定五个四星武器");

                goodsService.ClearPondGoods(authorizePO.Id, YSPondType.武器, 0);
                goodsService.InsertPondGoods(star5Goods, authorizePO.Id, YSPondType.武器, 0);
                goodsService.InsertPondGoods(star4Goods, authorizePO.Id, YSPondType.武器, 0);

                return ApiResult.Success();
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
        /// 清除一个授权码配置的所有角色池
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult ResetRolePond([FromForm] AuthorizeDto authorizeDto)
        {
            try
            {
                AuthorizePO authorizePO = authorizeDto.Authorize;
                goodsService.ClearPondGoods(authorizePO.Id, YSPondType.角色);
                return ApiResult.Success();
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
        /// 清除一个授权码配置的所有武器池
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult ResetArmPond([FromForm] AuthorizeDto authorizeDto)
        {
            try
            {
                AuthorizePO authorizePO = authorizeDto.Authorize;
                goodsService.ClearPondGoods(authorizePO.Id, YSPondType.武器);
                return ApiResult.Success();
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
        /// 重置一个成员的祈愿记录
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult ResetPrayRecord([FromForm] AuthorizeDto authorizeDto, string memberCode)
        {
            try
            {
                DbScoped.SugarScope.BeginTran();
                AuthorizePO authorizePO = authorizeDto.Authorize;
                memberGoodsService.clearMemberGoods(authorizePO.Id, memberCode);
                prayRecordService.ClearPrayRecord(authorizePO.Id, memberCode);
                memberService.ResetSurplus(authorizePO.Id, memberCode);
                DbScoped.SugarScope.CommitTran();
                return ApiResult.Success();
            }
            catch (BaseException ex)
            {
                DbScoped.SugarScope.RollbackTran();
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
        /// 修改一个授权码服装出现的概率
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="rare">0~100</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PublicLimit.Yes })]
        public ApiResult SetSkinRate([FromForm] AuthorizeDto authorizeDto, int rare)
        {
            try
            {
                if (rare < 0 || rare > 100) throw new ParamException("参数错误");
                AuthorizePO authorizePO = authorizeDto.Authorize;
                goodsService.UpdateSkinRate(authorizePO.Id, rare);
                return ApiResult.Success();
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

﻿using GenshinPray.Attribute;
using GenshinPray.Common;
using GenshinPray.Exceptions;
using GenshinPray.Models;
using GenshinPray.Models.DTO;
using GenshinPray.Models.PO;
using GenshinPray.Service;
using GenshinPray.Service.PrayService;
using GenshinPray.Type;
using GenshinPray.Util;
using Microsoft.AspNetCore.Mvc;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenshinPray.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ArmPrayController : BasePrayController<ArmPrayService>
    {
        public ArmPrayController(ArmPrayService armPrayService, AuthorizeService authorizeService, MemberService memberService,
            GoodsService goodsService, PrayRecordService prayRecordService, MemberGoodsService memberGoodsService)
            : base(armPrayService, authorizeService, memberService, goodsService, prayRecordService, memberGoodsService)
        {
        }

        /// <summary>
        /// 单抽武器祈愿池
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode">成员编号(可以传入QQ号)</param>
        /// <param name="memberName"></param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PrayLimit.Yes })]
        public ApiResult PrayOne([FromForm] AuthorizeDto authorizeDto, string memberCode, string memberName = "", bool toBase64 = false, int imgWidth = 0)
        {
            try
            {
                int pondIndex = 0;
                int prayCount = 1;
                checkNullParam(memberCode);
                CheckImgWidth(imgWidth);

                YSPrayResult ySPrayResult = null;
                AuthorizePO authorizePO = authorizeDto.Authorize;
                Dictionary<int, YSUpItem> upItemDic = goodsService.LoadArmItem(authorizePO.Id);
                YSUpItem ysUpItem = upItemDic.ContainsKey(pondIndex) ? upItemDic[pondIndex] : null;
                if (ysUpItem == null) ysUpItem = DataCache.DefaultArmItem.ContainsKey(pondIndex) ? DataCache.DefaultArmItem[pondIndex] : null;
                if (ysUpItem == null) return ApiResult.PondNotConfigured;

                lock (PrayLock)
                {
                    DbScoped.SugarScope.BeginTran();
                    MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode, memberName);
                    YSGoodsItem assignGoodsItem = memberInfo.ArmAssignId == 0 || ysUpItem.Star5UpList.Where(o => o.GoodsID == memberInfo.ArmAssignId).Any() == false ? null : goodsService.GetGoodsItemById(memberInfo.ArmAssignId);
                    List<MemberGoodsDto> memberGoods = goodsService.GetMemberGoods(authorizePO.Id, memberCode);
                    ySPrayResult = basePrayService.GetPrayResult(authorizePO, memberInfo, ysUpItem, assignGoodsItem, memberGoods, prayCount);
                    memberService.UpdateMember(memberInfo);//更新保底信息
                    prayRecordService.AddPrayRecord(YSPondType.武器, authorizePO.Id, pondIndex, memberCode, prayCount);//添加调用记录
                    memberGoodsService.AddMemberGoods(ySPrayResult, memberGoods, YSPondType.武器, authorizePO.Id, memberCode);//添加成员出货记录
                    DbScoped.SugarScope.CommitTran();
                }

                ApiPrayResult prayResult = basePrayService.CreatePrayResult(ysUpItem, ySPrayResult, authorizeDto, toBase64, imgWidth);
                return ApiResult.Success(prayResult);
            }
            catch (BaseException ex)
            {
                DbScoped.SugarScope.RollbackTran();
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"authorzation：{GetAuthCode()}，memberCode：{memberCode}，toBase64：{toBase64}，imgWidth：{imgWidth}");
                DbScoped.SugarScope.RollbackTran();
                return ApiResult.ServerError;
            }
        }

        /// <summary>
        /// 十连武器祈愿池
        /// </summary>
        /// <param name="authorizeDto"></param>
        /// <param name="memberCode">成员编号(可以传入QQ号)</param>
        /// <param name="memberName"></param>
        /// <param name="toBase64"></param>
        /// <param name="imgWidth"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(AuthorizeAttribute), Arguments = new object[] { PrayLimit.Yes })]
        public ApiResult PrayTen([FromForm] AuthorizeDto authorizeDto, string memberCode, string memberName = "", bool toBase64 = false, int imgWidth = 0)
        {
            try
            {
                int pondIndex = 0;
                int prayCount = 10;
                checkNullParam(memberCode);
                CheckImgWidth(imgWidth);

                YSPrayResult ySPrayResult = null;
                AuthorizePO authorizePO = authorizeDto.Authorize;
                Dictionary<int, YSUpItem> upItemDic = goodsService.LoadArmItem(authorizePO.Id);
                YSUpItem ysUpItem = upItemDic.ContainsKey(pondIndex) ? upItemDic[pondIndex] : null;
                if (ysUpItem == null) ysUpItem = DataCache.DefaultArmItem.ContainsKey(pondIndex) ? DataCache.DefaultArmItem[pondIndex] : null;
                if (ysUpItem == null) return ApiResult.PondNotConfigured;

                lock (PrayLock)
                {
                    DbScoped.SugarScope.BeginTran();
                    MemberPO memberInfo = memberService.GetOrInsert(authorizePO.Id, memberCode, memberName);
                    YSGoodsItem assignGoodsItem = memberInfo.ArmAssignId == 0 || ysUpItem.Star5UpList.Where(o => o.GoodsID == memberInfo.ArmAssignId).Any() == false ? null : goodsService.GetGoodsItemById(memberInfo.ArmAssignId);
                    List<MemberGoodsDto> memberGoods = goodsService.GetMemberGoods(authorizePO.Id, memberCode);
                    ySPrayResult = basePrayService.GetPrayResult(authorizePO, memberInfo, ysUpItem, assignGoodsItem, memberGoods, prayCount);
                    memberService.UpdateMember(memberInfo);//更新保底信息
                    prayRecordService.AddPrayRecord(YSPondType.武器, authorizePO.Id, pondIndex, memberCode, prayCount);//添加调用记录
                    memberGoodsService.AddMemberGoods(ySPrayResult, memberGoods, YSPondType.武器, authorizePO.Id, memberCode);//添加成员出货记录
                    DbScoped.SugarScope.CommitTran();
                }

                ApiPrayResult prayResult = basePrayService.CreatePrayResult(ysUpItem, ySPrayResult, authorizeDto, toBase64, imgWidth);
                return ApiResult.Success(prayResult);
            }
            catch (BaseException ex)
            {
                DbScoped.SugarScope.RollbackTran();
                LogHelper.Info(ex);
                return ApiResult.Error(ex);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"authorzation：{GetAuthCode()}，memberCode：{memberCode}，toBase64：{toBase64}，imgWidth：{imgWidth}");
                DbScoped.SugarScope.RollbackTran();
                return ApiResult.ServerError;
            }
        }


    }
}

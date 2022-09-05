﻿using GenshinPray.Models;
using GenshinPray.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinPray.Common
{
    public static class SiteConfig
    {
        /// <summary>
        /// 物品默认概率
        /// </summary>
        public static readonly decimal GoodsDefaultPR = 1;

        /// <summary>
        /// 公用授权码
        /// </summary>
        public static string PublicAuthCode = "theresa3rd";

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConnectionString = "";

        /// <summary>
        /// 祈愿结果图片保存目录
        /// </summary>
        public static string PrayImgSavePath = "";

        /// <summary>
        /// 祈愿素材保存路径
        /// </summary>
        public static string PrayMaterialSavePath = "";

        /// <summary>
        /// 祈愿结果图片http路径
        /// </summary>
        public static string PrayImgHttpUrl = "";

        /// <summary>
        /// 排行统计缓存时间(分钟)
        /// </summary>
        public static int RankingCacheMinutes = 5;

    }
}

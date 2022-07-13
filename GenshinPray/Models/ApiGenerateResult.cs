using GenshinPray.Models.VO;
using System.Collections.Generic;

namespace GenshinPray.Models
{
    public class ApiGenerateResult
    {
        /// <summary>
        /// 祈愿次数
        /// </summary>
        public int PrayCount { get; set; }

        /// <summary>
        /// api当天剩余调用次数
        /// </summary>
        public int ApiDailyCallSurplus { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImgHttpUrl { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public long ImgSize { get; set; }

        /// <summary>
        /// 图片相对路径
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// base64
        /// </summary>
        public string ImgBase64 { get; set; }

        /// <summary>
        /// 获得的3星物品列表
        /// </summary>
        public List<GoodsVO> Star3Goods { get; set; }

        /// <summary>
        /// 获得的4星物品列表
        /// </summary>
        public List<GoodsVO> Star4Goods { get; set; }

        /// <summary>
        /// 获得的5星物品列表
        /// </summary>
        public List<GoodsVO> Star5Goods { get; set; }

    }
}

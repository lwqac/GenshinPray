using System.Collections.Generic;

namespace GenshinPray.Models.DTO
{
    public class GenerateDataDto
    {
        /// <summary>
        /// 物品列表
        /// </summary>
        public List<GoodsData> GoodsData { get; set; }

        /// <summary>
        /// uid
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 是否使用皮肤
        /// </summary>
        public bool UseSkin { get; set; }

        /// <summary>
        /// 转base64
        /// </summary>
        public bool ToBase64 { get; set; }

        /// <summary>
        /// 图片宽度0~1920
        /// </summary>
        public int ImgWidth { get; set; }
    }

    public class GoodsData
    {
        /// <summary>
        /// 物品全称
        /// </summary>
        public string GoodsName { get; set;}

        /// <summary>
        /// 排除改结果集后已拥有改物品的数量
        /// </summary>
        public int OwnedCount { get; set; }
    }



}

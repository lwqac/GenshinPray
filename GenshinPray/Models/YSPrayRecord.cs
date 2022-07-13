using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinPray.Models
{
    public class YSPrayRecord
    {
        /// <summary>
        /// 获得补给项目
        /// </summary>
        public YSGoodsItem GoodsItem { get; set; }

        /// <summary>
        /// 包括当前结果在内,当前结果以前所拥有的数量
        /// </summary>
        public int OwnedCount { get; set; }

        /// <summary>
        /// 在一次保底中消耗多少抽
        /// </summary>
        public int Cost { get; set; }

        public YSPrayRecord(YSGoodsItem goodsItem)
        {
            this.OwnedCount = 1;
            this.GoodsItem = goodsItem;
        }

        public YSPrayRecord(YSGoodsItem goodsItem, int ownedCount, int cost)
        {
            this.GoodsItem = goodsItem;
            this.OwnedCount = ownedCount;
            this.Cost = cost;
        }


    }
}

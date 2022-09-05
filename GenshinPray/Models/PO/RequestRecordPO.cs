using GenshinPray.Type;
using SqlSugar;
using System;

namespace GenshinPray.Models.PO
{
    [SugarTable("request_record")]
    public class RequestRecordPO : BasePO
    {
        [SugarColumn(IsNullable = false, ColumnDescription = "授权Id")]
        public int AuthId { get; set; }

        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "请求ip地址")]
        public string IpAddr { get; set; }

        [SugarColumn(IsNullable = false, Length = 100, ColumnDescription = "请求路径")]
        public string Path { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime CreateDate { get; set; }

        public RequestRecordPO()
        {
            this.Path = String.Empty;
            this.IpAddr = string.Empty;
            this.CreateDate = DateTime.Now;
        }

    }
}

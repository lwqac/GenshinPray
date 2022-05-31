using GenshinPray.Exceptions;
using GenshinPray.Service;
using GenshinPray.Service.PrayService;

namespace GenshinPray.Controllers.Api
{
    public abstract class BasePrayController<T> : BaseController where T : BasePrayService, new()
    {
        protected T basePrayService;
        protected AuthorizeService authorizeService;
        protected MemberService memberService;
        protected GoodsService goodsService;
        protected PrayRecordService prayRecordService;
        protected MemberGoodsService memberGoodsService;
        protected static readonly object PrayLock = new object();

        public BasePrayController(T t, AuthorizeService authorizeService, MemberService memberService,
            GoodsService goodsService, PrayRecordService prayRecordService, MemberGoodsService memberGoodsService)
        {
            this.basePrayService = t;
            this.authorizeService = authorizeService;
            this.memberService = memberService;
            this.goodsService = goodsService;
            this.prayRecordService = prayRecordService;
            this.memberGoodsService = memberGoodsService;
        }

        protected void CheckImgWidth(int imgWidth)
        {
            if (imgWidth < 0 || imgWidth > 1920) throw new ParamException("图片宽度只能设定在0~1920之间");
        }

       

    }
}

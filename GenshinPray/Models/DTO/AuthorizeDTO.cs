using GenshinPray.Models.PO;

namespace GenshinPray.Models.DTO
{
    public class AuthorizeDto
    {
        public AuthorizePO AuthorizePO { get; set; }

        public int PrayTimesToday { get; set; }

        public AuthorizeDto() { }

        public AuthorizeDto(AuthorizePO AuthorizePO, int prayTimesToday)
        {
            this.AuthorizePO = AuthorizePO;
            this.PrayTimesToday = prayTimesToday;
        }


    }
}

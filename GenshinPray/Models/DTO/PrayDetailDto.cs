using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinPray.Models.DTO
{
    public class PrayDetailDto
    {
        public int Star4Count { get; set; }

        public int Star5Count { get; set; }

        public int RoleStar4Count { get; set; }

        public int ArmStar4Count { get; set; }

        public int PermStar4Count { get; set; }

        public int RoleStar5Count { get; set; }

        public int ArmStar5Count { get; set; }

        public int PermStar5Count { get; set; }

        public int RolePrayTimes { get; set; }

        public int ArmPrayTimes { get; set; }

        public int PermPrayTimes { get; set; }

        public int TotalPrayTimes { get; set; }

    }
}

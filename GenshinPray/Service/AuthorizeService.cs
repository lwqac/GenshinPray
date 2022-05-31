using GenshinPray.Dao;
using GenshinPray.Models.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinPray.Service
{
    public class AuthorizeService : BaseService
    {
        private AuthorizeDao authorizeDao;

        public AuthorizeService(AuthorizeDao authorizeDao)
        {
            this.authorizeDao = authorizeDao;
        }

        public AuthorizePO GetAuthorize(string code)
        {
            return authorizeDao.GetAuthorize(code);
        }



    }
}

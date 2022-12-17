using ADMIN.Models.CustomModels.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMIN.Services.IdentityService
{
    public interface IIdentityService
    {
        Task<JWTModel> GetTokensByRefresh();
    }
}

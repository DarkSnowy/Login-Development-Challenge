using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using UserManagement.Database.Models;

namespace UserManagement
{
    public interface IAuthService
    {
        //AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<ApplicationUser > GetAll();
        ApplicationUser  GetById(int id);
    }
}

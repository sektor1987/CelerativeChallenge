using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sat.Recruitment.Business.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sat.Recruitment.Business;
using Sat.Recruitment.Entities;

namespace Sat.Recruitment.Api.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public partial class UsersController : ControllerBase
    {

        private readonly IUserBsStrategy userBsStrategy;

        public UsersController(IUserBsStrategy userBsStrategy)
        {
            if (userBsStrategy == null)
                throw new ArgumentNullException("userBsStrategy");

            this.userBsStrategy = userBsStrategy;

        }

        [HttpPost]
        [Route("/create-user")]
        public async Task<Result> CreateUser(string name, string email, string address, string phone, string userType, string money)
        {
            var errors = "";
            Result _result = new Result();
            this.userBsStrategy.ValidateErrors(name, email, address, phone, ref errors);
            _result =  this.userBsStrategy.Calculate(name,  email,  address,  phone,  userType,  money, errors);

            return _result;
        }


    }

}

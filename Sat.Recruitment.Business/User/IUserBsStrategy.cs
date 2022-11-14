using Sat.Recruitment.Business.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sat.Recruitment.Business
{
    public interface IUserBsStrategy
    {
        public void ValidateErrors(string name, string email, string address, string phone, ref string errors);
        public Result Calculate(string name, string email, string address, string phone, string userType, string money, string errors);
    }
}

using System;
using CoreCommandLine.Example.Di.Contracts;

namespace CoreCommandLine.Example.Di.Services
{
    public class AddDateService:IAddDateService
    {
        public string AddDate(string value)
        {
            return value + DateTime.Now.ToString("yy-MM-dd");
        }
    }
}
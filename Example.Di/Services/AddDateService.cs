using System;
using Example.Di.Contracts;

namespace Example.Di.Services
{
    public class AddDateService:IAddDateService
    {
        public string AddDate(string value)
        {
            return value + DateTime.Now.ToString("yy-MM-dd");
        }
    }
}
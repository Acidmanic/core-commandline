using System;
using Example.DotnetDi.Contracts;

namespace Example.DotnetDi.Services
{
    public class AddDateService : IAddDateService
    {
        public string AddDate(string value)
        {
            return value + DateTime.Now.ToString("yy-MM-dd");
        }
    }
}
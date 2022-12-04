using CoreCommandLine.Example.Di.Contracts;

namespace CoreCommandLine.Example.Di.Services
{
    public class UpperCaseService:IUpperCaseService
    {
        public string ToUpper(string value)
        {
            return value?.ToUpper();
        }
    }
}
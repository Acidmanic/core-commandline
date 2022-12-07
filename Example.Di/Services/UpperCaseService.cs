using Example.Di.Contracts;

namespace Example.Di.Services
{
    public class UpperCaseService:IUpperCaseService
    {
        public string ToUpper(string value)
        {
            return value?.ToUpper();
        }
    }
}
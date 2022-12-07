using Example.DotnetDi.Contracts;

namespace Example.DotnetDi.Services
{
    public class UpperCaseService : IUpperCaseService
    {
        public string ToUpper(string value)
        {
            return value?.ToUpper();
        }
    }
}
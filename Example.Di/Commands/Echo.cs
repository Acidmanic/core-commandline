using CoreCommandLine;
using CoreCommandLine.Attributes;
using Example.Di.Contracts;
using Microsoft.Extensions.Logging;

namespace Example.Di.Commands
{
    
    
    [RootCommand]
    [CommandName("echo","-e")]
    public class Echo:CommandBase
    {

        private readonly IUpperCaseService _upperCaseService;
        private readonly IAddDateService _addDateService;

        public Echo(IUpperCaseService upperCaseService, IAddDateService addDateService)
        {
            _upperCaseService = upperCaseService;
            _addDateService = addDateService;
        }


        public override bool Execute(Context context, string[] args)
        {
            var index = IndexOf(NameBundle, args);

            if (index > -1)
            {
                var echoString = "";

                for (int i = index + 1; i < args.Length; i++)
                {
                    echoString += args[i] + " ";
                }

                var upper = _upperCaseService.ToUpper(echoString);
                var dated = _addDateService.AddDate(echoString);
                
                Logger.LogInformation("Original: {Value}",echoString);
                Logger.LogInformation("UpperCase: {Value}",upper);
                Logger.LogInformation("WithDate: {Value}",dated);
            }
            // I was not present
            return false;
        }
    }
}
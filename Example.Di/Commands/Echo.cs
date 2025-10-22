using CoreCommandLine;
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using Example.Di.Contracts;
using Microsoft.Extensions.Logging;

namespace Example.Di.Commands
{
    [RootCommand]
    [CommandName("echo", "-e")]
    public class Echo : ArgumentRangeCommandBase
    {
        private readonly IUpperCaseService _upperCaseService;
        private readonly IAddDateService _addDateService;

        public Echo(IUpperCaseService upperCaseService, IAddDateService addDateService)
        {
            _upperCaseService = upperCaseService;
            _addDateService = addDateService;
        }
        
        protected override void DoExecute(Context context, string[] args)
        {
            var echoString = string.Join(' ',args);

            var upper = _upperCaseService.ToUpper(echoString);
            var dated = _addDateService.AddDate(echoString);

            Logger.LogInformation("Original: {Value}", echoString);
            Logger.LogInformation("UpperCase: {Value}", upper);
            Logger.LogInformation("WithDate: {Value}", dated);

        }
    }
}
namespace CoreCommandLine.Dtos;

public sealed record ExecutionActionAssets(Context Context, string[] Args, ICommand Command, CommandLineApplication Application);
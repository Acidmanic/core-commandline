namespace CoreCommandLine.DotnetDi
{
    public interface IMethodInvoker
    {
        void InvokeMatchingMethods(params object[] arguments);

        void InvokeMatchingMethods<TArgument>(TArgument firstArgument);
        void InvokeMatchingMethods<TArg1, TArg2>(TArg1 arg1, TArg2 arg2);
        void InvokeMatchingMethods<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3);
        void InvokeMatchingMethods<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
        void InvokeSatisfiedMethods(params object[] arguments);
        void InvokeSatisfiedMethods<TArgument>(TArgument firstArgument);
        void InvokeSatisfiedMethods<TArg1, TArg2>(TArg1 arg1, TArg2 arg2);

        void InvokeSatisfiedMethods<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3);
        void InvokeSatisfiedMethods<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

        public object Target { get; }
    }
}
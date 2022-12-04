using System;

namespace CoreCommandLine.Di
{
    public interface IResolver
    {

        object Resolve(Type type);
    }
}
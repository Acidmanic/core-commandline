using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;

namespace CoreCommandLine.DotnetDi
{
    internal class MethodInvoker : IMethodInvoker
    {
        protected readonly Type TargetType;
        protected readonly object TargetObject;
        protected readonly string TargetName;
        protected readonly List<Func<MethodInfo, bool>> _filters;

        public MethodInvoker(Type targetType)
        {
            TargetType = targetType;

            TargetName = TargetType.Name;

            TargetObject = InstantiateStartupClass();

            _filters = new List<Func<MethodInfo, bool>>();

            _filters.Add(m => true);
        }

        protected object InstantiateStartupClass()
        {
            var startupObject = new ObjectInstantiator().BlindInstantiate(TargetType);

            return startupObject ?? throw new ParametricConstructorException(TargetName);
        }

        public object Target => TargetObject;


        private Type[] CorrespondingTypes(object[] values)
        {
            if (values == null)
            {
                return new Type[] { };
            }

            var types = new Type[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                Type type = values[i] == null ? null : values[i].GetType();

                types[i] = type;
            }

            return types;
        }

        /// <summary>
        /// Adding filters allow you pass only methods you want to be executed.
        /// </summary>
        /// <param name="filter">A filter delegate, returns true when it accepts the method. and false otherwise.</param>
        public void AddFilter(Func<MethodInfo, bool> filter)
        {
            _filters.Add(filter);
        }

        public void InvokeMatchingMethods(params object[] arguments)
        {
            var types = CorrespondingTypes(arguments);

            var consumers = FindConsumers(types, Matches);

            consumers.ForEach(m => InvokeFeedDirectly(m, TargetObject, arguments));
        }

        public void InvokeMatchingMethods<TArgument>(TArgument firstArgument)
        {
            var types = new Type[] { typeof(TArgument) };

            var consumers = FindConsumers(types, Matches);

            consumers.ForEach(m => InvokeFeedDirectly(m, TargetObject, new object[] { firstArgument }));
        }

        public void InvokeMatchingMethods<TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
        {
            var types = new Type[] { typeof(TArg1), typeof(TArg2) };

            var consumers = FindConsumers(types, Matches);

            consumers.ForEach(m => InvokeFeedDirectly(m, TargetObject, new object[] { arg1, arg2 }));
        }

        public void InvokeMatchingMethods<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var types = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };

            var consumers = FindConsumers(types, Matches);

            consumers.ForEach(m => InvokeFeedDirectly(m, TargetObject, new object[] { arg1, arg2, arg3 }));
        }

        public void InvokeMatchingMethods<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            var types = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };

            var consumers = FindConsumers(types, Matches);

            consumers.ForEach(m => InvokeFeedDirectly(m, TargetObject, new object[] { arg1, arg2, arg3, arg4 }));
        }

        public void InvokeSatisfiedMethods(params object[] arguments)
        {
            var types = CorrespondingTypes(arguments);

            var consumers = FindConsumers(types, Satisfies);

            consumers.ForEach(m => InvokeFeedAvailable(m, TargetObject, arguments));
        }

        public void InvokeSatisfiedMethods<TArgument>(TArgument firstArgument)
        {
            var types = new Type[] { typeof(TArgument) };

            var consumers = FindConsumers(types, Satisfies);

            consumers.ForEach(m => InvokeFeedAvailable(m, TargetObject, new object[] { firstArgument }));
        }

        public void InvokeSatisfiedMethods<TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
        {
            var types = new Type[] { typeof(TArg1), typeof(TArg2) };

            var consumers = FindConsumers(types, Satisfies);

            consumers.ForEach(m => InvokeFeedAvailable(m, TargetObject, new object[] { arg1, arg2 }));
        }

        public void InvokeSatisfiedMethods<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var types = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };

            var consumers = FindConsumers(types, Satisfies);

            consumers.ForEach(m => InvokeFeedAvailable(m, TargetObject, new object[] { arg1, arg2, arg3 }));
        }

        public void InvokeSatisfiedMethods<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            var types = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };

            var consumers = FindConsumers(types, Satisfies);

            consumers.ForEach(m => InvokeFeedAvailable(m, TargetObject, new object[] { arg1, arg2, arg3, arg4 }));
        }


        private bool PassesTheFilters(MethodInfo methodInfo)
        {

            foreach (var filterPass in _filters)
            {
                if (!filterPass(methodInfo))
                {
                    return false;
                }
            }

            return true;
        }
        
        private List<MethodInfo> FindConsumers(Type[] types, Func<ParameterInfo[], Type[], bool> selector)
        {
            var methods = TargetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                                BindingFlags.FlattenHierarchy);

            var consumers = new List<MethodInfo>();

            foreach (var method in methods)
            {
                if (!method.IsAbstract && !method.IsStatic && !method.IsGenericMethod)
                {
                    if (PassesTheFilters(method))
                    {
                        var parameters = method.GetParameters();

                        if (selector(parameters, types))
                        {
                            consumers.Add(method);
                        }   
                    }
                }
            }

            return consumers;
        }

        private bool Matches(ParameterInfo[] parameters, Type[] types)
        {
            if (parameters == null && types == null)
            {
                return true;
            }

            if (parameters == null || types == null)
            {
                return false;
            }

            if (parameters.Length != types.Length)
            {
                return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if ((types[i] != null && parameters[i].ParameterType != types[i]))
                {
                    return false;
                }
            }

            return false;
        }

        // Signatures with repeated types can not be satisfied because the order is ignored int this approach and 
        // there is would be no logical way to find the correct value for each parameter when values are not
        // distinguished by their types
        private bool Satisfies(ParameterInfo[] parameters, Type[] types)
        {
            if (parameters == null)
            {
                return true;
            }

            if (types == null)
            {
                return false;
            }

            // There are repeated types
            if (types.Length > CountTypes(types))
            {
                return false;
            }

            // there is at least one parameter with no value for it. 
            if (parameters.Length > types.Length)
            {
                return false;
            }

            foreach (var parameter in parameters)
            {
                if (!types.Contains(parameter.ParameterType))
                {
                    return false;
                }
            }

            return true;
        }


        private int CountTypes(IEnumerable<Type> types)
        {
            int count = 0;

            HashSet<Type> counted = new HashSet<Type>();

            foreach (var type in types)
            {
                if (!counted.Contains(type))
                {
                    counted.Add(type);
                    count++;
                }
            }

            return count;
        }

        private void InvokeFeedDirectly(MethodInfo method, object owner, object[] parameterValues)
        {
            method.Invoke(owner, parameterValues);
        }

        private void InvokeFeedAvailable(MethodInfo method, object owner, object[] parameterValues)
        {
            var parameters = method.GetParameters();

            var values = new object[parameters.Length];

            for (int i = 0; i < values.Length; i++)
            {
                var parameter = parameters[i];
                // This will ignore checking the null parameter value (if any)
                // which makes it's corresponding slot remained un set, which would be null!  
                values[i] = parameterValues.Single(v => AssignableForSure(v, parameter));
            }

            method.Invoke(owner, values);
        }

        private bool AssignableForSure(object o, ParameterInfo parameter)
        {
            if (o == null)
            {
                return false;
            }

            return parameter.ParameterType.IsAssignableFrom(o.GetType());
        }
    }

    internal class MethodInvoker<T> : MethodInvoker where T : class
    {
        public MethodInvoker() : base(typeof(T))
        {
        }

        public T TargetAs()
        {
            return Target as T;
        }
    }
}
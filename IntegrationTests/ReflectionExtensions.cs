using System;
using System.Reflection;

namespace IntegrationTests
{
    public static class ReflectionExtensions
    {
        public static T InvokePrivateMethod<T>(this object obj, string methodName, params object[] parameters)
        {
            Type type = obj.GetType();
            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)
            ?? type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            return methodInfo == null
            ? throw new ArgumentException($"Private method '{methodName}' not found in type '{type.Name}'")
            : (T)methodInfo.Invoke(obj, parameters);
        }
    }

}
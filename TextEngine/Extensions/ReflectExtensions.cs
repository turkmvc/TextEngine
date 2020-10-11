using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TextEngine.Misc;
using TextEngine.Extensions;

namespace TextEngine.Extensions
{
    public static class ReflectExtensions
    {
        public static MethodInfo GetMethodByNameWithParams(this Type type, string name, object[] parameters)
        {
            if (type == null) return null;
            var methods = type.GetMethods().Where((mi) => mi.Name == name).OrderByDescending((m => m.GetParameters().Length));
            if (parameters == null) parameters = new object[0];
            foreach (var method in methods)
            {
                bool matches = true;
                var methodParams = method.GetParameters();
                if (methodParams.Length == 0 &&( parameters.Length == 0 || (parameters.Length == 1 && parameters[0] == null))) return method;
                if (methodParams.Length == 0 && parameters.Length > 0) continue;
                for (int i = 0; i < methodParams.Length; i++)
                {
                    var curParam = methodParams[i];
                    if (i + 1 > parameters.Length)
                    {
                        if(!curParam.IsOptional)
                        {
                            matches = false;
                            break;
                        }
                        break;
                    }
                    Type targetType = null;
                    if (i < parameters.Length) targetType = parameters[i]?.GetType();
                    if(targetType == null)
                    {
                        if(curParam.ParameterType.IsPrimitive)
                        {
                            matches = false;
                            break;
                        }

                    }
                    else if (curParam.ParameterType != targetType && !targetType.IsInstanceOfType(curParam.ParameterType) &&  curParam.ParameterType != typeof(string))
                    {
                       
                        if(targetType == typeof(string) && TypeUtil.IsNumericType(curParam.ParameterType))
                        {
                            if(!((string)parameters[i]).IsNumeric())
                            {
                                matches = false;
                                break;
                            }
                        }

                        else if(!TypeUtil.IsNumericType(curParam.ParameterType) || !TypeUtil.IsNumericType(targetType) )
                        {
                            matches = false;
                            break;
                        }
                       
                    }
                    

                }
                if (matches) return method;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TextEngine.Misc
{
    public class ParamUtil
    {
        public static object[] MatchParams(object[] @params, ParameterInfo[] method_Params)
        {
            List<object> convertedParams = new List<object>();
            for (int i = 0; i < method_Params.Length; i++)
            {
                var cparam = method_Params[i];
                var callingParam = @params.ElementAtOrDefault(i);
                if (callingParam != null)
                {
                    if (cparam.ParameterType == typeof(object) || callingParam.GetType() == cparam.ParameterType)
                    {
                        convertedParams.Add(callingParam);
                    }
                    else if (convertedParams.GetType() == typeof(string))
                    {
                        convertedParams.Add(callingParam.ToString());
                    }
                    else if(cparam.ParameterType == typeof(char))
                    {
                        char c = '\0';
                        if(TypeUtil.IsNumericType(callingParam))
                        {
                            uint d = (uint) Convert.ChangeType(callingParam, TypeCode.UInt32);
                            if (d < 65536)
                            {
                                c = (char)d;
                            }
                            else
                            {
                                c = d.ToString()[0];
                            }
                        }
                        else
                        {
                            var str = callingParam.ToString();
                            if (str.Length > 0)
                            {
                                c = str[0];
                            }
                        }

                        convertedParams.Add(c);
                    }
                    else
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(cparam.ParameterType);
                        bool cf = converter.CanConvertFrom(cparam.ParameterType);
                        try
                        {
                            if (converter.CanConvertTo(cparam.ParameterType))
                            {

                                convertedParams.Add(converter.ConvertTo(callingParam, cparam.ParameterType));
                            }
                            else
                            {
                                convertedParams.Add(null);
                            }
                        }
                        catch (Exception)
                        {

                            convertedParams.Add(null);
                        }

                    }
                }
                else
                {
                    convertedParams.Add(null);
                }


            }
            return convertedParams.ToArray();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using TextEngine.Extensions;
using TextEngine.Misc;

namespace TextEngine.ParDecoder
{

    public class ComputeActions
    {
        public static List<string> PriotiryStop = new List<string>()
        {
            "and",
            "&&",
            "||",
            "|",
            "==",
            "=",
            ">",
            "<",
            ">=",
            "!=",
            "<=",
            "or",
            "+",
            "-",
            ",",
            "=>",
            "?",
            ":"
        };
        public static object OperatorResult(object item1, object item2, string @operator)
        {
            if (item1 == null && item2 == null)
            {
                return null;
            }
            if ((@operator == "+" || @operator == "-") && item1 == null && item2 != null && TypeUtil.IsNumericType(item2))
            {
                var leftitem = 0d;
                var rightitem = (double)Convert.ChangeType(item2, typeof(double));
                if (@operator == "+")
                {
                    return leftitem + rightitem;
                }
                else
                {
                    return leftitem - rightitem;
                }
            }
            if (@operator == "||" || @operator == "|" || @operator == "or" || @operator == "&&" || @operator == "&" || @operator == "and")
            {
                bool lefstate = PhpFuctions.not_empty(item1);
                bool rightstate = PhpFuctions.not_empty(item2);
                if (@operator == "||" || @operator == "|" || @operator == "or")
                {
                    if (lefstate != rightstate)
                    {
                        return true;
                    }
                    return lefstate;
                }
                else
                {
                    if (lefstate && rightstate)
                    {
                        return true;
                    }
                    return false;
                }
            }
            if ((item1 is string && item2 == null) || item2 is string && item1 == null)
            {
                return false;
            }

            if (@operator == "==" || @operator == "=" || @operator == "!=" && (item1 == null || item2 == null))
            {
                if (item1 == null)
                {
                    if (@operator == "==" || @operator == "=")
                    {
                        return item2 == null;
                    }
                    else

                    {
                        return item2 != null;
                    }
                }
                else if (item2 == null)
                {
                    if (@operator == "==" || @operator == "=")
                    {
                        return item1 == null;
                    }
                    else

                    {
                        return item1 != null;
                    }
                }
            }
            if (item1 is string && @operator == "+")
            {
                return item1.ToString() + item2;
            }
            else if (item2 is string && @operator == "+")
            {
                return item1 + item2.ToString();
            }
            if (item1 is bool && item2 is bool)
            {
                var leftitem = (bool)item1;
                var rightitem = (bool)item2;
                switch (@operator)
                {
                    case "==":
                    case "=":
                        return leftitem == rightitem;
                    case "!=":
                        return leftitem != rightitem;
                }
            }
            else if ((item1 is DateTime && TypeUtil.IsNumericType(item2)) || (item2 is DateTime && TypeUtil.IsNumericType(item1)))
            {

                if (item1 is DateTime)
                {
                    var leftitem = (DateTime)item1;
                    var rightitem = (double)item2;
                    switch (@operator)
                    {
                        case "+":

                            return leftitem.AddDays(rightitem);
                        case "-":
                            return leftitem.AddDays(-1d * rightitem);
                    }
                }
                else
                {
                    var leftitem = (double)item1;
                    var rightitem = (DateTime)item2;
                    switch (@operator)
                    {
                        case "+":

                            return rightitem.AddDays(leftitem);
                        case "-":
                            return rightitem.AddDays(-1d * leftitem);
                    }
                }
            }
            else if (item1 is DateTime && item2 is DateTime)
            {
                var leftitem = (DateTime)item1;
                var rightitem = (DateTime)item2;
                int cmpres = leftitem.CompareTo(rightitem);
                switch (@operator)
                {
                    case "==":
                    case "=":
                        return cmpres == 0;
                    case "!=":
                        return cmpres != 0;
                    case "<":

                        return cmpres == -1;
                    case "<=":
                        return cmpres == 0 || cmpres == -1;
                    case ">":
                        return cmpres == 1;
                    case ">=":
                        return cmpres == 0 || cmpres == 1;
                    default:
                        break;
                }
            }
            else if (item1 is string && item2 is string)
            {
                var leftitem = (string)item1;
                var rightitem = (string)item2;
                int cmpres = leftitem.CompareTo(rightitem);
                switch (@operator)
                {
                    case "==":
                    case "=":
                        return cmpres == 0;
                    case "!=":
                        return cmpres != 0;
                    case "<":

                        return cmpres == -1;
                    case "<=":
                        return cmpres == 0 || cmpres == -1;
                    case ">":
                        return cmpres == 1;
                    case ">=":
                        return cmpres == 0 || cmpres == 1;
                    default:
                        break;
                }
            }

            else
            {
                if (item2 == null) item2 = "";
                if (item1 == null) item1 = "";
                if (TypeUtil.IsNumericType(item1) && TypeUtil.IsNumericType(item2) || TypeUtil.IsNumericType(item1) && item2.ToString().IsNumeric() || TypeUtil.IsNumericType(item2) && item1.ToString().IsNumeric())
                {
                    var leftitem = (double)Convert.ChangeType(item1, typeof(double));
                    var rightitem = (double)Convert.ChangeType(item2, typeof(double));
                    switch (@operator)
                    {
                        case "==":
                        case "=":
                            return leftitem == rightitem;
                        case "!=":
                            return leftitem != rightitem;
                        case "<":
                            return leftitem < rightitem;
                        case "<=":
                            return leftitem <= rightitem;
                        case ">":
                            return leftitem > rightitem;
                        case ">=":
                            return leftitem >= rightitem;
                        case "+":
                            return leftitem + rightitem;
                        case "-":
                            return leftitem - rightitem;
                        case "*":
                            return leftitem * rightitem;
                        case "/":
                        case "div":
                            return leftitem / rightitem;
                        case "%":
                        case "mod":
                            return leftitem % rightitem;
                        case "^":
                            return Math.Pow(leftitem, rightitem);
                        case "&":
                            return (long)leftitem & (long)rightitem;
                        case "|":
                            return (long)leftitem | (long)rightitem;
                        default:
                            break;
                    }
                }
            }
            return null;
        }

        public static object CallMethodSingle(object @object, string name, object[] @params)
        {
            if (@object == null) return null;
            Type obj_type = @object.GetType();

            //var method = obj_type.GetMethod(name);
            var method = obj_type.GetMethodByNameWithParams(name, @params);
            return CallMethodDirect(@object, method, @params);

        }
        public static object CallMethodDirect(object @object, MethodInfo method, object[] @params)
        {
            if (method == null) return null;
            var convertedParams = ParamUtil.MatchParams(@params, method.GetParameters());
            if (method != null)
            {
                if (method.IsPublic)
                {
                    return method.Invoke(@object, convertedParams.ToArray());
                }
            }
            return null;
        }
        /**  @param $item InnerItem */
        public static object CallMethod(string name, object[] @params, object vars, object localvars = null)
        {
            int dpos = name.IndexOf("::");
            if (dpos >= 0)
            {
                var clsname = name.Substring(0, dpos);
                var method = name.Substring(dpos + 2);
                if (ParItem.GlobalFunctions.Contains(clsname + "::") || ParItem.GlobalFunctions.Contains(name))
                {
                    var clsttype = ParItem.StaticTypes[clsname];
                    if (clsttype != null)
                    {
                        var cm = clsttype.GetType().GetMethod(method);
                        if (cm != null)
                        {
                            return cm.Invoke(null, @params);
                        }
                    }
                }

            }
            return CallMethodSingle(vars, name, @params);
        }

        public static object GetPropValue(InnerItem item, object vars, object localvars = null)
        {
            object res = null;
            var name = item.Value.ToString();
            if (localvars != null)
            {
                res = GetProp(name, localvars);
            }
            if (res == null)
            {
                res = GetProp(name, vars);
            }
            return res;
        }
        public static object GetProp(string item, object vars)
        {
            if (vars == null) return null;
            if (vars is KeyValueGroup il)
            {
                for (int i = il.Count - 1; i >= 0; i--)
                {
                    if (il[i] is KeyValues<object> kv)
                    {
                        var m = GetProp(item, kv);
                        if (m != null) return m;
                    }
                }
                return null;
            }
            if (vars is IDictionary<string, object> dict)
            {
                if (dict.TryGetValue(item, out object nobj))
                {
                    return nobj;
                }
                return null;
            }
            if (vars is KeyValues<object> obj)
            {
                return obj[item];
            }
            else
            {
                var vtype = vars.GetType();
                var prop = vtype.GetProperty(item);
                if (prop != null)
                {
                    var v = prop.GetValue(vars);
                    return prop.GetValue(vars);
                }
            }

            return null;
        }
        public static bool IsObjectOrArray(object item)
        {
            return item != null && item is ExpandoObject || item is Dictionary<string, object>;
        }

    }
}

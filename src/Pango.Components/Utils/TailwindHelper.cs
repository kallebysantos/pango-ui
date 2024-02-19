using System.Collections;

namespace Pango.Components.Utils;

public class TailwindHelper
{
    private static string ToVal(object mix)
    {
        var str = "";

        if (mix is string || mix is int)
        {
            str += mix;
        }
        else if (mix is IEnumerable mixArray)
        {
            foreach (var item in mixArray)
            {
                if (item != null)
                {
                    var y = ToVal(item);
                    if (!string.IsNullOrEmpty(y))
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            str += " ";
                        }
                        str += y;
                    }
                }
            }
        }
        else if (mix is IDictionary mixDict)
        {
            foreach (var key in mixDict.Keys)
            {
                var value = mixDict[key];
                if (value != null)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        str += " ";
                    }
                    str += key;
                }
            }
        }

        return str;
    }

    public static string Tw(params object[] arguments)
    {
        var i = 0;
        var str = "";

        while (i < arguments.Length)
        {
            var tmp = arguments[i++];
            if (tmp != null)
            {
                var x = ToVal(tmp);
                if (!string.IsNullOrEmpty(x))
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        str += " ";
                    }
                    str += x;
                }
            }
        }

        return str;
    }
}

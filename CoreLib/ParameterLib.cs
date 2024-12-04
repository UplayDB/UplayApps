using System.ComponentModel;

namespace CoreLib
{
    public class ParameterLib
    {
        public static int IndexOfParam(string[] args, string param)
        {
            for (var x = 0; x < args.Length; ++x)
            {
                if (args[x].Equals(param, StringComparison.OrdinalIgnoreCase))
                    return x;
            }

            return -1;
        }
        public static bool HasParameter(string[] args, string param)
        {
            return IndexOfParam(args, param) > -1;
        }

        public static T GetParameter<T>(string[] args, string param, T defaultValue)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (string.IsNullOrEmpty(param))
            {
                throw new ArgumentException($"'{nameof(param)}' cannot be null or empty.", nameof(param));
            }

            var index = IndexOfParam(args, param);

            if (index == -1 || index == (args.Length - 1))
                return defaultValue;

            var strParam = args[index + 1];

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                T? obj = (T?)converter.ConvertFromString(strParam);
                return obj == null ? defaultValue : obj;
            }

            return defaultValue;
        }
    }
}

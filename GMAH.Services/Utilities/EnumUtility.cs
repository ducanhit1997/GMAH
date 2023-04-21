using System;
using System.Linq;
using System.Reflection;

namespace GMAH.Services.Utilities
{
    public static class EnumUtility
    {
        /// <summary>
        /// Lấy display name của enum
        /// https://stackoverflow.com/questions/13099834/how-to-get-the-display-name-attribute-of-an-enum-member-via-mvc-razor-code
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
        where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }
    }
}

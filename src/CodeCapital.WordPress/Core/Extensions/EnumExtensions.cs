using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CodeCapital.WordPress.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumerationValue)
        {
            var type = enumerationValue.GetType();

            if (!type.GetTypeInfo().IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }

            var memberInfo = type.GetMember(enumerationValue.ToString());

            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false).ToArray();

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return enumerationValue.ToString();
        }

        //public static string GetDescription(this Enum value)
        //    => value
        //           .GetType()
        //           .GetMember(value.ToString())
        //           .FirstOrDefault()
        //           ?.GetCustomAttribute<DescriptionAttribute>()
        //           ?.Description
        //       ?? value.ToString();
    }
}

using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Audacia.Core.Extensions
{
    public static class MemberInfoExtensions
    {
        public static string GetDataAnnotationDisplayName(this MemberInfo memberInfo)
        {
            var displayNameAttributes = (DisplayNameAttribute[])memberInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);

            if (displayNameAttributes.Any())
            {
                return displayNameAttributes[0].DisplayName;
            }

            var descriptionAttributes = (DescriptionAttribute[])memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptionAttributes.Any())
            {
                return descriptionAttributes[0].Description;
            }

            return memberInfo.Name;
        }
    }
}
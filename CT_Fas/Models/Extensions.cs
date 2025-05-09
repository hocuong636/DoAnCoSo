using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CT_Fas.Models
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var displayAttribute = enumValue.GetType()
                .GetField(enumValue.ToString())
                .GetCustomAttributes<DisplayAttribute>(false)
                .FirstOrDefault();

            if (displayAttribute != null)
                return displayAttribute.Name;

            return enumValue.ToString();
        }
    }
}
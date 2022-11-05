using System.ComponentModel;

namespace SciMaterials.Services.Database.Extensions
{
    public static class EnumExtension
    {
        public static string ToDescriptionString(this Enum @enum)
        {
            var attributes = (DescriptionAttribute[])@enum.GetType()
                .GetField(@enum.ToString())?
                .GetCustomAttributes(typeof(DescriptionAttribute), false)!;

            return attributes?.Length > 0
                ? attributes[0].Description
                : @enum.ToString();
        }
    }
}

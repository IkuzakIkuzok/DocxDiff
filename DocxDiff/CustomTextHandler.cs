
// (c) 2023 Kazuki KOHZUKI

using System.Reflection;

namespace DocxDiff;

internal static class CustomTextHandler
{
    internal static string ToString<T>(this Enum value) where T : CustomTextAttribute
    {
        var type = value.GetType();
        var fieldInfo = type.GetField(value.ToString());
        if (fieldInfo == null) return value.ToString();

        var attrs = fieldInfo.GetCustomAttributes<T>() as T[];
        return attrs?.FirstOrDefault()?.StringValue ?? value.ToString();
    } // internal static string ToString<T>() where T : CustomTextAttribute
} // internal static class CustomTextHandler

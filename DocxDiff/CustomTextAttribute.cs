
// (c) 2023 Kazuki KOHZUKI

namespace DocxDiff;

internal abstract class CustomTextAttribute : Attribute
{
    public string StringValue { get; }

    public CustomTextAttribute(string value)
    {
        this.StringValue = value;
    } // ctor (string)
} // internal abstract class CustomTextAttribute : Attribute

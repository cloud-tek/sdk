namespace Net10Lib.StyleError;

/// <summary>
/// Proves that style rules whose severity previously rode only on the ".globalconfig"
/// shorthand suffix now fire deterministically via explicit dotnet_diagnostic lines.
/// IDE0019 (use pattern matching instead of 'as' + null check) is configured as an error,
/// so this project must fail to build.
/// </summary>
public class Class1
{
    /// <summary>
    /// Docs
    /// </summary>
    public static bool Check(object value)
    {
        var text = value as string;
        if (text != null)
        {
            return text.Length > 0;
        }

        return false;
    }
}
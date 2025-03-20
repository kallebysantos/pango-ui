namespace Pango.UI.Components;

public partial class Alert
{
    public enum Variants
    {
        Default,
        Sucess,
        Destructive,
    }

    protected static string GetStyledVariant(Variants variant) =>
        variant switch
        {
            Variants.Sucess =>
                "border-emerald-500/50 text-emerald-500 dark:border-emerald-500 [&>i]:text-emerald-500",
            Variants.Destructive =>
                "border-destructive/50 text-destructive dark:border-destructive [&>i]:text-destructive",
            // Variants.Default
            _ => "text-foreground [&>i]:text-foreground",
        };
}

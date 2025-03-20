using Microsoft.AspNetCore.Components;

namespace Pango.UI.Components;

public partial class Badge
{
    public enum Variants
    {
        Default,
        Secondary,
        Destructive,
        Outline,
    }

    protected static string GetStyledVariant(Variants variant) =>
        variant switch
        {
            Variants.Secondary =>
                "border-transparent bg-secondary text-secondary-foreground hover:bg-secondary/80",
            Variants.Outline => "text-foreground",
            Variants.Destructive =>
                "border-transparent bg-destructive text-destructive-foreground shadow hover:bg-destructive/80",
            // Variants.Default
            _ => "border-transparent bg-primary text-primary-foreground shadow hover:bg-primary/80",
        };
}

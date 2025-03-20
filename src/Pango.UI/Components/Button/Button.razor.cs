using Microsoft.AspNetCore.Components;

namespace Pango.UI.Components;

public partial class Button
{
    public enum Sizes
    {
        Default,
        Sm,
        Lg,
        Icon,
    }

    public enum Variants
    {
        Default,
        Destructive,
        Outline,
        Secondary,
        Ghost,
        Link,
    }

    protected static string GetStyledSize(Sizes size) =>
        size switch
        {
            Sizes.Sm => "h-8 rounded-md px-3 text-xs",
            Sizes.Lg => "h-10 rounded-md px-8",
            Sizes.Icon => "h-9 w-9",
            // Sizes.Default
            _ => "h-9 px-4 py-2",
        };

    protected static string GetStyledVariant(Variants variant) =>
        variant switch
        {
            Variants.Destructive =>
                "bg-destructive text-destructive-foreground shadow-sm hover:bg-destructive/90",
            Variants.Outline =>
                "border border-input bg-transparent shadow-sm hover:bg-accent hover:text-accent-foreground",
            Variants.Secondary =>
                "bg-secondary text-secondary-foreground shadow-sm hover:bg-secondary/80",
            Variants.Ghost => "hover:bg-accent hover:text-accent-foreground",
            Variants.Link => "text-primary underline-offset-4 hover:underline",
            // Variants.Default
            _ => "bg-primary text-primary-foreground shadow hover:bg-primary/90",
        };
}

using Microsoft.AspNetCore.Components;
namespace Pango.Components.UI;

public enum ButtonVariant
{
    Default,
    Destructive,
    Outline,
    Secondary,
    Ghost,
    Link
}

public enum ButtonSize
{
    Default,
    Sm,
    Lg,
    Icon,
}

public partial class Button
{
    [Parameter]
    public ButtonVariant Variant { get; set; } = ButtonVariant.Default;

    [Parameter]
    public ButtonSize Size { get; set; } = ButtonSize.Default;

    protected readonly Dictionary<ButtonSize, string> ButtonSizes =
        new()
        {
            { ButtonSize.Default, Tw("h-9 px-4 py-2") },
            { ButtonSize.Sm, Tw("h-8 rounded-md px-3 text-xs") },
            { ButtonSize.Lg, Tw("h-10 rounded-md px-8") },
            { ButtonSize.Icon, Tw("h-9 w-9") },
        };

    protected readonly Dictionary<ButtonVariant, string> ButtonVariants =
        new()
        {
            {
                ButtonVariant.Default,
                Tw("bg-primary text-primary-foreground shadow hover:bg-primary/90")
            },
            {
                ButtonVariant.Destructive,
                Tw("bg-destructive text-destructive-foreground shadow-sm hover:bg-destructive/90")
            },
            {
                ButtonVariant.Outline,
                Tw(
                    "border border-input bg-transparent shadow-sm hover:bg-accent hover:text-accent-foreground"
                )
            },
            {
                ButtonVariant.Secondary,
                Tw("bg-secondary text-secondary-foreground shadow-sm hover:bg-secondary/80")
            },
            { ButtonVariant.Ghost, Tw("hover:bg-accent hover:text-accent-foreground") },
            { ButtonVariant.Link, Tw("text-primary underline-offset-4 hover:underline") },
        };
}

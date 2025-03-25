using Microsoft.AspNetCore.Components;

namespace Pango.UI.Pages.Examples;

public partial class BadgePage : ComponentBase
{
    protected const string BadgeCodeExample =
        @"<div class=""flex flex-col gap-4 items-center"">
  <Badge>Default</Badge>

  <Badge Variant=@(Badge.Variants.Secondary)>
    Secondary
  </Badge>

  <Badge Variant=@(Badge.Variants.Outline)>
    Outline
  </Badge>

  <Badge Variant=@(Badge.Variants.Destructive)>
    Destructive
  </Badge>
</div>";
}

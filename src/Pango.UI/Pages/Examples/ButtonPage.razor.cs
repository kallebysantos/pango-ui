using Microsoft.AspNetCore.Components;

namespace Pango.UI.Pages.Examples;

public partial class ButtonPage : ComponentBase
{
    protected const string ButtonCodeExample =
        @"
<div class=""flex flex-col gap-4 items-center"">
  <Button>Default</Button>

  <Button Variant=@(Button.Variants.Secondary)>
    Secondary
  </Button>

  <Button Variant=@(Button.Variants.Outline)>
    Outline
  </Button>

  <Button Variant=@(Button.Variants.Destructive)>
    Destructive
  </Button>

  <Button Variant=@(Button.Variants.Link)>
    Link
  </Button>

  <Button Variant=@(Button.Variants.Ghost)>
    Ghost
  </Button>
</div>";
}

using Microsoft.AspNetCore.Components;

namespace Pango.UI.Pages.Examples;

public partial class CodePage : ComponentBase
{
    protected const string CodePreviewExample =
        @"<div class=""flex flex-col gap-8 items-center"">
  <h4 class=""text-sm font-medium leading-none"">
    Pango.UI
  </h4>
  <p class=""text-sm text-muted-foreground"">
    An open-source UI component library.
  </p>
</div>

@code {
  public void Greetings() => Console.WriteLine(""Hello from Pango"");
}";

    protected const string CodeExample =
        @"<div class=""flex flex-col gap-8 items-center"">
  <Code class=""[&_pre]:max-h-80"" Language=""razor"">
    @(CodeExample)
  </Code>

  <Code Language=""bash"" class=""py-2"">echo ""Hello from Pango""</Code>
</div>

@code {
  // Passing as string
  protected const string CodeExample =
@""<div class=""""flex flex-col gap-8 items-center"""">
  <h4 class=""""text-sm font-medium leading-none"""">
    Pango.UI
  </h4>
  <p class=""""text-sm text-muted-foreground"""">
    An open-source UI component library.
  </p>
</div>

@code {
  public void Greetings() => Console.WriteLine(""""Hello from Pango"""");
}"";
";
}

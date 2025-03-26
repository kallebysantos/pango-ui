using Microsoft.AspNetCore.Components;

namespace Pango.UI.Pages.Examples;

public partial class AlertPage : ComponentBase
{
    protected const string AlertCodeExample =
        @"<div class=""flex flex-col gap-8 items-center"">
  <Alert EnableClose=@(false)>
    <AlertSide>
      <i class=""ph ph-terminal text-lg""></i>
      </AlertSide>
    <AlertTitle>Default</AlertTitle>
    <AlertDescription>This alert is the Default alert variant</AlertDescription>
  </Alert>

  <Alert Variant=@(Alert.Variants.Sucess)>
    <AlertSide>
      <i class=""ph ph-check text-lg""></i>
    </AlertSide>
    <AlertTitle>Success</AlertTitle>
    <AlertDescription>This alert is using Success variant</AlertDescription>
  </Alert>

  <Alert Variant=@(Alert.Variants.Destructive)>
    <AlertSide>
      <i class=""ph ph-warning-circle text-lg""></i>
    </AlertSide>
    <AlertTitle>Destructive</AlertTitle>
    <AlertDescription>This alert is using Destructive variant</AlertDescription>
  </Alert>
</div>";
}

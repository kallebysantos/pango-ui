using Microsoft.AspNetCore.Components;

namespace Pango.UI.Pages.Examples;

public partial class InputPage : ComponentBase
{
    protected const string InputCodeExample =
        @"@if (!string.IsNullOrEmpty(Text))
{
  <p class=""text-4xl text-muted-foreground/50"">
    @Text
  </p>
}
<Input @bind-Value=@(Text) placeholder=""write something here"" />

<label class=""w-full flex flex-col gap-1"">
  <span class=""text-sm font-medium"">
    Readonly
  </span>
  <Input Value=@(Text) ReadOnly placeholder=""nothing here"" />
</label>

<div class=""w-full flex flex-col gap-3 items-end"">
  <EditForm Model=@(this) FormName=""email-example-form"" Enhance OnValidSubmit=@(HandleEmail)
    class=""w-full flex flex-col gap-2"">
    <DataAnnotationsValidator />
    <div class=""flex gap-2 flex-col md:flex-row md:items-end"">
      <label class=""flex-1 flex flex-col gap-1"">
        <span class=""text-sm font-medium"">
          Using with <strong>EditForm</strong>
        </span>
        <Input @bind-Value=@(EmailText) placeholder=""Email address"" />
        <ValidationMessage class=""md:hidden text-sm text-destructive"" For=@(() => EmailText) />
      </label>

      <Button type=""submit"">
        Submit
      </Button>
    </div>

    <ValidationMessage class=""hidden md:block text-sm text-destructive"" For=@(() => EmailText) />
  </EditForm>

  @if (hasEmailSent)
  {
    <Alert Variant=@(Alert.Variants.Sucess) OnDismiss=@(() => hasEmailSent = false)>
      <AlertSide>
        <i class=""ph ph-check text-lg""></i>
      </AlertSide>
      <AlertTitle>Sucess</AlertTitle>
      <AlertDescription>
        Activation email sent to <strong class=""font-medium"">@EmailText</strong>
      </AlertDescription>
    </Alert>
  }
</div>
@code {
  string Text = string.Empty;

  [SupplyParameterFromForm]
  [EmailAddress(ErrorMessage = ""Its not a valid email address."")]
  [Required(ErrorMessage = ""Email address is required."")]
  public string? EmailText { get; set; }

  bool hasEmailSent;

  protected void HandleEmail()
  {
    hasEmailSent = true;
  }
}";
}

using Microsoft.AspNetCore.Components;

namespace Pango.UI.Pages.Examples;

public partial class TabsPage : ComponentBase
{
    protected const string TabsCodeExample =
        @"<Tabs DefaultValue=""account"">
  <TabsList class=""grid w-full grid-cols-2"">
    <TabsTrigger Value=""account"">Account</TabsTrigger>
    <TabsTrigger Value=""password"">Password</TabsTrigger>
  </TabsList>

  <TabsContent Value=""account"" class=""p-4 rounded-md border"">
    <EditForm Model=@(this) OnValidSubmit=@(HandleAccount) FormName=""account-form"" Enhance 
    class=""flex flex-col gap-4"">
      <div class=""space-y-2"">
        <h3 class=""font-semibold leading-none tracking-tight"">
          Account
        </h3>
        <p class=""text-sm text-muted-foreground"">
          Make changes to your account here. Click save when you're done.
        </p>
      </div>

      <div class=""flex flex-col gap-2"">
        <label class=""flex flex-col gap-1"">
          <span class=""text-sm font-medium"">Name</span>
          <Input @bind-Value=""@(Fullname)"" placeholder=""Please enter your name here.""/>
        </label>

        <label class=""flex flex-col gap-1 text-sm font-medium"">
          <span class=""text-sm font-medium"">Username</span>
          <Input @bind-Value=""@(Username)"" placeholder=""Please enter your username here.""/>
        </label>
      </div>

      @if (hasAccountSaved)
      {
        <Alert Variant=@(Alert.Variants.Sucess) OnDismiss=@(() => hasAccountSaved = false)>
          <AlertSide>
            <i class=""ph ph-check text-lg""></i>
          </AlertSide>
          <AlertTitle>Sucess</AlertTitle>
          <AlertDescription>
            Your account was saved, you can now log in as <strong class=""font-medium"">@_username</strong>
          </AlertDescription>
        </Alert>
      }

      <Button>Save Changes</Button>
    </EditForm>
  </TabsContent>

  <TabsContent Value=""password"" class=""p-4 rounded-md border"">
    <EditForm Model=@(this) OnValidSubmit=@(HandlePassword) FormName=""password-form"" Enhance 
    class=""flex flex-col gap-4"">
      <DataAnnotationsValidator />

      <div class=""space-y-2"">
        <h3 class=""font-semibold leading-none tracking-tight"">
          Password
        </h3>
        <p class=""text-sm text-muted-foreground"">
          Change your password here. After saving, you'll be logged out.
        </p>
      </div>

      <div class=""flex flex-col gap-2"">
        <label class=""flex flex-col gap-1"">
          <span class=""text-sm font-medium"">Current password</span>
          <Input @bind-Value=""@(CurrentPassword)"" type=""password"" autocomplete=""current-password"" />
          <ValidationMessage class=""text-sm text-destructive"" For=@(() => CurrentPassword) />
        </label>

        <label class=""flex flex-col gap-1 text-sm font-medium"">
          <span class=""text-sm font-medium"">New password</span>
          <Input @bind-Value=""@(NewPassword)"" type=""password"" autocomplete=""new-password"" />
          <ValidationMessage class=""text-sm text-destructive"" For=@(() => NewPassword) />
        </label>
      </div>

      @if (hasPasswordSaved)
      {
        <Alert Variant=@(Alert.Variants.Sucess) OnDismiss=@(() => hasPasswordSaved = false)>
          <AlertSide>
            <i class=""ph ph-check text-lg""></i>
          </AlertSide>
          <AlertTitle>Sucess</AlertTitle>
          <AlertDescription>
            Your password was changed.
          </AlertDescription>
        </Alert>
      }

      <Button>Change password</Button>
    </EditForm>
  </TabsContent>
</Tabs>

<Separator />

<Tabs Variant=@(Tabs.Variants.Underline) DefaultValue=""password"">
  <TabsList>
    <TabsTrigger Value=""account"">
      Account
    </TabsTrigger>
    <TabsTrigger Value=""password"">
      Password
    </TabsTrigger>
  </TabsList>

  <TabsContent Value=""account"" class=""p-4 rounded border"">
    <EditForm Model=@(this) OnValidSubmit=@(HandleAccount) FormName=""account-form"" Enhance 
    class=""flex flex-col gap-4"">
      <div class=""space-y-2"">
        <h3 class=""font-semibold leading-none tracking-tight"">
          Account
        </h3>
        <p class=""text-sm text-muted-foreground"">
          Make changes to your account here. Click save when you're done.
        </p>
      </div>

      <div class=""flex flex-col gap-2"">
        <label class=""flex flex-col gap-1"">
          <span class=""text-sm font-medium"">Name</span>
          <Input @bind-Value=""@(Fullname)"" placeholder=""Please enter your name here.""/>
        </label>

        <label class=""flex flex-col gap-1 text-sm font-medium"">
          <span class=""text-sm font-medium"">Username</span>
          <Input @bind-Value=""@(Username)"" placeholder=""Please enter your username here.""/>
        </label>
      </div>

      @if (hasAccountSaved)
      {
        <Alert Variant=@(Alert.Variants.Sucess) OnDismiss=@(() => hasAccountSaved = false)>
          <AlertSide>
            <i class=""ph ph-check text-lg""></i>
          </AlertSide>
          <AlertTitle>Sucess</AlertTitle>
          <AlertDescription>
            Your account was saved, you can now log in as <strong class=""font-medium"">@_username</strong>
          </AlertDescription>
        </Alert>
      }

      <Button>Save Changes</Button>
    </EditForm>
  </TabsContent>

  <TabsContent Value=""password"" class=""p-4 rounded border"">
    <EditForm Model=@(this) OnValidSubmit=@(HandlePassword) FormName=""password-form"" Enhance 
    class=""flex flex-col gap-4"">
      <DataAnnotationsValidator />

      <div class=""space-y-2"">
        <h3 class=""font-semibold leading-none tracking-tight"">
          Password
        </h3>
        <p class=""text-sm text-muted-foreground"">
          Change your password here. After saving, you'll be logged out.
        </p>
      </div>

      <div class=""flex flex-col gap-2"">
        <label class=""flex flex-col gap-1"">
          <span class=""text-sm font-medium"">Current password</span>
          <Input @bind-Value=""@(CurrentPassword)"" type=""password"" autocomplete=""current-password"" />
          <ValidationMessage class=""text-sm text-destructive"" For=@(() => CurrentPassword) />
        </label>

        <label class=""flex flex-col gap-1 text-sm font-medium"">
          <span class=""text-sm font-medium"">New password</span>
          <Input @bind-Value=""@(NewPassword)"" type=""password"" autocomplete=""new-password"" />
          <ValidationMessage class=""text-sm text-destructive"" For=@(() => NewPassword) />
        </label>
      </div>

      @if (hasPasswordSaved)
      {
        <Alert Variant=@(Alert.Variants.Sucess) OnDismiss=@(() => hasPasswordSaved = false)>
          <AlertSide>
            <i class=""ph ph-check text-lg""></i>
          </AlertSide>
          <AlertTitle>Sucess</AlertTitle>
          <AlertDescription>
            Your password was changed.
          </AlertDescription>
        </Alert>
      }

      <Button>Change password</Button>
    </EditForm>
  </TabsContent>

@code {
  [SupplyParameterFromForm(FormName = ""account-form"")]
  public string Fullname {get; set;} = ""Kalleby Santos"";

  [SupplyParameterFromForm(FormName = ""account-form"")]
  public string Username  {get; set;} = ""@kallebysantos"";

  [SupplyParameterFromForm(FormName = ""password-form"")]
  [Required(ErrorMessage = ""You must supply your current password."")]
  public string? CurrentPassword {get; set;}

  [SupplyParameterFromForm(FormName = ""password-form"")]
  [Required(ErrorMessage = ""You must supply a new desired password."")]
  public string? NewPassword {get; set;}

  bool hasAccountSaved;
  string _username = string.Empty;
  protected void HandleAccount()
  {
      hasAccountSaved = true;
      _username = Username;
  }

  bool hasPasswordSaved;
  protected void HandlePassword()
  {
      hasPasswordSaved = true;
      CurrentPassword = string.Empty;
      NewPassword = string.Empty;
  }
}";
}

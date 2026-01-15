using Microsoft.AspNetCore.Components;

namespace Pango.UI.Components;

/// <summary>
/// Non-generic interface for Select child components to access parent state
/// </summary>
public interface ISelect
{
    string State { get; }
    string? SelectedText { get; }
    string? Placeholder { get; }
    bool Disabled { get; }
    ElementReference SelectTriggerRef { get; set; }
    ElementReference SelectContentRef { get; set; }
    Task ToggleState();
    Task OpenDropdown();
    Task CloseDropdown();
}

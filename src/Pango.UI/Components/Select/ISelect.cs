using Microsoft.AspNetCore.Components;

namespace Pango.UI.Components;

public interface ISelect
{
    public string[] SelectKeys { get; }
    internal object? Placeholder { get; }
    internal bool Disabled { get; }
    internal List<SelectItem> AllItems { get; }
    internal int FocusedIndex { get; set; }
    internal object? SelectValue { get; }
    internal SelectItem? SelectedItem { get; }
    internal List<SelectItem> SelectedItems { get; }
    internal bool Open { get; }
    internal ElementReference? SelectTriggerRef { set; }
    internal ElementReference? SelectContentRef { set; }

    internal Task OpenSelect();

    internal Task SelectItem(object? value, SelectItem item);
    internal void RegisterItem(SelectItem item);
    internal Task CloseSelect();
}

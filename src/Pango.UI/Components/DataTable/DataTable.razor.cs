namespace Pango.UI.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Rendering;

[CascadingTypeParameter("TGridItem")]
public partial class DataTable<TGridItem> : QuickGrid<TGridItem>
{
    public DataTable()
        : base()
    {
        Theme = "data-table";
    }

    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
        __builder.OpenElement(0, "div");
        __builder.AddAttribute(1, "data-table-scope-id");

        base.BuildRenderTree(__builder);

        __builder.CloseElement();
    }
}

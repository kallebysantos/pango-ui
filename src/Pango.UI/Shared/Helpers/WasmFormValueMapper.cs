using Microsoft.AspNetCore.Components.Forms.Mapping;

namespace Pango.UI.Shared.Helpers;

public class WasmFormValueMapper : IFormValueMapper
{
    public bool CanMap(Type valueType, string mappingScopeName, string formName) => false;

    public void Map(FormValueMappingContext context) { }
}

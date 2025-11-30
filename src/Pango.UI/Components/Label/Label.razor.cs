using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace Pango.UI.Components;

[CascadingTypeParameter("TValue")]
public partial class Label(TwMerge TwMerge)
{
  /// <summary>
  /// Gets or sets a collection of additional attributes that will be applied to the created element.
  /// </summary>
  [Parameter(CaptureUnmatchedValues = true)]
  public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

  /// <summary>
  /// Merge Tailwind CSS classes without style conflicts
  /// </summary>
  public string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);

  [Parameter]
  public RenderFragment? ChildContent { get; set; }

  [Parameter]
  public Expression<Func<object>>? For { get; set; }

  [Parameter]
  public string? Value { get; set; }

  string? _value;
  string? _for;

  public override async Task SetParametersAsync(ParameterView parameters)
  {
    parameters.SetParameterProperties(this);

    if (For is not null)
    {
      var exp = this.For?.Body as MemberExpression;
      var property = exp?.Expression?.Type.GetProperty(exp.Member.Name);
      var displayAttr = property?
        .GetCustomAttributes(typeof(DisplayAttribute), false)?
        .FirstOrDefault() as DisplayAttribute;

      _value = displayAttr?.GetName() ?? exp?.Member.Name ?? string.Empty;
      _for = exp?.Member.Name;
    }

    _value = Value ?? _value;
    _for = AdditionalAttributes?.GetValueOrDefault("htmlFor") as string ?? _for;


    await base.SetParametersAsync(parameters);
  }
}

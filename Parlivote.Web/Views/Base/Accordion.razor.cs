using System;
using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base;

public partial class Accordion
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    public Guid id = Guid.NewGuid();
}
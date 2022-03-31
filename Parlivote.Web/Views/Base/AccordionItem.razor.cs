using System;
using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base;

public partial class AccordionItem : ComponentBase
{
    [CascadingParameter]
    public Guid AccordionId { get; set; }

    [Parameter]
    public RenderFragment Header { get; set; }

    [Parameter]
    public RenderFragment OuterButtonHeader { get; set; }

    

    [Parameter]
    public RenderFragment Body { get; set; }

    [Parameter] public bool Collapsed { get; set; }
    [Parameter] public string Value { get; set; }

    void Toggle()
    {
        Collapsed = !Collapsed;
    }

    private Guid id;

    protected override void OnInitialized()
    {
        id = Guid.NewGuid();
    }
}
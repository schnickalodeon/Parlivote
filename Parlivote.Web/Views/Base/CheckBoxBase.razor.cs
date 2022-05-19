using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base;

public partial class CheckBoxBase : ComponentBase
{
    [Parameter]
    public string Text { get; set; }

    [Parameter]
    public bool Value { get; set; }

    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    private async Task OnChange(Microsoft.AspNetCore.Components.ChangeEventArgs args)
    {
        bool value = (bool) (args?.Value ?? false);
        this.Value = value;
        await ValueChanged.InvokeAsync(value);
    }
}
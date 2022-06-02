using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.DropDowns;

namespace Parlivote.Web.Views.Base;

public partial class DropDownBase<TEntity, TValue> : ComponentBase
{
    [Parameter]
    public TValue Value { get; set; }

    [Parameter]
    public string ValueProperty { get; set; }

    [Parameter]
    public string TextProperty { get; set; }
    
    [Parameter]
    public string Placeholder { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter]
    public bool IsDisabled { get; set; }

    [Parameter]
    public List<TEntity> Items { get; set; }

    public bool IsEnabled => IsDisabled is false;

    public async Task SetValue(TValue value)
    {
        this.Value = value;
        await ValueChanged.InvokeAsync(this.Value);
    }

    private async Task OnValueChanged(
        ChangeEventArgs<TValue, TEntity> changeEventArgs)
    {
        await SetValue(changeEventArgs.Value);
    }

    public void Disable()
    {
        this.IsDisabled = true;
        InvokeAsync(StateHasChanged);
    }

    public void Enable()
    {
        this.IsDisabled = false;
        InvokeAsync(StateHasChanged);
    }
}
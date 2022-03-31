using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Calendars;

namespace Parlivote.Web.Components.Base;

public partial class DateTimePickerBase : ComponentBase
{
    [Parameter]
    public DateTimeOffset Value { get; set; }

    [Parameter]
    public string Placeholder { get; set; }

    [Parameter]
    public EventCallback<DateTimeOffset> ValueChanged { get; set; }

    [Parameter]
    public bool IsDisabled { get; set; }

    public bool IsEnabled => IsDisabled is false;

    public async Task SetValue(DateTimeOffset value)
    {
        this.Value = value;
        await ValueChanged.InvokeAsync(this.Value);
        StateHasChanged();
    }

    public async Task OnValueChanged(ChangedEventArgs<DateTimeOffset> changeEventArgs)
    {
        await SetValue(changeEventArgs.Value);
    }
}
﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base;

public partial class TextBoxBase : ComponentBase
{
    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public string Placeholder { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public bool IsDisabled { get; set; }

    public bool IsEnabled => IsDisabled is false;

    public async Task SetValue(string value)
    {
        this.Value = value;
        await ValueChanged.InvokeAsync(this.Value);
    }

    private Task OnValueChanged(ChangeEventArgs changeEventArgs)
    {
        this.Value = changeEventArgs.Value.ToString();

        return ValueChanged.InvokeAsync(this.Value);
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
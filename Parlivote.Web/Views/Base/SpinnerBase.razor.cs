﻿using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base
{
    public partial class SpinnerBase : ComponentBase
    {
        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public string Value { get; set; }

        public void Show()
        {
            IsVisible = true;
            InvokeAsync(StateHasChanged);
        }

        public void Hide()
        {
            IsVisible = false;
            InvokeAsync(StateHasChanged);
        }

        public void SetValue(string value)
        {
            Value = value;
            InvokeAsync(StateHasChanged);
        }
    }
}
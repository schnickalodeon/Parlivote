using System;
using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Extensions;

namespace Parlivote.Web.Views.Base
{
    public partial class ButtonBase : ComponentBase
    {
        [Parameter] public string Text { get; set; }

        [Parameter] public Action OnClick { get; set; }

        [Parameter] public string CustomCss { get; set; }

        private string css;
        protected override void OnParametersSet()
        {
            this.css = CustomCss.IsNullOrWhitespace() 
                ? "btn btn-primary"
                : CustomCss;
        }
    }
}

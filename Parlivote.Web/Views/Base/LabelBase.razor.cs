using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base
{
    public partial class LabelBase : ComponentBase
    {
        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public bool IsTopLabel { get; set; }
    }
}

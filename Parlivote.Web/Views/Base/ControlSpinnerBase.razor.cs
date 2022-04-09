using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base
{
    public partial class ControlSpinnerBase : ComponentBase
    {
        [Parameter]
        public string Value { get; set; }
    }
}

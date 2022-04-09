using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base
{
    public partial class TableBase : ComponentBase
    {
        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter] 
        public RenderFragment Body { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }
    }
}

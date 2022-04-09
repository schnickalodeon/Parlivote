using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base.Grids
{
    public partial class DataGridBaseColumn : ComponentBase
    {
        [Parameter]
        public object Data { get; set; }
    }
}

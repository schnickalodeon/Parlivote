using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base.Grids
{
    public partial class DataGridBase<TDataItem> : ComponentBase
    {
        [Parameter] 
        public RenderFragment HeaderTemplate { get; set; }

        [Parameter]
        public IEnumerable<TDataItem> DataItems { get; set; }
    }
}

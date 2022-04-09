using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base.Grids
{
    public partial class CrudDataGridBase<TDataItem> : ComponentBase
    {
        [Parameter]
        public IEnumerable<TDataItem> DataItems { get; set; }

        [Parameter]
        public RenderFragment HeaderTemplate { get; set; }

        [Parameter]
        public EventCallback<TDataItem> OnDeleteClicked { get; set; }

        [Parameter]
        public EventCallback<TDataItem> OnEditClicked { get; set; }
    }
}

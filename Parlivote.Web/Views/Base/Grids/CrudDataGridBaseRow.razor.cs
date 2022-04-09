using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base.Grids
{
    public partial class CrudDataGridBaseRow<TDataItem> : ComponentBase
    {
        [Parameter]
        public TDataItem DataItem { get; set; }

        [Parameter]
        public EventCallback<TDataItem> OnEdit { get; set; }

        [Parameter]
        public EventCallback<TDataItem> OnDelete { get; set; }

        private PropertyInfo[] properties;

        protected override void OnParametersSet()
        {
            SetProperties();
        }

        private void SetProperties()
        {
            PropertyInfo[] propertyInfos = DataItem.GetType().GetProperties();
            PropertyInfo[] propertiesToShow = propertyInfos.Where(property => !property.Name.Contains("Id")).ToArray();
            this.properties = propertiesToShow;
        }

        private object GetValue(PropertyInfo property)
        {
            return property.GetValue(DataItem);
        }

        private async void EditClicked()
        {
            await this.OnEdit.InvokeAsync(DataItem);
        }

        private async Task DeleteClicked()
        {
            await OnDelete.InvokeAsync(DataItem);
        }
    }
}

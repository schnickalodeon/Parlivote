using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base.Grids
{
    public partial class DataGridBaseRow<TDataItem> : ComponentBase
    {
        [Parameter]
        public TDataItem DataItem { get; set; }

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
    }
}

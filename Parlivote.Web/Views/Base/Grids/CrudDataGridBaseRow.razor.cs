using System;
using System.Collections.Generic;
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
        public List<string> Ignore { get; set; }

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
           
            Func<PropertyInfo, bool> containsNoId = property => !property.Name.Contains("Id");

            Func<PropertyInfo, bool> isNoList = 
                property => !(property.PropertyType.IsGenericType && (property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)));

            Func<PropertyInfo, bool> filter = property => (isNoList(property) && containsNoId(property) && !Ignore.Contains(property.Name));

            PropertyInfo[] propertiesToShow = propertyInfos.Where(filter).ToArray();
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

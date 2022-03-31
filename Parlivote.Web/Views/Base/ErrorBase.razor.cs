using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base
{
    public partial class ErrorBase : ComponentBase
    {
        [Parameter] 
        public string ErrorMessage { get; set; }
    }
}

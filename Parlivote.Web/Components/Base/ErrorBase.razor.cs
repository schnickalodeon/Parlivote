using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Components.Base
{
    public partial class ErrorBase : ComponentBase
    {
        [Parameter] 
        public string ErrorMessage { get; set; }
    }
}

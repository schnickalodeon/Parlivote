using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parlivote.Web.Views.Base
{
    public partial class Success
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Visible { get; set; }
    }
}

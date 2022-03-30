using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Components.Base
{
    public partial class DialogBase : ComponentBase
    {
        private bool show;
        private string btnSubmitCss;

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string SubmitButtonText { get; set; }

        [Parameter]
        public string CancelButtonText { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback OnSubmit { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string SubmitButtonCss { get; set; }

        [Parameter]
        public string Error { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                this.btnSubmitCss = SubmitButtonCss ?? "btn btn-primary";
            }
        }

        public void Show()
        {
            this.show = true;
            StateHasChanged();
        }

        public void Hide()
        {
            this.show = false;
            StateHasChanged();
        }

        protected async Task Cancel()
        {
            Hide();
            await OnCancel.InvokeAsync();
        }

        protected async Task Submit()
        {
            await OnSubmit.InvokeAsync();
        }
    }
}
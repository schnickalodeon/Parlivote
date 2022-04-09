using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base;

public partial class ConfirmationDialogBase : ComponentBase
{
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public string SubmitButtonText { get; set; }

    [Parameter]
    public string CancelButtonText { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    [Parameter]
    public EventCallback OnSubmit { get; set; }

    [Parameter]
    public bool IsVisible { get; set; }

    [Parameter]
    public string Error { get; set; }

    public void Show()
    {
        this.IsVisible = true;
        StateHasChanged();
    }

    public void Hide()
    {
        this.IsVisible = false;
        StateHasChanged();
    }

    private async Task Cancel()
    {
        IsVisible = false;
        await OnCancel.InvokeAsync();
    }
}
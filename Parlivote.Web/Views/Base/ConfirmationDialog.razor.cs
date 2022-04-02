using Microsoft.AspNetCore.Components;

namespace Parlivote.Web.Views.Base;

public partial class ConfirmationDialog : ComponentBase
{
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public EventCallback OnDelete { get; set; }

    private DialogBase dialog;

    private void CloseDialog()
    {
        this.dialog.Hide();
    }

    public void Show()
    {
        this.dialog.Show();
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.Identity.Users;
using Parlivote.Web.Models;
using Parlivote.Web.Services.Views.Users;
using Syncfusion.Blazor.Diagrams;

namespace Parlivote.Web.Views.Components.Users;

public partial class ApplicantsDropDown : ComponentBase
{
    [Inject]
    public IUserViewService UserViewService { get; set; }

    [Parameter]
    public Guid? ApplicantId { get; set; }

    [Parameter]
    public EventCallback<Guid?> ApplicantIdChanged { get; set; }

    private Guid? BoundValue
    {
        get => this.ApplicantId;
        set => ApplicantIdChanged.InvokeAsync(value);
    }

    private ComponentState state;
    private string error;
    private List<User> applicants;

    protected override async Task OnInitializedAsync()
    {
        this.state = ComponentState.Loading;
        await LoadApplicants();
    }

    private async Task LoadApplicants()
    {
        try
        {
            this.applicants =
                await this.UserViewService.RetrieveApplicants();

            this.state = ComponentState.Content;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            this.state = ComponentState.Error;
            this.error = e.Message;
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Parlivote.Shared.Models.VoteValues;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Views.Votes;

namespace Parlivote.Web.Views.Components.Motions;

public partial class VoteComponent: ComponentBase
{
    [Inject]
    public IVoteViewService VoteViewService { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Parameter]
    public EventCallback AfterVoted { get; set; }

    [Parameter]
    public MotionView ActiveMotion { get; set; }

    private VoteValue selectedVoteValue = VoteValue.NoValue;

    private void ForClicked()
    {
        this.selectedVoteValue = VoteValue.For;
        StateHasChanged();
    }

    private void AgainstClicked()
    {
        this.selectedVoteValue = VoteValue.Against;
        StateHasChanged();
    }

    private void AbstentionClicked()
    {
        this.selectedVoteValue = VoteValue.Abstention;
        StateHasChanged();
    }

    private string GetSubmitButtonCss()
    {
        return this.selectedVoteValue switch
        {
            VoteValue.For => "btn btn-success",
            VoteValue.Against => "btn btn-danger",
            VoteValue.Abstention => "btn btn-warning",
            _ => "invisible"
        };
    }

    private async void SubmitVoteAsync()
    {
        Guid userId = await GetUserId();

        var voteView = new VoteView()
        {
            Value = this.selectedVoteValue,
            MotionId = this.ActiveMotion.MotionId,
            UserId = userId
        };

        await this.VoteViewService.AddAsync(voteView);
        await this.AfterVoted.InvokeAsync();
    }

    private async Task<Guid> GetUserId()
    {
        AuthenticationState authState =
            await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

        string userId =
            authState.User.Claims.First(claim => claim.Type == "id").Value;

        return Guid.Parse(userId);
    }
}
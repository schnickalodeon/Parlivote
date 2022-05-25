using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Parlivote.Shared.Models.VoteValues;
using Parlivote.Web.Models.Views.Motions;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Views.Base;
using Syncfusion.Blazor.DropDowns;

namespace Parlivote.Web.Views.Components.Motions;

public partial class MotionResultDialog: ComponentBase
{
    [Parameter]
    public MotionView MotionView { get; set; }

    [Parameter]
    public bool IsVisible { get; set; } = false;

    private InfoDialogBase infoDialog;
    private int yesCount = 0;
    private int noCount = 0;
    private int abstentionCount = 0;

    public void Show()
    {
        this.IsVisible = true;
        this.infoDialog.Show();
    }

    protected override void OnParametersSet()
    {
        SetVoteResults();
    }

    private void SetVoteResults()
    {
        List<VoteView> votes = MotionView.VoteViews;
        this.yesCount = votes.Count(vote => vote.Value == VoteValue.For);
        this.noCount = votes.Count(vote => vote.Value == VoteValue.Against);
        this.abstentionCount = votes.Count(vote => vote.Value == VoteValue.Abstention);
    }
}
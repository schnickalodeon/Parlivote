using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Meetings;
using Parlivote.Web.Services.Views.Meetings;
using Parlivote.Web.Services.Views.Users;

namespace Parlivote.Web.Views.Components;

public partial class AttendanceComponent : ComponentBase
{
    [Inject]
    public IUserViewService UserViewService { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private HubConnection hubConnection;
    private bool IsConnected => 
        this.hubConnection.State == HubConnectionState.Connected;

    private Guid userId;

    private bool isAttendant;
    public bool IsAttendant
    {
        get => this.isAttendant;

        set
        {
            this.isAttendant = value;
            UpdateAttendance(value);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        this.userId = await GetUserId();
        this.isAttendant =  await this.UserViewService.IsAttendant(this.userId);
        await ConnectToVoteHub();
    }

    private async Task ConnectToVoteHub()
    {
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/votehub"))
            .Build();

        await this.hubConnection.StartAsync();
    }

    private async void UpdateAttendance(bool attendance)
    {

        int attendanceCount = await this.UserViewService.UpdateAttendance(this.userId, attendance);

        if (IsConnected)
        {
            await this.hubConnection.InvokeAsync(VoteHub.AttendanceUpdatedMethod, attendanceCount);
        }
        else
        {
            throw new Exception("Not Connected");
        }
        
    }

    private async Task<Guid> GetUserId()
    {
        AuthenticationState authState =
            await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

        string strUserId =
            authState.User.Claims.First(claim => claim.Type == "id").Value;

        return Guid.Parse(strUserId);
    }
}
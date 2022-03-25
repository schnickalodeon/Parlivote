using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlivote.Shared.Models.Motions;

namespace Parlivote.Web.Brokers.API;

public partial class ApiBroker
{
    private const string MotionsRelativeUrl = "/api/v1/motions";

    public async Task<Motion> PostMotionAsync(Motion poll) =>
        await this.PostAsync(MotionsRelativeUrl, poll);

    public async Task<List<Motion>> GetAllMotionsAsync() =>
        await this.GetAsync<List<Motion>>(MotionsRelativeUrl);
    

    public async Task<Motion> GetMotionById(Guid pollId) =>
        await this.GetAsync<Motion>($"{MotionsRelativeUrl}/{pollId}");

    public async Task<Motion> PutMotionAsync(Motion poll) =>
        await this.PutAsync(MotionsRelativeUrl,poll);

    public async Task<Motion> DeleteMotionById(Guid pollId) =>
        await this.DeleteAsync<Motion>($"{MotionsRelativeUrl}/{pollId}");
}
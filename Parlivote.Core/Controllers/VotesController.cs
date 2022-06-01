using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Parlivote.Core.Services.Foundations.Votes;
using Parlivote.Shared.Models.Votes;
using Parlivote.Shared.Models.Votes.Exceptions;
using RESTFulSense.Controllers;

namespace Parlivote.Core.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class VotesController : RESTFulController
{
    private readonly IVoteService voteService;
    public VotesController(IVoteService voteService)
    {
        this.voteService = voteService;
    }

    [HttpPost]
    public async Task<ActionResult<Vote>> PostVoteAsync([FromBody] Vote vote)
    {
        try
        {
            Vote createdVote =
                await this.voteService.AddAsync(vote);

            return Created(createdVote);
        }
        catch (VoteValidationException voteValidationException)
        {
            return BadRequest(voteValidationException.InnerException);
        }
        catch (VoteDependencyValidationException voteDependencyException)
            when (voteDependencyException.InnerException is AlreadyExistsVoteException)
        {
            return Conflict(voteDependencyException.InnerException);
        }
        catch (VoteDependencyException voteDependencyException)
        {
            return InternalServerError(voteDependencyException);
        }
        catch (VoteServiceException voteServiceException)
        {
            return InternalServerError(voteServiceException);
        }
    }

    [HttpGet]
    public ActionResult<IQueryable<Vote>> GetAllVotes()
    {
        try
        {
            IQueryable<Vote> votes = this.voteService.RetrieveAll();
            return Ok(votes);
        }
        catch (VoteDependencyException voteDependencyException)
        {
            return InternalServerError(voteDependencyException);
        }
        catch (VoteServiceException voteServiceException)
        {
            return InternalServerError(voteServiceException);
        }
    }

    [HttpGet("{voteId}")]
    public async Task<ActionResult<Vote>> GetVoteByIdAsync(Guid voteId)
    {
        try
        {
            Vote activeVote =
                await this.voteService.RetrieveByIdAsync(voteId);

            return Ok(activeVote);
        }
        catch (VoteDependencyException voteDependencyException)
        {
            return InternalServerError(voteDependencyException);
        }
        catch (VoteServiceException voteServiceException)
        {
            return InternalServerError(voteServiceException);
        }
    }

    [HttpPut]
    public async Task<ActionResult<Vote>> PutVoteAsync([FromBody] Vote vote)
    {
        try
        {
            Vote modifiedVote =
                await this.voteService.ModifyAsync(vote);

            return Ok(modifiedVote);
        }
        catch (VoteValidationException voteValidationException)
            when (voteValidationException.InnerException is NotFoundVoteException)
        {
            return NotFound(voteValidationException.InnerException);
        }
        catch (VoteValidationException voteValidationException)
        {
            return BadRequest(voteValidationException.InnerException);
        }
        catch (VoteDependencyValidationException voteDependencyValidationException)
            when (voteDependencyValidationException.InnerException is AlreadyExistsVoteException)
        {
            return Conflict(voteDependencyValidationException.InnerException);
        }
        catch (VoteDependencyException voteDependencyException)
        {
            return InternalServerError(voteDependencyException);
        }
        catch (VoteServiceException voteServiceException)
        {
            return InternalServerError(voteServiceException);
        }
    }

    [HttpDelete("{voteId}")]
    public async Task<ActionResult<Vote>> DeleteVoteById(Guid voteId)
    {
        try
        {
            Vote deletedVote =
                await this.voteService.RemoveByIdAsync(voteId);

            return Ok(deletedVote);
        }
        catch (Exception e)
        {
            return InternalServerError(e);
        }
    }

    [HttpDelete("ByMotion/{motionId}")]
    public async Task<ActionResult<Vote>> DeleteVoteByMotionId(Guid motionId)
    {
        try
        {
            Vote deletedVote =
                await this.voteService.RemoveByIdMotionIdAsync(motionId);

            return Ok(deletedVote);
        }
        catch (Exception e)
        {
            return InternalServerError(e);
        }
    }

}
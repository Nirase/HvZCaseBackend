﻿using AutoMapper;
using HvZAPI.Models.DTOs.ChannelDTOs;
using HvZAPI.Models;
using HvZAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;
using HvZAPI.Exceptions;

namespace HvZAPI.Controllers
{
    [Route("api/v1/game/{gameId}/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private readonly IChannelService _channelService;
        private readonly IMapper _mapper;

        public ChannelController(IChannelService channelService, IMapper mapper)
        {
            _channelService = channelService;
            _mapper = mapper;
        }

        /// <summary>
        /// Fetches all Channels that the user has access to
        /// </summary>
        /// <returns>Enumerable of all Channels</returns>
        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<ActionResult<IEnumerable<ChannelDTO>>> GetChannels(int gameId)
        {

            var subject = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                return Ok(_mapper.Map<IEnumerable<ChannelDTO>>(await _channelService.GetChannels(gameId, subject)));
            }
            catch(PlayerNotFoundException ex)
            {
                return NotFound(new ProblemDetails { Detail = ex.Message });
            }
        }

        /// <summary>
        /// Fetches all Channel entities that a player has access to with details
        /// </summary>
        /// <returns>Detailed Channel entities</returns>
        [HttpGet("withdetails")]
        [Authorize(Roles = "user")]
        public async Task<ActionResult<IEnumerable<DetailedChannelDTO>>> GetChannelsDetailed(int gameId)
        {
            var subject = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                return Ok(_mapper.Map<IEnumerable<DetailedChannelDTO>>(await _channelService.GetChannels(gameId, subject)));
            }
            catch (PlayerNotFoundException ex)
            {
                return NotFound(new ProblemDetails { Detail = ex.Message });
            }
        }
        /// <summary>
        /// Fetches a Channel entity based on id
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="gameId">Game id</param>
        /// <returns>Found Channel entity</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ChannelDTO>> GetChannelById(int id, int gameId)
        {
            try
            {
                return Ok(_mapper.Map<ChannelDTO>(await _channelService.GetChannelById(id, gameId)));
            }
            catch (ChannelNotFoundException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Fetches a Channel entity with details based on id
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="gameId">Game id</param>
        /// <returns>Found Channel entity</returns>
        [HttpGet("{id}/withdetails")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<DetailedChannelDTO>> GetChannelWithDetailsById(int id, int gameId)
        {
            try
            {
                return Ok(_mapper.Map<DetailedChannelDTO>(await _channelService.GetChannelById(id, gameId)));
            }
            catch (ChannelNotFoundException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Creates a new Channel entity
        /// </summary>
        /// <param name="channelDto">Channel entity to create</param>
        /// <returns>Fully created Channel entity</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Channel>> CreateChannel(ChannelDTO channelDto)
        {
            var channel = _mapper.Map<Channel>(channelDto);
            try
            {
                await _channelService.CreateChannel(channel, channelDto.GameId);
                return CreatedAtAction(nameof(GetChannelById), new { id = channel.Id }, channel);
            }
            catch(ChannelAlreadyExistsException ex)
            {
                return BadRequest(new ProblemDetails { Detail= ex.Message });
            }
        }

        /// <summary>
        /// Updates a Channel entity
        /// </summary>
        /// <param name="id">Id of entity to update</param>
        /// <param name="updatedChannel">Values to update with</param>
        /// <param name="gameId">Game id</param>
        /// <returns>Complete updated Channel entity</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ChannelDTO>> UpdateChannel(int id, Channel updatedChannel, int gameId)
        {
            if (id != updatedChannel.Id)
                return BadRequest();
            var result = await _channelService.UpdateChannel(updatedChannel, gameId);
            return Ok(_mapper.Map<ChannelDTO>(result));
        }

        /// <summary>
        /// Deletes an existing Channel entity and all players and kills included in it 
        /// </summary>
        /// <param name="id">Id of entity to delete</param>
        /// <param name="gameId">Game id</param>
        /// <returns>NoContent or NotFound</returns>
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteChannel(int id, int gameId)
        {
            try
            {
                await _channelService.DeleteChannel(id, gameId);
            }
            catch (ChannelNotFoundException error)
            {
                return NotFound(new ProblemDetails { Detail = error.Message });
            }
            return NoContent();
        }
    }
}

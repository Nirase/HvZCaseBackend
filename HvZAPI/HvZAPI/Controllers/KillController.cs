﻿using AutoMapper;
using HvZAPI.Models;
using HvZAPI.Services.Concrete;
using HvZAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace HvZAPI.Controllers
{
    [Route("api/v1/game/{gameId}/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class KillController : ControllerBase
    {
        private readonly IKillService _killService;
        private readonly IMapper _mapper;

        public KillController(IKillService KillService, IMapper mapper)
        {
            _killService = KillService;
            _mapper = mapper;
        }

        /// <summary>
        /// Fetches all Kills
        /// </summary>
        /// <returns>Enumerable of all Kills</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kill>>> GetKills(int gameId)
        {
            return Ok(_mapper.Map<IEnumerable<Kill>>(await _killService.GetKills(gameId)));
        }

        /// <summary>
        /// Fetches a Kill entity based on id
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Found Kill entity</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Kill>> GetKillById(int id, int gameId)
        {
            try
            {
                return Ok(_mapper.Map<Kill>(await _killService.GetKillById(id, gameId)));
            }
            catch (Exception ex)
            {
                return NotFound(new ProblemDetails
                {
                    Detail = ex.Message
                });
            }
        }


        /// <summary>
        /// Creates a new Kill entity
        /// </summary>
        /// <param name="biteCode">Supplied bite code</param>
        /// <returns>Fully created Kill entity</returns>
        [HttpPost]
        public async Task<ActionResult<Kill>> CreateKill(int killerId, int gameId, string biteCode)
        {
            var kill = await _killService.CreateKill(killerId, gameId, biteCode);
            return CreatedAtAction(nameof(GetKillById), new { id = kill.Id }, kill);
        }


        /// <summary>
        /// Deletes a Kill entity
        /// </summary>
        /// <param name="killId">Id of entity to delete</param>
        /// <param name="gameId">Game that kill is in</param>
        [HttpDelete]
        public async Task<IActionResult> DeleteKill(int killId, int gameId)
        {
            try
            {
                await _killService.DeleteKill(killId, gameId);
            }
            catch (Exception error)
            {
                return NotFound(new ProblemDetails { Detail = error.Message });
            }
            return NoContent();
        }
    }
}

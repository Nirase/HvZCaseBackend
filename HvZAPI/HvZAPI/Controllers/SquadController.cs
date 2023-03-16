﻿using AutoMapper;
using HvZAPI.Models.DTOs.SquadDTOs;
using HvZAPI.Models;
using HvZAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using HvZAPI.Services.Concrete;
using HvZAPI.Models.DTOs.SquadDTOs;

namespace HvZAPI.Controllers
{
    [Route("api/v1/game/{gameId}/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class SquadController : ControllerBase
    {
        private readonly ISquadService _squadService;
        private readonly IMapper _mapper;


        public SquadController(ISquadService SquadService, IMapper mapper)
        {
            _squadService = SquadService;
            _mapper = mapper;
        }


        /// <summary>
        /// Gets all squads in a game
        /// </summary>
        /// <param name="gameId">Game id</param>
        /// <returns>Found squads</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SquadDTO>>> GetSquads(int gameId)
        {
            return Ok(_mapper.Map<IEnumerable<SquadDTO>>(await _squadService.GetSquads(gameId)));
        }

        /// <summary>
        /// Gets a squad based on id
        /// </summary>
        /// <param name="id">Id of squad entity</param>
        /// <param name="gameId">Game id to search within</param>
        /// <returns>Found squad</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SquadDTO>> GetSquadById(int id, int gameId)
        {
            return Ok(_mapper.Map<SquadDTO>(await _squadService.GetSquadById(id, gameId)));
        }


        [HttpPost]
        [ActionName(nameof(GetSquadById))]
        public async Task<ActionResult<SquadDTO>> CreateSquad(int gameId, CreateSquadDTO createSquadDTO)
        {
            var creatorId = createSquadDTO.CreatorId;
            var squad = _mapper.Map<Squad>(createSquadDTO);
            var created = await _squadService.CreateSquad(squad, gameId, creatorId);
            return CreatedAtAction(nameof(GetSquadById), new { id = created.Id }, _mapper.Map<SquadDTO>(created));
        }
    }
}

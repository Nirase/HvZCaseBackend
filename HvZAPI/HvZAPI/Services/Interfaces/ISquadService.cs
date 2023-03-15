﻿using HvZAPI.Models;

namespace HvZAPI.Services.Interfaces
{
    public interface ISquadService
    {
        /// <summary>
        /// Fetches all Squads
        /// </summary>
        /// <returns>Enumerable of Squads</returns>
        Task<IEnumerable<Squad>> GetSquads(int gameId);

        /// <summary>
        /// Fetches Squad based on id
        /// </summary>
        /// <param name="id">Squad Id to find</param>
        /// <returns>Found Squad entity</returns>
        Task<Squad> GetSquadById(int id, int gameId);

        /// <summary>
        /// Creates a new Squad entity
        /// </summary>
        /// <param name="Squad">Squad to create</param>
        /// <returns>Created Squad entity</returns>
        Task<Squad> CreateSquad(Squad Squad, int gameId);

        /// <summary>
        /// Updates an existing Squad entity
        /// </summary>
        /// <param name="Squad">Squad entity to update to</param>
        /// <returns>Updated Squad entity</returns>
        Task<Squad> UpdateSquad(Squad Squad, int gameId);

        /// <summary>
        /// Deletes a Squad entity
        /// </summary>
        /// <param name="id">Id of Squad entity</param>
        /// <returns></returns>
        Task DeleteSquad(int id, int gameId);
    }
}

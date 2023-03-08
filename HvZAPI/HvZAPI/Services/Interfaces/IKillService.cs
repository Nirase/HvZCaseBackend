﻿using HvZAPI.Models;

namespace HvZAPI.Services.Interfaces
{
    public interface IKillService
    {

        /// <summary>
        /// Fetches all Kills
        /// </summary>
        /// <returns>Enumerable of Kills</returns>
        Task<IEnumerable<Kill>> GetKills(int gameId);

        /// <summary>
        /// Fetches Kill based on id
        /// </summary>
        /// <param name="id">Kill Id to find</param>
        /// <returns>Found Kill entity</returns>
        Task<Kill> GetKillById(int id, int gameId);

        /// <summary>
        /// Creates a new Kill entity
        /// </summary>
        /// <param name="killerId">Id of killer</param>
        /// <param name="gameId">Id of game</param>
        /// <param name="biteCode">Supplied bitecode</param>
        /// <returns>Created Kill entity</returns>
        Task<Kill> CreateKill(int killerId, int gameId, string biteCode);
    }
}

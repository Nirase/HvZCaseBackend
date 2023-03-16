﻿using HvZAPI.Contexts;
using HvZAPI.Models;
using HvZAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HvZAPI.Services.Concrete
{
    public class SquadService : ISquadService
    {
        private readonly HvZDbContext _context;
        public SquadService(HvZDbContext context)
        {
            _context = context;
        }

        public async Task<Squad> CreateSquad(Squad squad, int gameId, int creatorId)
        {
            var creator = await _context.Players.FindAsync(creatorId);
            if (creator is null)
                throw new Exception("Player not found");
            if (creator.SquadId != null)
                throw new Exception("Player already in a squad");
            var foundSquad = GetSquadByName(squad.Name, gameId);
            if(foundSquad != null)
                throw new Exception($"Squad with name {squad.Name} already exists");

            await _context.Squads.AddAsync(squad);
            await _context.SaveChangesAsync();
            creator.SquadId = squad.Id;
            await _context.SaveChangesAsync();
            return squad;
        }

        public Task DeleteSquad(int id, int gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<Squad> GetSquadById(int id, int gameId)
        {
            var squad = await _context.Squads.Include(x => x.Players).Include(x => x.SquadCheckIns).Where(x => x.GameId == gameId).FirstOrDefaultAsync(x => x.Id == id);
            if (squad is null)
                throw new Exception("Squad not found");
            return squad;
        }

        public async Task<Squad?> GetSquadByName(string name, int gameId)
        {
            var squad = await _context.Squads.Include(x => x.Players).Include(x => x.SquadCheckIns).Where(x => x.GameId == gameId).FirstOrDefaultAsync(x => x.Name == name);
            if (squad != null)
                throw new Exception($"Squad with name {squad.Name} already exists");
            return squad;
        }

        public async Task<IEnumerable<Squad>> GetSquads(int gameId)
        {
            return await _context.Squads.Include(x => x.Players).Include(x => x.SquadCheckIns).Where(x => x.GameId == gameId).ToListAsync();
        }

        public Task<Squad> UpdateSquad(Squad Squad, int gameId)
        {
            throw new NotImplementedException();
        }
    }
}

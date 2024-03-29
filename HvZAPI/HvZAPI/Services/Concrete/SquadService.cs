﻿using HvZAPI.Contexts;
using HvZAPI.Exceptions;
using HvZAPI.Models;
using HvZAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HvZAPI.Services.Concrete
{
    public class SquadService : ISquadService
    {
        private readonly HvZDbContext _context;
        public SquadService(HvZDbContext context)
        {
            _context = context;
        }

        public async Task<Squad> CreateSquad(Squad squad, int gameId, int creatorId, string subject)
        {
            var subjectPlayer = await _context.Players.Include(x => x.User).Where(x => x.User.KeycloakId == subject).Where(x => x.GameId == gameId).FirstOrDefaultAsync();
            if (subjectPlayer is null)
                throw new PlayerNotFoundException("Player not found by subject");

            var creator = await _context.Players.FindAsync(creatorId);
            if (creator is null)
                throw new PlayerNotFoundException("Player not found");

            if (creator.Id != subjectPlayer.Id)
                throw new SubjectDoesNotMatchException("Subject does not match found player");
            if (creator.SquadId != null)
                throw new PlayerAlreadyInSquadException("Player already in a squad");
            var foundSquad = await GetSquadByName(squad.Name, gameId);
            if(foundSquad != null)
                throw new SquadNameAlreadyInUseException($"Squad with name {squad.Name} already exists");

            var channel = new Channel { GameId = gameId, Name = squad.Name };
            await _context.Channels.AddAsync(channel);
            await _context.SaveChangesAsync();
            squad.ChannelId = channel.Id;
            await _context.Squads.AddAsync(squad);
            await _context.SaveChangesAsync();
            creator.SquadId = squad.Id;
            await _context.SaveChangesAsync();
            return squad;
        }

        public async Task DeleteSquad(int id, int gameId)
        {
            var foundSquad = await _context.Squads.Include(x => x.Players).ThenInclude(x => x.User).Include(x => x.SquadCheckIns).Where(x => x.GameId == gameId).FirstOrDefaultAsync(x => x.Id == id);
            if (foundSquad == null)
                throw new SquadNotFoundException("Squad not found");
            _context.Squads.Remove(foundSquad);
            await _context.SaveChangesAsync();
        }

        public async Task<Squad> GetSquadById(int id, int gameId, string subject)
        {
            var issuer = await _context.Players.Include(x => x.User).Where(x => x.User.KeycloakId == subject).Where(x => x.SquadId == id).FirstOrDefaultAsync();
            if (issuer is null)
                throw new SubjectDoesNotMatchException("Subject is not part of squad");
            var squad = await _context.Squads.Include(x => x.Players).ThenInclude(x => x.User).Include(x => x.SquadCheckIns).Where(x => x.GameId == gameId).FirstOrDefaultAsync(x => x.Id == id);
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
            return await _context.Squads.Include(x => x.Players).ThenInclude(x => x.User).Include(x => x.SquadCheckIns).Where(x => x.GameId == gameId).ToListAsync();
        }

        public async Task<Squad> JoinSquad(int gameId, int squadId, int playerId, string subject)
        {
            var subjectPlayer = await _context.Players.Include(x => x.User).Where(x => x.User.KeycloakId == subject).Where(x => x.GameId == gameId).FirstOrDefaultAsync();
            if (subjectPlayer is null)
                throw new PlayerNotFoundException("Player not found by subject");


            var foundPlayer = await _context.Players.Include(x => x.Squad).FirstOrDefaultAsync(x=> x.Id == playerId);
            if (foundPlayer is null)
                throw new PlayerNotFoundException("Player not found");
            if (foundPlayer.Id != subjectPlayer.Id)
                throw new SubjectDoesNotMatchException("Subject does not match found player");


            if (foundPlayer.Squad != null)
                throw new PlayerAlreadyInSquadException("Player is already in a squad");
            
            var foundSquad = await _context.Squads.Include(x => x.Players).FirstOrDefaultAsync(x => x.Id == squadId);
            if (foundSquad is null)
                throw new SquadNotFoundException("Squad not found");
            
            foundPlayer.SquadId = squadId;
            foundSquad.Players.Add(foundPlayer);
            await _context.SaveChangesAsync();
            return foundSquad;
        }

        public async Task<Squad> LeaveSquad(int gameId, int squadId, int playerId, string subject)
        {
            var subjectPlayer = await _context.Players.Include(x => x.User).Where(x => x.User.KeycloakId == subject).Where(x => x.GameId == gameId).FirstOrDefaultAsync();
            if (subjectPlayer is null)
                throw new PlayerNotFoundException("Player not found by subject");


            var foundPlayer = await _context.Players.Include(x => x.Squad).FirstOrDefaultAsync(x => x.Id == playerId);
            if (foundPlayer is null)
                throw new PlayerNotFoundException("Player not found");

            if (foundPlayer.Id != subjectPlayer.Id)
                throw new SubjectDoesNotMatchException("Subject does not match found player");

            if (foundPlayer.Squad is null)
                throw new PlayerNotInASquadException("Player is not in a squad");
            if (foundPlayer.Squad.Id != squadId)
                throw new PlayerLeavingWrongSquadException("Player is trying to leave a squad they are not in");
            var foundSquad = await _context.Squads.Include(x => x.Players).FirstOrDefaultAsync(x => x.Id == squadId);
            if (foundSquad is null)
                throw new SquadNotFoundException("Squad not found");

            foundPlayer.SquadId = null;
            foundSquad.Players.Remove(foundPlayer);
            await _context.SaveChangesAsync();
            return foundSquad;
        }

        public async Task<Squad> UpdateSquad(Squad Squad, int gameId)
        {
            var foundSquad = await _context.Squads.Include(x => x.Players).ThenInclude(x => x.User).Include(x => x.SquadCheckIns).Where(x => x.GameId == gameId).FirstOrDefaultAsync(x => x.Id == Squad.Id);
            if (foundSquad is null)
                throw new SquadNotFoundException("Squad not found");
            foundSquad.Name = Squad.Name;
            await _context.SaveChangesAsync();
            return foundSquad;
        }
    }
}

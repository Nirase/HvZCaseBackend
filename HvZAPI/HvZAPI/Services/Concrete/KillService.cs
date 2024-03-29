﻿using HvZAPI.Contexts;
using HvZAPI.Exceptions;
using HvZAPI.Models;
using HvZAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HvZAPI.Services.Concrete
{
    public class KillService : IKillService
    {
        private readonly HvZDbContext _context;
        public KillService(HvZDbContext context)
        {
            _context = context;
        }

        public async Task<Kill> CreateKill(Kill kill, int gameId, string biteCode, string subject)
        {
            var issuer = await _context.Players.Include(x => x.User).Where(x => x.User.KeycloakId == subject).Where(x => x.Id == kill.KillerId).FirstOrDefaultAsync();
            if (issuer is null)
                throw new SubjectDoesNotMatchException("Subject does not match request");
            var victim = await _context.Players.Where(x => x.GameId == gameId).FirstOrDefaultAsync(p => p.BiteCode== biteCode);
            if (victim is null)
                throw new PlayerNotFoundException($"Victim with bite code {biteCode} not found");

            var killer = await _context.Players.Where(x => x.GameId == gameId).FirstOrDefaultAsync(p => p.Id == kill.KillerId);
            if (killer is null)
                throw new PlayerNotFoundException($"Killer {kill.KillerId} not found");
            if (killer.IsHuman || !victim.IsHuman)
                throw new InvalidKillException($"Invalid kill, killer is alive or victim is already dead: Killer is alive: {killer.IsHuman}, Victim is alive: {victim.IsHuman}");

            var killToMake = new Kill { GameId = gameId, TimeOfDeath = kill.TimeOfDeath, KillerId = kill.KillerId, VictimId = victim.Id, Description = kill.Description, Location = kill.Location };
            victim.IsHuman = false;
            _context.Kills.Add(killToMake);
            await _context.SaveChangesAsync();
            return killToMake;
        }

        public async Task DeleteKill(int killId, int gameId)
        {
            var kill = await GetKillById(killId, gameId);
            if (kill is null)
                throw new KillNotFoundException($"Kill {killId} not found");

            var game = await _context.Games.Include(x => x.Kills).FirstOrDefaultAsync(x => x.Id == gameId);

            if (game is null)
                throw new GameNotFoundException($"Game {gameId} not found");

            var victim = await _context.Players.FirstOrDefaultAsync(x => x.Id == kill.VictimId);

            if (victim is null)
                throw new PlayerNotFoundException($"Victim {kill.VictimId} not found");

            victim.IsHuman = true;
            game.Kills.Remove(kill);
            await _context.SaveChangesAsync();
        }

        public async Task<Kill> GetKillById(int id, int gameId)
        {
            var kill = await _context.Kills.Include(x => x.Victim).Include(x => x.Killer).Where(x => x.GameId == gameId).FirstOrDefaultAsync(x => x.Id == id);
            if (kill is null)
                throw new KillNotFoundException("Kill Not Found");
            return kill;
        }

        public async Task<IEnumerable<Kill>> GetKills(int gameId)
        {
            return await _context.Kills.Include(x => x.Victim).Include(x => x.Killer).Where(x => x.GameId == gameId).ToListAsync();
        }

        public async Task<Kill> UpdateKill(Kill kill, int gameId)
        {
            var currentKill = await GetKillById(kill.Id, gameId);
            if (currentKill is null)
                throw new KillNotFoundException($"Kill {kill.Id} not found");

            if(kill.VictimId != currentKill.VictimId)
            {
                currentKill.Victim.IsHuman = true;
                var newVictim = await _context.Players.FirstOrDefaultAsync(x => x.Id == kill.VictimId);
                newVictim.IsHuman = false;
                currentKill.Victim= newVictim;
            }
            currentKill.GameId = gameId;
            currentKill.VictimId = kill.VictimId;
            currentKill.Location = kill.Location;
            currentKill.TimeOfDeath = kill.TimeOfDeath;
            currentKill.KillerId = kill.KillerId;
            currentKill.Id = kill.Id;
            await _context.SaveChangesAsync();
            return currentKill;
        }
    }
}

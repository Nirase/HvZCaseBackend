﻿namespace HvZAPI.Models.DTOs.KillDTOs
{
    public class LightweightKillDTO
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string TimeOfDeath { get; set; }
        public string? Location { get; set; }
        public int? VictimId { get; set; }
        public int? KillerId { get; set; }
    }
}

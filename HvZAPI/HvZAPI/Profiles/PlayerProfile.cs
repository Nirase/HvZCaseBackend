﻿using AutoMapper;
using HvZAPI.Models;
using HvZAPI.Models.DTOs.PlayerDTOs;

namespace HvZAPI.Profiles
{
    public class PlayerProfile : Profile
    {

        public PlayerProfile() 
        {
            CreateMap<Player, LightweightPlayerDTO>()
                .ForMember(dto => dto.FirstName, options =>
                {
                    options.MapFrom(src => src.User.first_name);
                })
                .ForMember(dto => dto.LastName, options =>
                {
                    options.MapFrom(src => src.User.last_name);
                })
                .ForMember(dto => dto.URL, options =>
                {
                    options.MapFrom(src => $"api/v1/game/{src.GameId}/player/{src.Id}");
                });

        }
    }
}
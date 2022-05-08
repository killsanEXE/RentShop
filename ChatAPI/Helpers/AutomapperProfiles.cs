using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChatAPI.DTOs;
using ChatAPI.Entities;

namespace ChatAPI.Helpers
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<Group, GroupDTO>()
                .ForMember(f => f.Username1, opt => opt.MapFrom(s => s.User1!.UserName))
                .ForMember(f => f.Username2, opt => opt.MapFrom(s => s.User2!.UserName))
                .ForMember(f => f.User1Photo, opt => opt.MapFrom(s => s.User1!.PhotoUrl))
                .ForMember(f => f.User2Photo, opt => opt.MapFrom(s => s.User2!.PhotoUrl));
            
            CreateMap<Message, MessageDTO>();
            CreateMap<MessageDTO, Message>();
        }


    }
}
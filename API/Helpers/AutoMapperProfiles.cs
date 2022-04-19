using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RegisterDTO, AppUser>();
            CreateMap<AppUser, ClientDTO>().ForMember(f => f.Age, opt => opt.MapFrom(s => s.DateOfBirth.CalculateAge()));

            // CreateMap<Item, ItemDTO>().ForMember(f => f.PreviewPhotoUrl, opt => opt.MapFrom(s => s.Photos!.FirstOrDefault(f => f.IsPreview)!.Url));
            CreateMap<Item, ItemDTO>();

            CreateMap<ItemDTO, Item>();

            CreateMap<ItemUnitPoint, UnitDTO>()
                .ForMember(f => f.Description, opt => opt.MapFrom(s => s.Unit!.Description))
                .ForMember(f => f.Point, opt => opt.MapFrom(s => s.Point))
                .ForMember(f => f.WhenWillBeAvaliable, opt => opt.MapFrom(s => s.Unit!.WhenWillBeAvailable))
                .ForMember(f => f.Disabled, opt => opt.MapFrom(s => s.Unit!.Disabled))
                .ForMember(f => f.IsAvailable, opt => opt.MapFrom(s => s.Unit!.IsAvailable));

            CreateMap<LocationDTO, Location>();
            CreateMap<LocationDTO, Point>();
            CreateMap<Location, LocationDTO>();
            // CreateMap<LocationDTO, Point>().ForMember(f => f.Disabled, opt => opt.MapFrom(s => false));

            CreateMap<Point, PointDTO>();

            CreateMap<Photo, PhotoDTO>();

            CreateMap<Unit, UnitDTO>()
                .ForMember(f => f.Point, opt => opt.MapFrom(s => s.ItemUnitPoint!.Point))
                .ForMember(f => f.ItemId, opt => opt.MapFrom(s => s.ItemUnitPoint!.Item!.Id));

            CreateMap<AppUser, DeliverymanDTO>().ForMember(f => f.Age, opt => opt.MapFrom(s => s.DateOfBirth.CalculateAge()));
            
            CreateMap<Order, OrderDTO>();

            CreateMap<Message, MessageDTO>();
            CreateMap<MessageDTO, Message>();
        }
    }
}
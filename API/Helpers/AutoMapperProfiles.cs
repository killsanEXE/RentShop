using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RegisterDTO, AppUser>();
            CreateMap<AppUser, ClientDTO>();

            // CreateMap<Item, ItemDTO>().ForMember(f => f.PreviewPhotoUrl, opt => opt.MapFrom(s => s.Photos!.FirstOrDefault(f => f.IsPreview)!.Url));
            CreateMap<Item, ItemDTO>();
            CreateMap<ItemDTO, Item>();

            CreateMap<ItemUnitPoint, UnitDTO>()
                .ForMember(f => f.Description, opt => opt.MapFrom(s => s.Unit!.Description))
                .ForMember(f => f.Point, opt => opt.MapFrom(s => s.Point))
                .ForMember(f => f.WhenWillBeAvaliable, opt => opt.MapFrom(s => s.Unit!.WhenWillBeAvaliable))
                .ForMember(f => f.IsAvaliable, opt => opt.MapFrom(s => s.Unit!.IsAvaliable));

            CreateMap<LocationDTO, Location>();
            CreateMap<LocationDTO, Point>();

            CreateMap<Point, PointDTO>();

            CreateMap<Photo, PhotoDTO>();

            CreateMap<Unit, UnitDTO>()
                .ForMember(f => f.Point, opt => opt.MapFrom(s => s.ItemUnitPoint!.Point));
        }
    }
}
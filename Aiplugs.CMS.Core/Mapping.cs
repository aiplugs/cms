using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core
{
    public class Mapping
    {
        public static IMapper Mapper = new MapperConfiguration(config =>
        {
            config.CreateMap<CMS.Data.Entities.File, Models.File>()
                    .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dst => dst.Size, opt => opt.MapFrom(src => src.Size))
                    .ForMember(dst => dst.ContentType, opt => opt.MapFrom(src => src.ContentType))
                    .ForMember(dst => dst.LastModifiedAt, opt => opt.MapFrom(src => src.LastModifiedAt))
                    .ForMember(dst => dst.LastModifiedBy, opt => opt.MapFrom(src => src.LastModifiedBy));

            config.CreateMap<CMS.Data.Entities.Folder, Models.Folder>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Path, opt => opt.MapFrom(src => src.Path));

        }).CreateMapper();
    }
}

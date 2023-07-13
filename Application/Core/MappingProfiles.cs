using Application.ProjectVersions;
using AutoMapper;
using Domain.ProjectHierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<ProjectVersion, ProjectVersionDto>()
                .ForMember(d => d.ProjectId, o => o.MapFrom(s => s.Id));
        }
    }
}

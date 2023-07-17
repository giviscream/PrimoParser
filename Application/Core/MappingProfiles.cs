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

            //CreateMap<ContentFile, ContentFileChangesAnalyzer>()
            //    .ForMember(d => d.Id, o => o.Ignore())
            //    .ForMember(d => d.ProjectVersionId, o => o.MapFrom(s => s.ProjectVersion.Id));
        }
    }
}

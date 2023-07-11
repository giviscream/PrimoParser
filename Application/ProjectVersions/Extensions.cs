using Domain.ProjectHierarchy;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ProjectVersions
{
    public static class Extensions
    {
        public static Task<ProjectVersion?> GetLastVersionAsync(this IQueryable<ProjectVersion> projectVersionsColl, Guid projectId)
        {
            return Task<ProjectVersion?>.Run(() =>
            {
                var pVersions = projectVersionsColl.Where(x => x.Project.Id == projectId);

                return pVersions?.OrderByDescending(x => x.DateTime).FirstOrDefault();
            });
        }
    }
}

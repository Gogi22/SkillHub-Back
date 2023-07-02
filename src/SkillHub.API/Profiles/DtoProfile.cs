using AutoMapper;
using SkillHub.API.Entities;
using SkillHub.API.Features.FreelancerProfile.Queries;
using SkillHub.API.Features.Project.Queries;

namespace SkillHub.API.Profiles;

public class DtoProfile : Profile
{
    public DtoProfile()
    {
        CreateMap<Project, GetProject.Project>()
            .ConstructUsing(project => ProjectEntityToDto(project));
        CreateMap<Client, Features.ClientProfile.Queries.GetProfile.Profile>()
            .ConstructUsing(client => ClientEntityToProfileDto(client));
        CreateMap<Freelancer, GetProfile.Profile>()
            .ConstructUsing(freelancer => FreelancerEntityToProfileDto(freelancer));
    }

    private static GetProfile.Profile FreelancerEntityToProfileDto(Freelancer freelancer)
    {
        return new GetProfile.Profile
        {
            FirstName = freelancer.FirstName,
            LastName = freelancer.LastName,
            Bio = freelancer.Bio,
            Title = freelancer.Title,
            ProfilePhotoUrl = freelancer.ProfilePhotoId,
            Skills = freelancer.Skills.Select(skill => skill.Name).ToList()
        };
    }

    private static Features.ClientProfile.Queries.GetProfile.Profile ClientEntityToProfileDto(Client client)
    {
        return new Features.ClientProfile.Queries.GetProfile.Profile
        {
            ClientInfo = client.ClientInfo,
            FirstName = client.FirstName,
            LastName = client.LastName,
            CompanyName = client.CompanyName,
            WebsiteUrl = client.WebsiteUrl
        };
    }

    private static GetProject.Project ProjectEntityToDto(Project project)
    {
        return new GetProject.Project
        {
            Budget = project.Budget,
            ClientId = project.ClientId,
            Description = project.Description,
            ExperienceLevel = project.ExperienceLevel.ToString(),
            FreelancerId = project.FreelancerId,
            Id = project.Id,
            ProjectStatus = project.ProjectStatus.ToString(),
            Title = project.Title
        };
    }
}
using AutoMapper;
using SkillHub.API.Entities;
using SkillHub.API.Features.FreelancerProfile.Queries;
using SkillHub.API.Features.Project.Queries;
using SkillHub.API.Features.Proposal.Queries;

namespace SkillHub.API.Profiles;

public class DtoProfile : Profile
{
    public DtoProfile()
    {
        CreateMap<Project, ProjectDto>()
            .ConstructUsing(project => ProjectEntityToDto(project));
        CreateMap<Client, Features.ClientProfile.Queries.GetProfile.ProfileDto>()
            .ConstructUsing(client => ClientEntityToProfileDto(client));
        CreateMap<Freelancer, GetProfile.Profile>()
            .ConstructUsing(freelancer => FreelancerEntityToProfileDto(freelancer));
        CreateMap<Proposal, GetProposal.ProposalDto>()
            .ConstructUsing(proposal => ProposalEntityToDto(proposal));
    }

    private static GetProposal.ProposalDto ProposalEntityToDto(Proposal proposal) =>
        new()
        {
            FreelancerId = proposal.FreelancerId,
            ProjectId = proposal.ProjectId,
            CoverLetter = proposal.CoverLetter,
            Status = proposal.Status,
            CreatedAt = proposal.CreatedAt
        };

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

    private static Features.ClientProfile.Queries.GetProfile.ProfileDto ClientEntityToProfileDto(Client client)
    {
        return new Features.ClientProfile.Queries.GetProfile.ProfileDto
        {
            ClientInfo = client.ClientInfo,
            FirstName = client.FirstName,
            LastName = client.LastName,
            CompanyName = client.CompanyName,
            WebsiteUrl = client.WebsiteUrl
        };
    }

    private static ProjectDto ProjectEntityToDto(Project project)
    {
        return new()
        {
            Budget = project.Budget,
            ClientId = project.ClientId,
            Description = project.Description,
            ExperienceLevel = project.ExperienceLevel.ToString(),
            FreelancerId = project.FreelancerId,
            Id = project.Id,
            ProjectStatus = project.Status.ToString(),
            Title = project.Title
        };
    }
}
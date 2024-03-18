using Microsoft.AspNetCore.Mvc;
using Promact.CustomerSuccess.Platform.Entities;
using Promact.CustomerSuccess.Platform.Services.Dtos;
using Promact.CustomerSuccess.Platform.Services.Dtos.Project;
using Promact.CustomerSuccess.Platform.Services.Dtos.RiskProfile;
using Promact.CustomerSuccess.Platform.Services.Emailing;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Promact.CustomerSuccess.Platform.Services
{
    public class ProjectService : CrudAppService<
                                Project,
                                ProjectDto,
                                Guid,
                                PagedAndSortedResultRequestDto,
                                CreateProjectDto,
                                UpdateProjectDto>,
                                IProjectService
    {
        private readonly IEmailService _emailService;
        
        private readonly IRepository<Project,Guid> _projectRepository;
        public ProjectService(IRepository<Project, Guid> projectRepository, IEmailService emailService) : base(projectRepository)
        {
            _emailService = emailService;
            _projectRepository = projectRepository;
        }
        public override async Task<ProjectDto> CreateAsync(CreateProjectDto input)
        {
            var projectDto = await base.CreateAsync(input);


            return projectDto;
        }

        public override async Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto input)
        {
            var projectDto = await base.UpdateAsync(id, input);


            return projectDto;
        }

        public override async Task DeleteAsync(Guid id)
        {
            await base.DeleteAsync(id);
        }

        [HttpGet("projects")]
        public async Task<List<ProjectDto>> GetProjectsByUserIdAsync([FromQuery] Guid? UserId)
        {
            List<Project> projects;
            if (UserId.HasValue)
            {
                projects = await _projectRepository.GetListAsync(p => p.UserId == UserId);
            }
            else
            {
                projects = await _projectRepository.GetListAsync();
            }

            return ObjectMapper.Map<List<Project>, List<ProjectDto>>(projects);
        }

    }
}

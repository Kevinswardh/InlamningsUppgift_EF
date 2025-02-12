using Business.DTOs;

public interface IProjectService
{
    Task<string> GetNextProjectNumberAsync();
    Task CreateProjectAsync(ProjectDTO projectDto);
    Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync();
    Task<ProjectDTO> GetProjectByIdAsync(int id);
    Task UpdateProjectAsync(ProjectDTO projectDto);
    Task DeleteProjectAsync(int id);
    Task<IEnumerable<ProjectLeaderDTO>> GetAllProjectLeadersAsync();

}

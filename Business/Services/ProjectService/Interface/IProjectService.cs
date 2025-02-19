using Business.DTOs;

/// <summary>
/// Defines methods for managing project operations.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Retrieves the next available project number asynchronously.
    /// </summary>
    /// <returns>A string representing the next project number.</returns>
    Task<string> GetNextProjectNumberAsync();


    /// <summary>
    /// Creates a new project asynchronously.
    /// </summary>
    /// <param name="projectDto">The data transfer object containing project information.</param>
    Task CreateProjectAsync(ProjectDTO projectDto);


    /// <summary>
    /// Retrieves all projects asynchronously.
    /// </summary>
    /// <returns>A collection of <see cref="ProjectDTO"/> representing all projects.</returns>
    Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync();


    /// <summary>
    /// Retrieves a project by its ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the project.</param>
    /// <returns>A <see cref="ProjectDTO"/> representing the requested project.</returns>
    Task<ProjectDTO> GetProjectByIdAsync(int id);


    /// <summary>
    /// Updates an existing project asynchronously.
    /// </summary>
    /// <param name="projectDto">The data transfer object containing updated project information.</param>
    Task UpdateProjectAsync(ProjectDTO projectDto);


    /// <summary>
    /// Deletes a project by its ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the project to delete.</param>
    Task DeleteProjectAsync(int id);


    /// <summary>
    /// Retrieves all project leaders asynchronously.
    /// </summary>
    /// <returns>A collection of <see cref="ProjectLeaderDTO"/> representing all project leaders.</returns>
    Task<IEnumerable<ProjectLeaderDTO>> GetAllProjectLeadersAsync();

}

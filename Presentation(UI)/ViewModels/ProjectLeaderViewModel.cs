public class ProjectLeaderViewModel
{
    public int ProjectLeaderID { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Department { get; set; }

    public int IsDeleted { get; set; }
    public string ProjectLeaderName => $"{FirstName} {LastName}".Trim();
}

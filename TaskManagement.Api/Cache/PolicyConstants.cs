namespace TaskManagement.Api.Cache;

public static class PolicyConstants
{
    public static readonly (string name, string tag) GetAllTasksCache = new ("GetAllTasksCache", "Tasks");
    public static readonly (string name, string tag) GetTaskCache = new ("GetTaskCache", "Task");
    public static readonly (string name, string tag) GetAllUsersCache = new ("GetAllUsersCache", "Users");
    public static readonly (string name, string tag) GetUserCache = new ("GetUserCache", "User");
}
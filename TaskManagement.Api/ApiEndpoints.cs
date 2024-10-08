namespace TaskManagement.Api;

internal static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Jwt
    {
        private const string Base = $"{ApiBase}/jwt";
        public const string Create = Base;
    }

    public static class Users
    {
        private const string Base = $"{ApiBase}/users";
        public const string Create = Base;
        public const string Get = $"{Base}/{{id:guid}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
    } 
    public static class Tasks
    {
        private const string Base = $"{ApiBase}/tasks";
        public const string Create = Base;
        public const string Get = $"{Base}/{{id:guid}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
    }
}
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Tests.Users;

public class UserServiceIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    
    #region GetAll
    [Theory]
    [InlineData("Hello", "Email")]
    public async Task GetAllAsync_ShouldReturnUsers_WhenFiltersAreApplied(string username, string email)
    {
        //Arrange
        var client = factory.CreateClient();
        var fullResponse = await client.GetAsync($"/users/api/users");
        fullResponse.EnsureSuccessStatusCode();
        var notFilteredUsers = DeserializeResponse<UsersResponse>(await fullResponse.Content.ReadAsStringAsync());
        
        //Act
        var response = await client.GetAsync($"/users/api/users?username={username}&email={email}");
        response.EnsureSuccessStatusCode();

        //Assert
        var filteredUsers = DeserializeResponse<UsersResponse>(await response.Content.ReadAsStringAsync());
        var expectedUsers = notFilteredUsers.Users
            .Where(x => x.Email == email && x.Username == username)
            .ToList();
        Assert.Equal(expectedUsers.Count, filteredUsers.Users.Count());
        foreach (var expectedUser in expectedUsers)
        {
            Assert.Contains(expectedUser, filteredUsers.Users, new UserResponseComparer());
        }
    }
    #endregion
    
    
    public class UserResponseComparer : IEqualityComparer<UserResponse>
    {
        public bool Equals(UserResponse x, UserResponse y)
        {
            if (x == null || y == null)
                return false;

            return x.Username == y.Username && x.Email == y.Email; // Add other properties if needed
        }

        public int GetHashCode(UserResponse obj)
        {
            return HashCode.Combine(obj.Username, obj.Email);
        }
    }
    private T DeserializeResponse<T>(string response)
    {
        return JsonConvert.DeserializeObject<T>(JsonConvert.DeserializeObject<string>(response)!)!; 
    }
}
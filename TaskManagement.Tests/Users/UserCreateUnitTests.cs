using TaskManagement.Common.Models;
using Xunit;


namespace TaskManagement.Tests.Users;

public class UserCreateUnitTests
{
    [Theory]
    [InlineData("Valid Username", "Valid Email")]
    public void UserCreate_ShouldBeCreated_WhenParametersAreValid(string userName, string email)
    {
        //Arange
        var id = Guid.NewGuid();
        
        //Act
        User user = new User(id, userName, email);
        
        //Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(userName, user.Username);
        Assert.Equal(email, user.Email);
    }
    
    [Fact]
    public void UserCreate_ShouldThrowArgumentException_WhenUserNameIsEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var emptyUsername = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new User(id, emptyUsername, "Valid Email")
        );
    }
        
    [Fact]
    public void UserCreate_ShouldThrowArgumentNullException_WhenUserNameIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        string? nullUserName = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new User(id, nullUserName!, "Valid Email")
        );
    } 
    [Fact]
    public void UserCreate_ShouldThrowArgumentException_WhenUserEmailIsEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var emptyEmail = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new User(id,"Valid UserName",  emptyEmail)
        );
    }
        
    [Fact]
    public void UserCreate_ShouldThrowArgumentNullException_WhenEmailIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        string? nullEmail = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new User(id, "Valid UserName", nullEmail!)
        );
    } 
    
    
}
using TaskManagement.Common.Models;
using Xunit;


namespace TaskManagement.Tests.Users;

public class UserUpdateUnitTests
{
    [Theory]
    [InlineData("Valid Username", "Valid Email")]
    public void UserUpdate_ShouldBeUpdated_WhenParametersAreValid(string userName, string email)
    {
        //Arange
        var id = Guid.NewGuid();
        
        User user = new User(id, "ToUpdate", "ToUpdate");
        
        //Act
        user.UpdateEmail(email);
        user.UpdateUsername(userName);
        
        //Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(userName, user.Username);
        Assert.Equal(email, user.Email);
    }
    
    [Fact]
    public void TaskUpdate_ShouldThrowArgumentException_WhenUsernameIsEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var emptyUserName = string.Empty;
        User user = new User(id, "ToUpdate", "Valid Email");


        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            user.UpdateUsername(emptyUserName)
        );
    }
        
    [Fact]
    public void TaskUpdate_ShouldThrowArgumentNullException_WhenUsernameIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        string nullUserName = null!;
        User user = new User(id, "ToUpdate", "Valid Email");


        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            user.UpdateUsername(nullUserName)
        );
    }
   
    [Fact]
    public void TaskUpdate_ShouldThrowArgumentException_WhenEmailIsEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var emptyEmail = string.Empty;
        User user = new User(id, "Valid Username", "ToUpdate");


        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            user.UpdateEmail(emptyEmail)
        );
    }
        
    [Fact]
    public void TaskUpdate_ShouldThrowArgumentNullException_WhenEmailIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        string nullEmail = null!;
        User user = new User(id, "Valid UserName", "ToUpdate");


        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            user.UpdateEmail(nullEmail)
        );
    }
}
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Models;
using TaskManagement.Users.Services;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Tests.Users;

public class UserServiceUnitTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly IUserService _userService;
    private readonly Mock<ILogger<UserService>> _mockLogger;

    public UserServiceUnitTests()
    {
        _mockLogger = new Mock<ILogger<UserService>>();
        _mockRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockRepository.Object,
            Mock.Of<ILogger<UserService>>());
    }

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_ShouldReturnUser_WhenUserIsCreated()
    {
        // Arrange
        var validUser = new User(Guid.NewGuid(), "UserName", "Email");
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(validUser.Id, default))
            .ReturnsAsync(
                Result<bool>
                    .Success(false));
        _mockRepository.Setup(repo => repo.CreateAsync(validUser, default))
            .ReturnsAsync(Result<User>.Success(validUser));

        // Act
        var result = await _userService.CreateAsync(validUser);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validUser, result.Response);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnValidationError_WhenUserIsNull()
    {
        // Act
        var result = await _userService.CreateAsync(null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var validUser = new User(Guid.NewGuid(), "UserName", "Email");
        var error = new DatabaseError("Failed to create", "User", null, "Simulated error from repository");

        _mockRepository.Setup(repo => repo.ExistsByIdAsync(validUser.Id, default))
            .ReturnsAsync(
                Result<bool>
                    .Success(false)); //We need to do that because in the taskService.CreateAsync, ExistsByIdAsync is called, and we don't do that it will return null.
        _mockRepository.Setup(repo => repo.CreateAsync(validUser, default))
            .ReturnsAsync(Result<User>.Failure(error));

        // Act
        var result = await _userService.CreateAsync(validUser);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is DatabaseError);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedUser = new User(id, "UserName", "Email");

        _mockRepository.Setup(repo => repo.GetByIdAsync(id, default))
            .ReturnsAsync(Result<User>.Success(expectedUser));

        // Act
        var result = await _userService.GetByIdAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Response.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFoundError_WhenUserDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Mocking the repository to return a failure when the task does not exist
        _mockRepository.Setup(repo => repo.GetByIdAsync(id, default))
            .ReturnsAsync(Result<User>.Failure(new NotFoundError("User", id.ToString())));

        // Act
        var result = await _userService.GetByIdAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is NotFoundError); // Assuming your service logs this message.
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var error = new DatabaseError("Failed to get", "User", null, "Simulated error from repository");

        _mockRepository.Setup(repo => repo.GetByIdAsync(id, default))
            .ReturnsAsync(Result<User>.Failure(error));

        // Act
        var result = await _userService.GetByIdAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is DatabaseError);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsers_WhenFiltersAreApplied()
    {
        // Arrange
        var options = new GetAllUsersOptions
        {
            Username = "Test UserName",
            Email = "Test Email",
            Page = 1,
            PageSize = 10
        };
        var expectedUsers = new List<User>
        {
            new User(Guid.NewGuid(), "Test UserName", "Test Email")
        };

        _mockRepository.Setup(repo => repo.GetAllAsync(options, default))
            .ReturnsAsync(Result<IEnumerable<User>>.Success(expectedUsers));

        // Act
        var result = await _userService.GetAllAsync(options);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Response);
        Assert.Single(result.Response);
        Assert.Equal(expectedUsers[0].Email, result.Response.ToList()[0].Email);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsers_WhenNoFiltersAreApplied()
    {
        // Arrange
        var options = new GetAllUsersOptions
        {
            Page = 1,
            PageSize = 10
        };

        var expectedUsers = new List<User>
        {
            new User(Guid.NewGuid(), "Test UserName 1", "Test Email 1"),
            new User(Guid.NewGuid(), "Test UserName 2", "Test Email 2")
        };

        _mockRepository.Setup(repo => repo.GetAllAsync(options, default))
            .ReturnsAsync(Result<IEnumerable<User>>.Success(expectedUsers));

        // Act
        var result = await _userService.GetAllAsync(options);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Response);
        Assert.Equal(2, result.Response.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnValidationError_WhenOptionsIsNull()
    {
        // Act
        var result = await _userService.GetAllAsync(null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var options = new GetAllUsersOptions
        {
            Page = 1,
            PageSize = 10
        };

        var error = new DatabaseError("Failed to retrieve", "User", null, "Simulated error from repository");

        _mockRepository.Setup(repo => repo.GetAllAsync(options, default))
            .ReturnsAsync(Result<IEnumerable<User>>.Failure(error));

        // Act
        var result = await _userService.GetAllAsync(options);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is DatabaseError);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenUserIsUpdated()
    {
        // Arrange
        var validUser = new User(Guid.NewGuid(), "Valid UserName", "Valid Email");
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(validUser.Id, default))
            .ReturnsAsync(Result<bool>.Success(true));
        _mockRepository.Setup(repo => repo.UpdateAsync(validUser, default))
            .ReturnsAsync(Result<User>.Success(validUser));

        // Act
        var result = await _userService.UpdateAsync(validUser);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validUser, result.Response);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnValidationError_WhenUserIsNull()
    {
        // Act
        var result = await _userService.UpdateAsync(null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var error = new DatabaseError("Failed to update", "User", null, "Simulated error from repository");
        var validUser = new User(Guid.NewGuid(), "Valid UserName", "Valid Email");

        _mockRepository.Setup(repo => repo.ExistsByIdAsync(validUser.Id, default))
            .ReturnsAsync(Result<bool>.Success(true));
        _mockRepository.Setup(repo => repo.UpdateAsync(validUser, default))
            .ReturnsAsync(Result<User>.Failure(error));

        // Act
        var result = await _userService.UpdateAsync(validUser);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is DatabaseError);
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenUserIsDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Success(true));
        _mockRepository.Setup(repo => repo.DeleteByIdAsync(id, default))
            .ReturnsAsync(Result<None>.Success(None.Value));

        // Act
        var result = await _userService.DeleteByIdAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnValidationError_WhenUserDontExist()
    {
        //Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Success(false));

        // Act
        var result = await _userService.DeleteByIdAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is NotFoundError);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var error = new DatabaseError("Failed to delete", "User", null, "Simulated error from repository");

        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Success(true));
        _mockRepository.Setup(repo => repo.DeleteByIdAsync(id, default))
            .ReturnsAsync(Result<None>.Failure(error));

        // Act
        var result = await _userService.DeleteByIdAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is DatabaseError);
    }

    #endregion


    #region ExistAsync

    [Fact]
    public async Task ExistAsync_ShouldReturnSuccess_WhenUserExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _userService.ExistAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Response);
    }

    [Fact]
    public async Task ExistAsync_ShouldReturnSuccess_WhenUserDontExist()
    {
        //Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Success(false));
        // Act
        var result = await _userService.ExistAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Response);
    }

    [Fact]
    public async Task ExistAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var error = new DatabaseError("Failed to check Existing User", "User", null, "Simulated error from repository");

        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Failure(error));

        // Act
        var result = await _userService.ExistAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is DatabaseError);
    }

    #endregion
}
using Microsoft.Extensions.Logging;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Services;
using Moq;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Tasks.Models;
using Xunit;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tests.Tasks;

public class TaskServiceUnitTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly ITaskService _taskService;

    public TaskServiceUnitTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _taskService = new TaskService(_mockRepository.Object,
            Mock.Of<ILogger<TaskService>>(),
            Mock.Of<IMediator>());
    }

    #region CreateAsync

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_ShouldReturnSuccess_WhenTaskIsCreated()
    {
        // Arrange
        var validTask = new Task(Guid.NewGuid(), "Test Title", "", null, null, null, null);
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(validTask.Id, default))
            .ReturnsAsync(
                Result<bool>
                    .Success(false)); //We need to do that because in the taskService.CreateAsync, ExistsByIdAsync is called, and we don't do that it will return null.
        _mockRepository.Setup(repo => repo.CreateAsync(validTask, default))
            .ReturnsAsync(Result<Task>.Success(validTask));

        // Act
        var result = await _taskService.CreateAsync(validTask);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validTask, result.Response);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_ShouldReturnValidationError_WhenTaskIsNull()
    {
        // Act
        var result = await _taskService.CreateAsync(null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var validTask = new Task(Guid.NewGuid(), "Test Title", "", null, null, null, null);
        var error = new DatabaseError("Failed to create", "Task", null, "Simulated error from repository");

        _mockRepository.Setup(repo => repo.ExistsByIdAsync(validTask.Id, default))
            .ReturnsAsync(
                Result<bool>
                    .Success(false)); //We need to do that because in the taskService.CreateAsync, ExistsByIdAsync is called, and we don't do that it will return null.
        _mockRepository.Setup(repo => repo.CreateAsync(validTask, default))
            .ReturnsAsync(Result<Task>.Failure(error));

        // Act
        var result = await _taskService.CreateAsync(validTask);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is DatabaseError);
        
    }
    
    #endregion

    #region GetByIdAsync

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnTask_WhenTaskExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedTask = new Task(id, "ExpectedTask", "", null, null, null, null);

        _mockRepository.Setup(repo => repo.GetByIdAsync(id, default))
            .ReturnsAsync(Result<Task>.Success(expectedTask));

        // Act
        var result = await _taskService.GetByIdAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Response.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnNotFoundError_WhenTaskDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Mocking the repository to return a failure when the task does not exist
        _mockRepository.Setup(repo => repo.GetByIdAsync(id, default))
            .ReturnsAsync(Result<Task>.Failure(new NotFoundError("Task", id.ToString())));

        // Act
        var result = await _taskService.GetByIdAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is NotFoundError); // Assuming your service logs this message.
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var error = new DatabaseError("Failed to get task", "Task", null, "Simulated error from repository");
        
        _mockRepository.Setup(repo => repo.GetByIdAsync(id, default))
            .ReturnsAsync(Result<Task>.Failure(error));

        // Act
        var result = await _taskService.GetByIdAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors,e=> e is DatabaseError);
    }
    #endregion

    #region GetAllAsync

    [Fact]
    public async System.Threading.Tasks.Task GetAllAsync_ShouldReturnTasks_WhenFiltersAreApplied()
    {
        // Arrange
        var options = new GetAllTasksOptions
        {
            Title = "Test Task",
            Description = "Test Description",
            Deadline = DateTime.UtcNow,
            Priority = (Priority)1,
            Status = (Status)1,
            Page = 1,
            PageSize = 10
        };
        var expectedTasks = new List<Task>
        {
            new Task(Guid.NewGuid(), "Test Task", "Test Description", DateTime.UtcNow, Priority.Medium,
                Status.InProgress, null)
        };

        _mockRepository.Setup(repo => repo.GetAllAsync(options, default))
            .ReturnsAsync(Result<IEnumerable<Task>>.Success(expectedTasks));

        // Act
        var result = await _taskService.GetAllAsync(options);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Response);
        Assert.Single(result.Response);
        Assert.Equal(expectedTasks[0].Title, result.Response.ToList()[0].Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllAsync_ShouldReturnTasks_WhenNoFiltersAreApplied()
    {
        // Arrange
        var options = new GetAllTasksOptions
        {
            Page = 1,
            PageSize = 10
        };

        var expectedTasks = new List<Task>
        {
            new Task(Guid.NewGuid(), "Test Task 1", "Test Description 1", DateTime.UtcNow, Priority.Medium,
                Status.InProgress, null),
            new Task(Guid.NewGuid(), "Test Task 2", "Test Description 2", DateTime.UtcNow, Priority.Low, Status.Closed,
                null)
        };

        _mockRepository.Setup(repo => repo.GetAllAsync(options, default))
            .ReturnsAsync(Result<IEnumerable<Task>>.Success(expectedTasks));

        // Act
        var result = await _taskService.GetAllAsync(options);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Response);
        Assert.Equal(2, result.Response.Count());
    }
    
    [Fact]
    public async System.Threading.Tasks.Task GetAllAsync_ShouldReturnValidationError_WhenOptionsIsNull()
    {
        // Act
        var result = await _taskService.GetAllAsync(null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var options = new GetAllTasksOptions
        {
            Page = 1,
            PageSize = 10
        };

        var error = new DatabaseError("Failed to retrieve tasks", "Task", null, "Simulated error from repository");

        _mockRepository.Setup(repo => repo.GetAllAsync(options, default))
            .ReturnsAsync(Result<IEnumerable<Task>>.Failure( error));

        // Act
        var result = await _taskService.GetAllAsync(options);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is DatabaseError);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_ShouldReturnSuccess_WhenTaskIsUpdated()
    {
        // Arrange
        var validTask = new Task(Guid.NewGuid(), "Test Title", "", null, null, null, null);
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(validTask.Id, default))
            .ReturnsAsync(Result<bool>.Success(true));
        _mockRepository.Setup(repo => repo.UpdateAsync(validTask, default))
            .ReturnsAsync(Result<Task>.Success(validTask));

        // Act
        var result = await _taskService.UpdateAsync(validTask);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(validTask, result.Response);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_ShouldReturnValidationError_WhenTaskIsNull()
    {
        // Act
        var result = await _taskService.UpdateAsync(null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }
    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var error = new DatabaseError("Failed to update", "Task", null, "Simulated error from repository");
        var validTask = new Task(Guid.NewGuid(), "Test Title", "", null, null, null, null);
        
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(validTask.Id, default))
            .ReturnsAsync(Result<bool>.Success(true));
        _mockRepository.Setup(repo => repo.UpdateAsync(validTask, default))
            .ReturnsAsync(Result<Task>.Failure(error));

        // Act
        var result = await _taskService.UpdateAsync(validTask);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors,e=> e is DatabaseError);
    }
    
    #endregion

    #region DeleteAsync

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_ShouldReturnSuccess_WhenTaskIsDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Success(true));
        _mockRepository.Setup(repo => repo.DeleteByIdAsync(id, default))
            .ReturnsAsync(Result<None>.Success(None.Value));

        // Act
        var result = await _taskService.DeleteByIdAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_ShouldReturnValidationError_WhenTaskDontExist()
    {
        //Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Success(false));
        
        // Act
        var result = await _taskService.DeleteByIdAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is NotFoundError);
    }
    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var error = new DatabaseError("Failed to delete", "Task", null, "Simulated error from repository");
        
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Success(true));
        _mockRepository.Setup(repo => repo.DeleteByIdAsync(id, default))
            .ReturnsAsync(Result<None>.Failure(error));

        // Act
        var result = await _taskService.DeleteByIdAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors,e=> e is DatabaseError);
    }

    #endregion


    #region ExistAsync

    [Fact]
    public async System.Threading.Tasks.Task ExistAsync_ShouldReturnSuccess_WhenTaskExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _taskService.ExistAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Response);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExistAsync_ShouldReturnSuccess_WhenTaskDontExist()
    {
        //Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Success(false));
        // Act
        var result = await _taskService.ExistAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Response);
    }
    [Fact]
    public async System.Threading.Tasks.Task ExistAsync_ShouldReturnDatabaseError_WhenRepositoryReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var error = new DatabaseError("Failed to check Existing Task", "Task", null, "Simulated error from repository");
        
        _mockRepository.Setup(repo => repo.ExistsByIdAsync(id, default))
            .ReturnsAsync(Result<bool>.Failure(error));

        // Act
        var result = await _taskService.ExistAsync(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors,e=> e is DatabaseError);
    }
    #endregion
}
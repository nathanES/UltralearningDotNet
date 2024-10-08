using TaskManagement.Common.Models;
using Xunit;
using System;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tests.Tasks;

public class TaskCreateUnitTests
{
    [Theory]
    [InlineData("Valid Title", "Valid Description", null, null, null, null)]
    [InlineData("Task Title", "Task Description", "2024-12-31", (Priority)1, (Status)1, "f6f4bd94-1b87-431f-b9c7-12d97e245d9e")]
    [InlineData("Empty Description", "", null, null, null, null)]
    public void TaskCreate_ShouldBeCreated_WhenParametersAreValid(string title, string description, string? deadlineString, Priority? priority, Status? status, string? userIdString)
    {
        //Arange
        var id = Guid.NewGuid();
        Guid? userId = string.IsNullOrWhiteSpace(userIdString) ? null : Guid.Parse(userIdString);
        DateTime? deadline = string.IsNullOrWhiteSpace(deadlineString) ? null : DateTime.Parse(deadlineString);
        
        //Act
        Task task = new Task(id, title, description, deadline, priority, status, userId);
        
        //Assert
        Assert.Equal(id, task.Id);
        Assert.Equal(title, task.Title);
        Assert.Equal(description, task.Description);
        Assert.Equal(deadline, task.Deadline);
        Assert.Equal(priority, task.Priority);
        Assert.Equal(status, task.Status);
        Assert.Equal(userId, task.UserId);
    }
    
    [Fact]
    public void TaskCreate_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var emptyTitle = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new Task(id, emptyTitle, "", null, null, null, null)
        );
    }
        
    [Fact]
    public void TaskCreate_ShouldThrowArgumentNullException_WhenTitleIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        string? nullTitle = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Task(id, nullTitle!, "", null, null, null, null)
        );
    } 
    
    [Fact]
    public void TaskCreate_ShouldThrowArgumentNullException_WhenDescriptionIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        string description = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new Task(id, "Valid Title", description!, null, null, null, null)
        );
    } 
}
using TaskManagement.Common.Models;
using Xunit;
using System;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tests.Tasks;

public class TaskUpdateUnitTests
{
    [Theory]
    [InlineData("Valid Title", "Valid Description", null, null, null, null)]
    [InlineData("Task Title", "Task Description", "2024-12-31", (Priority)1, (Status)1, "f6f4bd94-1b87-431f-b9c7-12d97e245d9e")]
    [InlineData("Empty Description", "", null, null, null, null)]
    public void TaskUpdate_ShouldBeUpdated_WhenParametersAreValid(string title, string description, string? deadlineString, Priority? priority, Status? status, string? userIdString)
    {
        //Arange
        var id = Guid.NewGuid();
        Guid? userId = string.IsNullOrWhiteSpace(userIdString) ? null : Guid.Parse(userIdString);
        DateTime? deadline = string.IsNullOrWhiteSpace(deadlineString) ? null : DateTime.Parse(deadlineString);
        Task task = new Task(id, "ToUpdate", "ToUpdate", null, null, null, null);
        
        //Act
        task.UpdateTitle(title);
        task.UpdateDescription(description);
        task.UpdateDeadline(deadline);
        task.UpdatePriority(priority);
        task.UpdateStatus(status);
        task.UpdateUserId(userId);
        
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
    public void TaskUpdate_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var emptyTitle = string.Empty;
        Task task = new Task(id, "ToUpdate", "", null, null, null, null);


        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            task.UpdateTitle(emptyTitle)
        );
    }
        
    [Fact]
    public void TaskUpdate_ShouldThrowArgumentNullException_WhenTitleIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        string? nullTitle = null;
        Task task = new Task(id, "ToUpdate", "", null, null, null, null);


        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            task.UpdateTitle(nullTitle!)
        );
    } 
    
    [Fact]
    public void TaskUpdate_ShouldThrowArgumentNullException_WhenDescriptionIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        string description = null!;
        Task task = new Task(id, "ValidTitle", "", null, null, null, null);


        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            task.UpdateDescription(description)
        );
    } 
}
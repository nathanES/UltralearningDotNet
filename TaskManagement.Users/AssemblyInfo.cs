using System.Runtime.CompilerServices;


[assembly: InternalsVisibleTo("TaskManagement.Tests")] //To allow our Project test to access internal
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] //To allow Moq to access internal
namespace TaskManagement.Users;

public class AssemblyInfo
{
    
}
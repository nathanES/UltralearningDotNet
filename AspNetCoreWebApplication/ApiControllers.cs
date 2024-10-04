using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWebApplication;

public class ApiControllers : ControllerBase
{
    [HttpGet("api/movies1")]
     public async Task<IActionResult> GetAll1( CancellationToken token)
     {
         Console.WriteLine(nameof(GetAll1) + "Start");
         var movieResult = new Movie()
         {
             Title = nameof(GetAll1),
             YearOfRelease = 2024
         };
         Console.WriteLine(nameof(GetAll1) + "End");

         return Ok(movieResult);
     }

    [HttpGet("api/movies2")]
    public async Task<IActionResult> GetAll2(CancellationToken token)
    {
        Console.WriteLine(nameof(GetAll2) + "Start");
        var movieResult = new Movie()
        {
            Title = nameof(GetAll2),
            YearOfRelease = 2024
        };
        Console.WriteLine(nameof(GetAll2) + "End");
        return Ok(movieResult);
    }

    public class Movie
    {
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
    }
}
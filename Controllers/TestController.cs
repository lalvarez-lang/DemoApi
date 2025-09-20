using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoApi.Data;
using DemoApi.Models;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    public TestController(ApplicationDbContext context)
    {
 
    }

 
    [HttpGet]
    public void GetMessage()
    {
        Console.WriteLine("this is a new 1.0.8, edited");
    }    
}
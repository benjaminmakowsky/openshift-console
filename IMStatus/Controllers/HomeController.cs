using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using IMStatus.Models;

namespace IMStatus.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger){
        _logger = logger;
    }

    public IActionResult Index(){
        return View();
    }

    public IActionResult Privacy(){
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(){
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> ChangeTextButton_Click(string token, string testing_url)
    {
        string result;

        try {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(testing_url);
            int statusCode = (int)response.StatusCode;
            result = 
                $"Token: {token}\n" +
                $"Url: {testing_url}\n" +
                $"HTTP Status Code: {statusCode}";
        }
        catch (Exception ex)
        {
            result = $"Unexpected error: {ex.Message}";
        }

        ViewBag.Result = result;
        return View("Index");
    }
}
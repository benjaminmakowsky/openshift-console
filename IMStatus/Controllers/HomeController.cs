using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using IMStatus.Models;
using Newtonsoft.Json;

namespace IMStatus.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly OpenshiftDbContext _openshiftDbContext;

    //Minimum required values for accessing OpenShift
    private string bearerToken = "";
    private string rootURL = "";
        
        
    public HomeController(ILogger<HomeController> logger, OpenshiftDbContext openshiftDbContext){
        _logger = logger;
        _openshiftDbContext = openshiftDbContext;
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

        
        bearerToken = token;
        rootURL = testing_url;  
        
        
        
        // Create a handler that skips certificate validation
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        
        try {
            //Attempt to reach openshift with the bearer token
            using var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
            
            //Attempt to parse response as JSON object
            var response = await client.GetAsync(testing_url);
            HttpStatusCode statusCode = response.StatusCode;

            //If valid response attempt to create db
            if (statusCode == HttpStatusCode.OK) {
                string responseString = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                if (jsonObject != null)
                {
                    //Save each project into the DB
                    foreach (dynamic item in jsonObject.items)
                    {
                        _openshiftDbContext.Add(Project.FromJson(item));
                    } 
                    await _openshiftDbContext.SaveChangesAsync();
                }
            }
            //Report the status code of the URL
            result = $"Url: {rootURL} HTTP Status Code: {statusCode}\n";
        }
        catch (Exception ex)
        {
            result = $"Unexpected error: {ex.Message}";
        }

        //Reload the webpage with the db and the result of the rest call
        List<Project> projectsList = _openshiftDbContext.Projects.ToList();
        ViewBag.Result = result;
        return View("Index", projectsList);
    }

    /// <summary>
    /// Loads up the project view passing in the name of the project to analyze. This is called from the table displaying
    /// available projects within openshift.
    /// </summary>
    /// <param name="id">The id of the project in the database to fetch</param>
    /// <returns></returns>
    public IActionResult LoadProject(int id)
    {
        var dbProject = _openshiftDbContext.Projects.SingleOrDefault(project => project.Id == id);
        if (dbProject != null)
        {
            Console.WriteLine(dbProject);
            return View("~/Views/Project/Index.cshtml", dbProject);
        }

        return View("~/Views/Project/Index.cshtml");
    }
    
    
    public static async Task StartBuildAsync(string buildConfigName, string namespaceName, string envKey, string envValue, string token)
    {
        /*client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var requestUri = $"https://openshift.example.com/apis/build.openshift.io/v1/namespaces/{namespaceName}/buildconfigs/{buildConfigName}/instantiate";
        var content = new StringContent($"{{ \"env\": [{{ \"name\": \"{envKey}\", \"value\": \"{envValue}\" }}] }}", System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync(requestUri, content);
        response.EnsureSuccessStatusCode();

        Console.WriteLine("Build started successfully.");*/
    }
}
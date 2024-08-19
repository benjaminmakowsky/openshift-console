using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using IMStatus.Models;
using Newtonsoft.Json;

namespace IMStatus.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly OpenshiftDbContext _openshiftDbContext;
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

        // Create a handler that skips certificate validation
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        try {
            using var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(testing_url);
            string responseString = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseString);
            string responseKind = jsonObject.kind;
            int statusCode = (int)response.StatusCode;
            HashSet<Project> projects = new HashSet<Project>();
            
            foreach (var item in jsonObject.items)
            {
                Project project = new Project();
                project.Title = item.metadata.name;
                project.CreationDate = item.metadata.creationTimestamp;
                project.Status = item.status.phase;
                _openshiftDbContext.Add(project);
                await _openshiftDbContext.SaveChangesAsync();
                projects.Add(project);
            }

            result =
                $"Token: {token}\n" +
                $"Url: {testing_url}\n" +
                $"HTTP Status Code: {statusCode}\n" +
                $"Response Kind: {responseKind}\n";

        }
        catch (Exception ex)
        {
            result = $"Unexpected error: {ex.Message}";
        }

        List<Project> projectsList = _openshiftDbContext.Projects.ToList();
        ViewBag.Result = result;
        return View("Index", projectsList);
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
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAdminInterface _admin;

    public HomeController(ILogger<HomeController> logger,IAdminInterface admin)
    {
        _admin = admin;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<List<t_Class>> GetAllClass(){
        List<t_Class> tc = await _admin.GetAllClass();
        return tc;
    }

    public async Task<List<t_subjects>> GetAllSubjects(){
        List<t_subjects> ts = await _admin.GetAllSubjects();
        return ts;
    }

    public async Task<List<t_teacherGet>> GetAllTeachers(){
        List<t_teacherGet> tg = await _admin.GetAllTeachers();
        return tg;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

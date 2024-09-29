using FakeUserDataGenerator.Models;
using FakeUserDataGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CsvHelper;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

namespace FakeUserDataGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserDataService _userDataService;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(ILogger<HomeController> logger, UserDataService userDataService
            , IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _userDataService = userDataService;
            _localizer = localizer;
        }

        public IActionResult Index()
        {

            ViewData["Message"] = _localizer["WelcomeMessage"];
            return View();
        }

        [HttpGet]
        public JsonResult GetUserRecords(string region, int seed, int errorCount, int page, int pageSize = 20)
        {
            var records = _userDataService.GenerateUserRecords(region, seed, errorCount, page, pageSize);
            return Json(records);
        }

        [HttpPost]
        public IActionResult TranslateTable()
        {
            var translatedTableHeaders = new
            {
                Name = _localizer["Name"],
                Address = _localizer["Address"],
                Phone = _localizer["Phone"]
            };

            return Json(translatedTableHeaders);
        }

        [HttpGet]
        public IActionResult ExportToCSV(string region, int seed, int errorCount)
        {
            var records = _userDataService.GenerateUserRecords(region, seed, errorCount, 0, 100); // Example of 100 records

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return File(memoryStream.ToArray(), "text/csv", "user_data.csv");
            }
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
}

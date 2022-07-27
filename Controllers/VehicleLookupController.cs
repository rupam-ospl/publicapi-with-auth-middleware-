using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OpenAPI3.Controllers;

[ApiController]
[Route("[controller]")]
public class VehicleLookupController : ControllerBase
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<VehicleLookupController> _logger;

    public VehicleLookupController(ILogger<VehicleLookupController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetVehicleByRegistrationNumber")]
    public Vehicle Get(string vehicleReg)
    {
        _logger.LogInformation($"Entered VehicleLookupController with argument {vehicleReg}");
        string _filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        _logger.LogInformation($"FilePath {_filePath}");
        StreamReader r = new StreamReader(_filePath + "/json.json");

        var vehicle = JsonConvert.DeserializeObject<Vehicle>(r.ReadToEnd());

        _logger.LogInformation($"Returning Vehile Data response {vehicle}");

        return vehicle;
    }

    [HttpPost(Name = "PostFormData")]
    public string PostForm(FormData formData)
    {
       // _logger.LogInformation($"Entered VehicleLookupController with argument {vehicleReg}");
        //string _filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        //_logger.LogInformation($"FilePath {_filePath}");
        //StreamReader r = new StreamReader(_filePath + "/json.json");

       // var vehicle = JsonConvert.DeserializeObject<Vehicle>(r.ReadToEnd());

        //_logger.LogInformation($"Returning Vehile Data response {vehicle}");

        //return vehicle;
        return "Ok";
    }

}

public class FormData
{
    public string FormName { get; set; }
    public string FormVersion { get; set; }

    public Dictionary<string,string> FormKeyValues
    {
        get;
        set;

    }
}

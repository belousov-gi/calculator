using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Extesions;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private BusinessLogic _businessLogic;
    

    public HomeController(ILogger<HomeController> logger, BusinessLogic businessLogic)
    {
        _logger = logger;
        _businessLogic = businessLogic;
    }
    
    public IActionResult Index()
    {
        var results = SessionManager.Get<SessionCalculationResultsModel>(HttpContext.Session, "sessionCalcResults");
        return View(results);
    }
    
    [HttpPost]
    public IActionResult Index( SessionCalculationResultsModel calculationParamsModel)
    {
        var results =
            SessionManager.Get<SessionCalculationResultsModel>(HttpContext.Session, "sessionCalcResults");
        
        try
        {
            var validateAnswer = ValidateInputData(calculationParamsModel.InputString);
            if (validateAnswer.IsValidationPassed)
            {
                if (ModelState.IsValid)
                {
                    var inputString = calculationParamsModel.InputString.Replace(" ", "");
                    var result = _businessLogic.StartСalculation(inputString);
                    var resultModel = new CalculationResultModel()
                    {
                        Result = result,
                        InputString = inputString
                    };
                
                    results.sessionCalcResults.Add(resultModel);
                    SessionManager.Set(HttpContext.Session, "sessionCalcResults", results);
                    return View(results);
                }
            }

            ViewBag.Message = validateAnswer.ValidationFailInfo;
            return View(results);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ViewBag.Message = e.Message;
            return View(results);
        }
    }

    //TODO: разобраться как прокидывать кастомные ошибки во вьюхи
    [NonAction]
    public ValidateAnswerModel ValidateInputData(string data)
    {
        if (data == null)
        {
            return new ValidateAnswerModel(false, "Введите выражение");
        }
    
        else
        {
            Regex regex = new Regex(@"[^0-9+\-\/*^ (),]");
            MatchCollection matches = regex.Matches(data);
            if (matches.Count > 0)
            {
                return new ValidateAnswerModel(false, $@"Найдены неподдерживаемые символы, например: {matches[0].Value}");
            }
           
            
            regex = new Regex(@"[\d]");
            matches = regex.Matches(data);
            if (matches.Count < 1)
            {
                return new ValidateAnswerModel(false, $@"Не введено ниодного числа");
            }
            
            regex = new Regex(@"(?=([*+-\/^]))\1{2,}");
            matches = regex.Matches(data);
            if (matches.Count > 1)
            {
                return new ValidateAnswerModel(false, $@"Найдены повторяющиеся операторы");
            }
        }
        return new ValidateAnswerModel(true);
    }
 

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
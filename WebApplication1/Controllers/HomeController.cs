using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private CalculatorLogic _calculatorLogic;
    private ISessionsStorage _sessionsStorage;
    public HomeController(CalculatorLogic calculatorLogic, ISessionsStorage sessionsStorage)
    {
        _calculatorLogic = calculatorLogic;
        _sessionsStorage = sessionsStorage;
    }
    public IActionResult Index()
    {
        var results = _sessionsStorage.Get();
        return View(results);
    }
    
    [HttpPost]
    public IActionResult Index(SessionCalculationResultsModel calculationParamsModel)
    {
        try
        {
            ValidateInputData(calculationParamsModel.InputString);
            var inputString = calculationParamsModel.InputString.Replace(" ", "");
            var result = _calculatorLogic.Calculate(inputString);
            var resultModel = new CalculationResultModel()
            {
                Result = result,
                InputString = inputString
            };
            _sessionsStorage.AddResult(resultModel);
            return Redirect("/");
        }

        catch (ValidationException e)
        {
            ViewBag.Message = e.Message;
            return View(_sessionsStorage.Get());
        }

        catch (Exception)
        {
            ViewBag.Message = "Произошла ошибка при вычислениях";
            return View(_sessionsStorage.Get());
        }
    }

    //TODO: разобраться как прокидывать кастомные ошибки во вьюхи
    [NonAction]
    public void ValidateInputData(string data)
    {
        if (data == null)
        {
            throw new ValidationException("Введите выражение");
        }
        
        Regex regex = new Regex(@"[^0-9+\-\/*^ (),]");
        MatchCollection matches = regex.Matches(data);
        if (matches.Count > 0)
        {
            throw new ValidationException($"Найдены неподдерживаемые символы, например: {matches[0].Value}");
        }
       
        
        regex = new Regex(@"[\d]");
        matches = regex.Matches(data);
        if (matches.Count < 1)
        {
            throw new ValidationException("Не введено ниодного числа");
        }
        
        regex = new Regex(@"(?=([*+-\/^]))\1{2,}");
        matches = regex.Matches(data);
        if (matches.Count > 1)
        {
            throw new ValidationException("Найдены повторяющиеся операторы");
        }
    }
}
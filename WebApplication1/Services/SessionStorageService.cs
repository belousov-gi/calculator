using System.Text.Json;
using WebApplication1.Enums;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class SessionStorageService : ISessionsStorage
{
    private readonly TypeOfSorting _typeOfSorting;
    private readonly string _resultsKeyInJSON;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionStorageService(IHttpContextAccessor httpContextAccessor)
    {
        _typeOfSorting = Enum.Parse<TypeOfSorting>(Environment.GetEnvironmentVariable("ResultsOrder") ?? "ACS");
        _httpContextAccessor = httpContextAccessor;
        _resultsKeyInJSON = "sessionCalcResults";
    }

    public void AddResult(CalculationResultModel resultModel)
    {
        ISession session = _httpContextAccessor.HttpContext.Session;   
        var results = Get();
        results.sessionCalcResults.Add(resultModel);
        session.SetString(_resultsKeyInJSON, JsonSerializer.Serialize(results));
    }
    
    public void Set(SessionCalculationResultsModel value)
    {
        ISession session = _httpContextAccessor.HttpContext.Session;  
        session.SetString(_resultsKeyInJSON, JsonSerializer.Serialize(value));
    }
 
    public SessionCalculationResultsModel Get()
    {
        ISession session = _httpContextAccessor.HttpContext.Session;   
        string value;
        string key = _resultsKeyInJSON;
        if (session.Keys.Contains(key))
        {
            value = session.GetString(key);
        }
        else
        {
            Set(new SessionCalculationResultsModel());
            value = session.GetString(key);
        }

        var model = JsonSerializer.Deserialize<SessionCalculationResultsModel>(value);
        
        if (_typeOfSorting == TypeOfSorting.ACS)
        {
            SessionCalculationResultsModel resultsModel = new SessionCalculationResultsModel
            {
                InputString = model.InputString,
                sessionCalcResults = new List<CalculationResultModel>(model.sessionCalcResults)
            };
            resultsModel.SortResultsAsc();
            return resultsModel;
        }
        return model; 
    }
}
using System.Text.Json;
using WebApplication1.Enums;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class SessionStorageService : ISessionsStorage
{
    public void Set(ISession session, string key, SessionCalculationResultsModel value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }
 
    public SessionCalculationResultsModel Get(ISession session, string key)
    {
        string value;
        if (session.Keys.Contains("sessionCalcResults"))
        {
            value = session.GetString(key);
        }
        else
        {
            Set(session, "sessionCalcResults", new SessionCalculationResultsModel());
            value = session.GetString(key);
        }

        var model = JsonSerializer.Deserialize<SessionCalculationResultsModel>(value);
        return model; 
    }

    public SessionCalculationResultsModel Get(ISession session, string key, TypeOfSorting typeOfSorting)
    {
        var model = Get(session, key);
        if (typeOfSorting == TypeOfSorting.ACS)
        {
            SessionCalculationResultsModel resultsModel = new SessionCalculationResultsModel()
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
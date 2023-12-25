using WebApplication1.Enums;
using WebApplication1.Models;

namespace WebApplication1.Interfaces;

public interface ISessionsStorage
{
     void AddResult(CalculationResultModel value);
     // void AddResult(ISession session, string key, CalculationResultModel value);
     SessionCalculationResultsModel Get();
}
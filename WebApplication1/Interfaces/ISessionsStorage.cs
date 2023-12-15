using WebApplication1.Enums;
using WebApplication1.Models;

namespace WebApplication1.Interfaces;

public interface ISessionsStorage
{
     void Set(ISession session, string key, SessionCalculationResultsModel value);
     SessionCalculationResultsModel Get(ISession session, string key);
     SessionCalculationResultsModel Get(ISession session, string key, TypeOfSorting typeOfSorting);
    
     

}
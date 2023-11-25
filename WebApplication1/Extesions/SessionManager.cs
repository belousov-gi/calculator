using WebApplication1.Models;

namespace WebApplication1.Extesions;

using System.Text.Json;
 
public static class SessionManager
{
    public static void Set<T>(this ISession session, string key, T value)
    {
       session.SetString(key, JsonSerializer.Serialize<T>(value));
    }
 
    public static T? Get<T>(this ISession session, string key)
    {
        string value = "";
        if (session.Keys.Contains("sessionCalcResults"))
        {
            value = session.GetString(key);
        }
        else
        {
            Set(session, "sessionCalcResults", new SessionCalculationResultsModel());
            value = session.GetString(key);
        }
        
        // return value == "" ? default(T) : JsonSerializer.Deserialize<T>(value); 
        return JsonSerializer.Deserialize<T>(value); 
    }
}
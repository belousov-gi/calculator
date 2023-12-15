using System.ComponentModel.DataAnnotations;


namespace WebApplication1.Models;

public class SessionCalculationResultsModel 
{
    public string InputString { get; set; }
    public List<CalculationResultModel> sessionCalcResults { get; set; } = new List<CalculationResultModel>();

    public void SortResultsAsc()
    {
        sessionCalcResults.Reverse();
    }
}
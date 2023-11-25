using System.ComponentModel.DataAnnotations;


namespace WebApplication1.Models;

public class SessionCalculationResultsModel 
{
    [Required(ErrorMessage = "Введите выражение")]
    public string InputString { get; set; }
    public List<CalculationResultModel> sessionCalcResults { get; set; } = new List<CalculationResultModel>();
    
 
}
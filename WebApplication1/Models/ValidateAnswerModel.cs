namespace WebApplication1.Models;
//для кастомной валидации
public class ValidateAnswerModel
{
    public bool IsValidationPassed { get; init; }
    public string ValidationFailInfo { get; init; }

    public ValidateAnswerModel(bool isValidationPassed)
    {
        IsValidationPassed = isValidationPassed;
    }
    
    public ValidateAnswerModel(bool isValidationPassed, string validationFailInfo)
    {
        IsValidationPassed = isValidationPassed;
        ValidationFailInfo = validationFailInfo;
    }
}
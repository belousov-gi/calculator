using System.ComponentModel.DataAnnotations;
using WebApplication1.Enums;

namespace WebApplication1;

public class CalculatorLogic
{
    //TODO: сделать обработку деления на 0 через exception
    //TODO: сделать возможность использования не только целых чисел
    public double Calculate( string expression)
    {
        string postfixExpr = ToPostfix(expression + "\r");
        double result = CalcPostfix(postfixExpr);
        return result;
    }

    private Dictionary<char, int> operationPriority = new() {
        {'(', 0},
        {'+', 1},
        {'-', 1},
        {'*', 2},
        {'/', 2},
        {'^', 3},
        {'~', 4}	//	Унарный минус
    };
  
    //TODO: возможэно стоит поменять double на decimal, точность будет выше
    //Получаем число
    private string GetStringNumber(string expr, ref int pos)
    {
        string strNumber = "";

        for (; pos < expr.Length; pos++)
        {
            char num = expr[pos];

            //	Проверяем, является символ числом
            if (Char.IsDigit(num) || num == ',')
                //	Если да - прибавляем к строке
                strNumber += num;
            else
            {
                //	Если нет, то перемещаем счётчик к предыдущему символу
                pos--;
                break;
            }
        }

        //	Возвращаем число
        return strNumber;
    }
    
    private string ToPostfix(string infixExpr)
    {
        //	Выходная строка, содержащая постфиксную запись
        string postfixExpr = "";
        //	Инициализация стека, содержащий операторы в виде символов
        Stack<char> stack = new();
        Stack<char> braketsValidationStack = new();
        TypeOfChar? previusChar = null;

        int LengthOfExpr = infixExpr.Length;
        for (int i = 0; i < LengthOfExpr; i++)
        {
            char c = infixExpr[i];
          
            if (Char.IsDigit(c))
            {
                //TODO:добавить обработку наличия операторов нормальную
               
                //	Парсии его, передав строку и текущую позицию, и заносим в выходную строку
                postfixExpr += GetStringNumber(infixExpr, ref i) + " ";
                ValidateInputData(TypeOfChar.numeric, previusChar);
                previusChar = TypeOfChar.numeric;
                
            }
           
            else if (c == '(')
            {
                if (previusChar == TypeOfChar.sequenceOperator || i == 0 || c == '(')
                {
                    stack.Push(c);
                    braketsValidationStack.Push(c);
                }
            }
           
            else if (c == ')')
            {
                if (!braketsValidationStack.Any())
                {
                    throw new ValidationException("Невалидная последовательность скобок");
                }
                
                int stackCount = stack.Count;
                //	Заносим в выходную строку из стека всё вплоть до открывающей скобки
                while (stackCount > 0 && stack.Peek() != '(')
                {
                    postfixExpr += stack.Pop();                        
                }
                
                //	Удаляем открывающуюся скобку из стека
                stack.Pop();
                braketsValidationStack.Pop();
            }
            
            else if (operationPriority.ContainsKey(c))
            {
                ValidateInputData(TypeOfChar.sequenceOperator, previusChar);
                previusChar = TypeOfChar.sequenceOperator;
                
                char op = c;
                
                //	Если да, то сначала проверяем является ли оператор унарным символом
                if (op == '-' && (i == 0 || (i > 1 && operationPriority.ContainsKey(infixExpr[i - 1]))))
                {
                    op = '~';
                }
			
                //	Заносим в выходную строку все операторы из стека, имеющие более высокий приоритет
                while (stack.Count > 0 && operationPriority[stack.Peek()] >= operationPriority[op])
                {
                    postfixExpr += stack.Pop();
                }
                    
                //	Заносим в стек оператор
                stack.Push(op);
            }
        }
        
        //	Заносим все оставшиеся операторы из стека в выходную строку
        foreach (char op in stack)
        {
            postfixExpr += op;
        }
        
        if (braketsValidationStack.Any())
        {
            throw new ValidationException("Невалидная последовательность скобок");
        }
        
        return postfixExpr;
    }

    private double CalcualateExpr(char op, double first, double second)
    {
        switch (op)
        {
            case '+': return first + second;
            case '-': return first - second;
            case '*': return first * second;
            case '/':
            {
                if (second != 0)
                {
                    return first / second;
                }

                throw new DivideByZeroException();
            }
            case '^': return Math.Pow(first, second);
            default: throw new ValidationException("Невалидный оператор");
        }
        
    }
    
    public double CalcPostfix(string postfixExpr)
    {
        //	Стек для хранения чисел
        Stack<double> locals = new();
       
        //	Счётчик действий
        int counter = 0;
        
        for (int i = 0; i < postfixExpr.Length; i++)
        {
            char c = postfixExpr[i];
			
            if (Char.IsDigit(c))
            {
                string number = GetStringNumber(postfixExpr, ref i);
                
                //	Заносим в стек, преобразовав из String в Double-тип
                locals.Push(Convert.ToDouble(number));
            }
           
            else if (operationPriority.ContainsKey(c))
            {
                counter += 1;
                if (c == '~')
                {
                    //	Проверяем, пуст ли стек: если да - задаём нулевое значение,
                    //	еси нет - выталкиваем из стека значение
                    double last = locals.Count > 0 ? locals.Pop() : throw new ValidationException("Невалдиная последовательность операторов");

                    //	Получаем результат операции и заносим в стек
                    locals.Push(CalcualateExpr('-', 0, last));
                    continue;
                }
					        
                //	Получаем значения из стека в обратном порядке
                double second = locals.Count > 0 ? locals.Pop() : throw new ValidationException("Невалдиная последовательность операторов"),
                    first = locals.Count > 0 ? locals.Pop() : throw new ValidationException("Невалдиная последовательность операторов");
					        
                //	Получаем результат операции и заносим в стек
                locals.Push(CalcualateExpr(c, first, second));
            }
        }
        //	По завершению цикла возвращаем результат из стека
        return locals.Pop();
    }

    private void ValidateInputData(TypeOfChar currentChar, TypeOfChar? previousChar)
    {
        if (currentChar == TypeOfChar.sequenceOperator)
        {
            if (previousChar == TypeOfChar.sequenceOperator)
            {
                throw new ValidationException("Невалидная последовательность операторов");
            }
        }
        
        if (currentChar == TypeOfChar.numeric)
        {
            if (previousChar == TypeOfChar.numeric)
            {
                throw new ValidationException("Между числами должен быть оператор");
            }  
        }
    }
}
namespace WebApplication1;

public class BusinessLogic
{
    //TODO: сделать обработку деления на 0 через exception
    //TODO: сделать возможность использования не только целых чисел
    public double StartСalculation( string expression)
    {
        Calculator calculator = new(expression);
        return calculator.Calc();
    }
    
    public class Calculator
    {
        //	Хранит инфиксное выражение
        public string infixExpr {get; private set; }
        
        //	Хранит постфиксное выражение
        public string postfixExpr { get; private set; }

        private Dictionary<char, int> operationPriority = new() {
            {'(', 0},
            {'+', 1},
            {'-', 1},
            {'*', 2},
            {'/', 2},
            {'^', 3},
            {'~', 4}	//	Унарный минус
        };
        public Calculator(string expression)
        {
            infixExpr = expression;
            postfixExpr = ToPostfix(infixExpr + "\r");
        }
        
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
            bool IsPreviousValOperator = false; 
            
            for (int i = 0; i < infixExpr.Length; i++)
            {
                char c = infixExpr[i];
              
                if (Char.IsDigit(c))
                {
                    //TODO:добавить обработку наличия операторов нормальную
                   
                    //	Парсии его, передав строку и текущую позицию, и заносим в выходную строку
                    postfixExpr += GetStringNumber(infixExpr, ref i) + " ";
                    IsPreviousValOperator = false;
                    
                }
               
                else if (c == '(')
                {
                    if (IsPreviousValOperator || i == 0)
                    {
                        stack.Push(c);
                        IsPreviousValOperator = false; 
                    }
                    else
                    {
                        throw new Exception("Укажите перед скобками оператор");
                    }
                
                }
               
                else if (c == ')')
                {
                    int stackCount = stack.Count;
                    //	Заносим в выходную строку из стека всё вплоть до открывающей скобки
                    while (stackCount > 0 && stack.Peek() != '(')
                    {
                        if (stackCount == 1)
                        {
                            throw new Exception("Неверно расставлены скобки");
                        }
                        
                        postfixExpr += stack.Pop();
                        
                    }
                    
                    //	Удаляем открывающуюся скобку из стека
                    stack.Pop();
                    IsPreviousValOperator = false;
                }
                
                else if (operationPriority.ContainsKey(c))
                {
                    if (IsPreviousValOperator)
                    {
                        throw new Exception("Два оператора подряд. Используйте скобки при необходимости");
                    }
                    IsPreviousValOperator = true;
                    
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
                default: throw new Exception("Invalid operator");
            }
            
        }
        
        public double Calc()
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
                        double last = locals.Count > 0 ? locals.Pop() : 0;

                        //	Получаем результат операции и заносим в стек
                        locals.Push(CalcualateExpr('-', 0, last));
                        continue;
                    }
					            
                    //	Получаем значения из стека в обратном порядке
                    double second = locals.Count > 0 ? locals.Pop() : 0,
                    first = locals.Count > 0 ? locals.Pop() : 0;
					            
                    //	Получаем результат операции и заносим в стек
                    locals.Push(CalcualateExpr(c, first, second));
                }
            }
            //	По завершению цикла возвращаем результат из стека
            return locals.Pop();
        }
    }
}
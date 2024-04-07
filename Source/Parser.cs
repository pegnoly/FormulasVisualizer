using FormulasVisualizer.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace FormulasVisualizer.Source {

    /// <summary>
    /// Содержит информацию о формуле, разбитую на необходимые для вычислений элементы.
    /// </summary>
    public class FormulaElements {
        /// <summary>
        /// Имя результата вычислений формулы(иначе говоря, имя оси Y на графике)
        /// </summary>
        public string? _result = String.Empty;
        /// <summary>
        /// Текущие значения переменных формулы.
        /// </summary>
        public Dictionary<string, double>? _variables = new Dictionary<string, double>();
        /// <summary>
        /// Имя и значения основной переменной формулы.
        /// </summary>
        public KeyValuePair<string, List<double>> _mainVariable = new KeyValuePair<string, List<double>>();
        /// <summary>
        /// Список выражений, которые должны быть посчитаны в процессе вычисления значения формулы.
        /// </summary>
        public List<Expression> _expressions = new List<Expression>();

        /// <summary>
        /// Назначает новое значение переменной
        /// </summary>
        /// <param name="name">Имя переменной</param>
        /// <param name="value">Новое значение переменной</param>
        public void UpdateVariable(string name, double value) {
            _variables![name] = value;
        }

        public void PrintInfo() {
            Trace.WriteLine($"Result {_result}");
            Trace.WriteLine("Variables: ");
            foreach(KeyValuePair<string, double> kvp in _variables!) {
                Trace.WriteLine($"{kvp.Key} = {kvp.Value}");
            }
            Trace.WriteLine("Expressions: ");
            foreach(Expression expression in _expressions) {
                expression.Print();
            }
        }
    }

    /// <summary>
    /// Некоторый общий функционал, который используется при парсинге
    /// </summary>
    public static class ParserExtensions {

        /// <summary>
        /// Возможные приоритеты операторов
        /// </summary>
        private enum OperatorPriority {
            PRIORITY_FIRST,
            PRIORITY_SECOND,
            PRIORITY_THIRD
        }

        /// <summary>
        /// Символы чисел
        /// </summary>
        private static IReadOnlyList<char> _numbers = new List<char>() {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '0'
        };

        /// <summary>
        /// Символы операторов
        /// </summary>
        private static IReadOnlyList<char> _operators = new List<char> {
            '=', '*', '/', '^', '-', '+'
        };

        /// <summary>
        /// Соответствие операторов их приоритетам
        /// </summary>
        private static IReadOnlyDictionary<char, OperatorPriority> _operatorsPriority = new Dictionary<char, OperatorPriority>() {
            ['='] = OperatorPriority.PRIORITY_FIRST,
            ['^'] = OperatorPriority.PRIORITY_FIRST,
            ['*'] = OperatorPriority.PRIORITY_SECOND,
            ['/'] = OperatorPriority.PRIORITY_SECOND,
            ['+'] = OperatorPriority.PRIORITY_THIRD,
            ['-'] = OperatorPriority.PRIORITY_THIRD
        };
        
        /// <summary>
        /// Определяет, является ли оператором символ.
        /// </summary>
        /// <param name="c">Проверяемый символ</param>
        /// <returns>
        /// true, если символ является оператором, иначе false
        /// </returns>
        public static bool IsOperator(char c) {
            return _operators.Contains(c);
        }

        /// <summary>
        /// Определяет, является ли оператором строка.
        /// </summary>
        /// <param name="s">Проверяемая строка</param>
        /// <returns>
        /// true, если в строке 1 символ и это один из возможных символов операторов, иначе false
        /// </returns>
        public static bool IsOperator(string s) {
            return (s.Length == 1 && _operators.Any(x => s.Contains(x)));
        }

        /// <summary>
        /// Определяет, является ли числом строка.
        /// </summary>
        /// <param name="s">Проверяемая строка</param>
        /// <returns>true, если строка является числом, иначе false</returns>
        public static bool IsNumber(string s) {
            return s.All(x => _numbers.Contains(x));
        }

        /// <summary>
        /// Определяет, является ли первый оператор более приоритетным или равным по приоритету второму.
        /// </summary>
        /// <param name="op1">Первый проверяемый оператор</param>
        /// <param name="op2">Второй проверяемый оператор</param>
        /// <returns>true, если первый оператор имеет больший или равный приоритет, чем второй, иначе false</returns>
        public static bool IsPriorOrEqualOperator(char op1, char op2) {
            return _operatorsPriority[op1] <= _operatorsPriority[op2];
        }
    }

    /// <summary>
    /// Часть выражения. Поскольку в формуле могут быть, как цифры, так и переменные, записанные в виде строк, используется такая формула.
    /// </summary>
    public class ExpressionElement {
        /// <summary>
        /// Имя элемента
        /// </summary>
        public string? _name { get; }
        /// <summary>
        /// Значение элемента
        /// </summary>
        public double _value { get; set; }

        /// <summary>
        /// Создает элемент из имени и значения.
        /// </summary>
        /// <param name="name">Имя элемента</param>
        /// <param name="value">Значение элемента</param>
        public ExpressionElement(string? name, double value) {
            _name = name;
            _value = value;
        }

        /// <summary>
        /// Создает элемент из заданной строки. Если эта строка - число, то она интерпретируется, как значение этого элемента, если нет, то как имя.
        /// </summary>
        /// <param name="definition">Заданная заранее неопределенная строка</param>
        public ExpressionElement(string definition) {
            bool isNumber = ParserExtensions.IsNumber(definition);
            _name = isNumber ? String.Empty : definition;
            _value = isNumber ? Convert.ToDouble(definition) : Double.NegativeInfinity;
        }
    }

    /// <summary>
    /// Выражение, содержащее левый и правый элементы, связывающий их оператор, а также имя элемента, который будет произведен в результате вычисления этого выражения.
    /// </summary>
    public class Expression {
        /// <summary>
        /// Левый элемент выражения
        /// </summary>
        public ExpressionElement? _left { get; }
        /// <summary>
        /// Правый элемент выражения
        /// </summary>
        public ExpressionElement? _right { get; }
        /// <summary>
        /// Оператор, связывающий элементы выражения.
        /// </summary>
        public char _operator { get; }
        /// <summary>
        /// Имя производного элемента.
        /// </summary>
        public string _outValueName { get; }

        public Expression(ExpressionElement? left, ExpressionElement? right, char @operator, string outValueName) {
            _left = left;
            _right = right;
            _operator = @operator;
            _outValueName = outValueName;
        }

        public void Print() {
            string left = String.IsNullOrEmpty(_left!._name) ? _left._value.ToString() : _left._name;
            string right = String.IsNullOrEmpty(_right!._name) ? _right._value.ToString() : _right._name;
            Trace.WriteLine($"{left}{_operator}{right}={_outValueName}");
        }
    }

    /// <summary>
    /// Создает выражения в нужном порядке, используя поступающую из парсера информацию.
    /// </summary>
    public class ExpressionConstructor {
        /// <summary>
        /// Связный список, в который записывается последовательность, в которой значения поступают из парсера.
        /// </summary>
        private LinkedList<string> _variablesList = new LinkedList<string>();
        /// <summary>
        /// Текущее число созданных выражений.
        /// </summary>
        private int _expressionsCount = 0;

        public delegate void ExpressionConstructed(Expression expression);
        /// <summary>
        /// Вызывается, когда новое выражение создано.
        /// </summary>
        public event ExpressionConstructed? OnExpressionConstructed;

        /// <summary>
        /// Вызывается, чтобы принять из парсера строку, которая была интерпретирована, как переменная выражения. Вставляется в конец списка.
        /// </summary>
        /// <param name="variable">Строка из парсера</param>
        public void AddVariable(string variable) { 
            _variablesList.AddLast(variable);
        }

        /// <summary>
        /// Вызывается, чтобы принять из парсера символ, который был интерпретирован, как оператор.
        /// Поскольку парсер разбирает исходную строку формулы, используя обратную польскую запись, то:
        /// 1)Если два предыдущих значения, принятых этим объектом, были переменными, значит, можно 
        /// 2)Создать выражение из этих двух значений и поступившего оператора.
        /// 3)После этого убрать два последних значения из списка и записать в него имя производного значения созданного выражения.
        /// (сложна, но лучше я чето не придумал)
        /// </summary>
        /// <param name="op"></param>
        public void AddOperator(char op) {
            LinkedListNode<string> lastNode = _variablesList.Last!;
            LinkedListNode<string> prevNode = lastNode.Previous!;
            Trace.WriteLine($"adding operator {op} with prev node {prevNode.Value} and last node {lastNode.Value}");
            // 1.
            if (!ParserExtensions.IsOperator(lastNode.Value) && !ParserExtensions.IsOperator(prevNode.Value)) {
                // 2.
                _expressionsCount++;
                string result = $"EXPRESSION_RESULT_{_expressionsCount}";
                Expression expression = Create(prevNode.Value, lastNode.Value, op, result);
                OnExpressionConstructed!(expression);
                // 3.
                _variablesList.RemoveLast();
                _variablesList.Last!.Value = result;
            }
            else {
                _variablesList.AddLast(op.ToString());
            }
        }

        private Expression Create(string left, string right, char op, string outval) {
            return new Expression(
                left: new ExpressionElement(left),
                right: new ExpressionElement(right),
                op,
                outval
            );
        }
    }
    /*
     * Производит парсинг строки с формулой, используя обратную польскую запись.
     */
    public class FormulaParser {
        /// <summary>
        /// Ссылка на объект с полной информацией о формуле.
        /// </summary>
        private FormulaElements? _formulaElements;
        /// <summary>
        /// Ссылка на текущий конструктор выражений.
        /// </summary>
        private ExpressionConstructor _expressionConstructor = new ExpressionConstructor();
        private string _currentVariable = String.Empty;

        public FormulaParser(FormulaElements formulaElements) {
            _formulaElements = formulaElements;
            _expressionConstructor.OnExpressionConstructed += ExpressionConstructedCallback;
        }

        /// <summary>
        /// Проверяет наличие текущей строки, содержащей переменную и пытается использовать её.
        /// </summary>
        private void TryAddVariable() {
            if (!String.IsNullOrEmpty(_currentVariable)) {
                // передать в конструктор выражений...
                _expressionConstructor.AddVariable(_currentVariable);
                if(!ParserExtensions.IsNumber(_currentVariable)) {
                    // ... и записать, как одну из переменных формулы, если это не число.
                    if (!_formulaElements!._variables!.ContainsKey(_currentVariable)) {
                        _formulaElements!._variables!.Add(_currentVariable, 0.0);
                    }
                }
                _currentVariable = String.Empty;
            }
        }

        /// <summary>
        /// Пытается интерпретировать текущую строку переменной, как результат вычислений формулы.
        /// </summary>
        private void TrySetAnswer() {
            if (!String.IsNullOrEmpty(_currentVariable)) {
                // записать в инфу о формуле, как результат
                _formulaElements!._result = _currentVariable;
                _currentVariable = String.Empty;
            }
        }

        /// <summary>
        /// Непосредственно парсит строку с формулой
        /// </summary>
        /// <param name="s">Строка с формулой</param>
        public void Parse(string s) {
            Stack<char> operatorsStack = new Stack<char>();
            for (int i = 0; i < s.Length; i++) {
                char c = s[i];
                switch (c) {
                    // обнаружили символ '=', интерпретируем текущую строку, как результат формулы
                    // (поскольку дефолтная сигнатура формулы: answer = ...
                    case '=':
                        TrySetAnswer();
                        break;
                    // обнаружили открывающую скобку - пытаемся интерпретировать строку, как переменную
                    // (хотя в целом, наверное, перед такой скобкой не может быть переменных),
                    // пушим символ в стек операторов
                    case '(':
                        TryAddVariable();
                        operatorsStack.Push(c);
                        break;
                    // обнаружили закрывающую скобку - пытаемся интерпретировать строку, как переменную
                    // (поскольку, перед такой скобкой может быть только переменная, либо еще такая же скобка),
                    // отправляем в конструктор выражений элементы стека, пока не найдем открывающую скобку
                    case ')':
                        TryAddVariable();
                        while (operatorsStack.Peek() != '(') {
                            char curr = operatorsStack.Pop();
                            _expressionConstructor.AddOperator(curr);
                        }
                        operatorsStack.Pop();
                        break;
                    // обнаружили оператор - пытаемся интерпретировать строку, как переменную
                    // (поскольку, перед оператором может быть только переменная, либо закрывающая скобка),
                    // пока наверху стека оператор, более приоритетный, чем op, отправляем элементы из стека в конструктор.
                    case char op when (ParserExtensions.IsOperator(op)):
                        TryAddVariable();
                        while(operatorsStack.Count > 0 && ParserExtensions.IsOperator(operatorsStack.Peek())) {
                            if (ParserExtensions.IsPriorOrEqualOperator(operatorsStack.Peek(), op)) {
                                _expressionConstructor.AddOperator(operatorsStack.Pop());
                            }
                            else {
                                break;
                            }
                        }
                        operatorsStack.Push(op);
                        break;
                    case ' ':
                        break;
                    // любой другой символ - добавляем его к строке
                    default:
                        _currentVariable += c;
                        break;
                }
            }
            TryAddVariable();
            while(operatorsStack.Count > 0) {
                char curr = operatorsStack.Pop();
                _expressionConstructor.AddOperator(curr);
            }
            _formulaElements!.PrintInfo();
        }

        private void ExpressionConstructedCallback(Expression expression) {
            if (expression == null) {
                return;
            }
            _formulaElements!._expressions.Add(expression);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace FormulasVisualizer.Source {

    public enum OperatorPriority {
        PRIORITY_FIRST,
        PRIORITY_SECOND,
        PRIORITY_THIRD
    }

    public struct Operation {
        public string eval1 { get; set; }
        public string eval2 { get; set; }
        public char op { get; set; }
        public string outval { get; set; }

        public void Print() {
            Trace.WriteLine($"{outval} = {eval1}{op}{eval2}");
        }
    }

    /*
     * Парсит строку с формулой(пока тест логика)
     */
    public class FormulaParser {

        private static IReadOnlyList<char> _numbers = new List<char>() {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '0'
        };

        private static IReadOnlyDictionary<char, OperatorPriority> _operators = new Dictionary<char, OperatorPriority>() {
            ['='] = OperatorPriority.PRIORITY_FIRST,
            ['^'] = OperatorPriority.PRIORITY_FIRST,
            ['*'] = OperatorPriority.PRIORITY_SECOND,
            ['/'] = OperatorPriority.PRIORITY_SECOND,
            ['+'] = OperatorPriority.PRIORITY_THIRD,
            ['-'] = OperatorPriority.PRIORITY_THIRD
        };

        private int _replacesCount = 0;

        public Queue<Operation> queue { get; } = new Queue<Operation>();
        public List<string> _possibleVariables = new List<string>();

        private bool IsNumber(string possibleNumber) {
            return possibleNumber.All(x => _numbers.Contains(x));
        }

        private bool IsReplacer(string possibleReplacer) {
            return possibleReplacer.Contains("EXPRESSION_RESULT_");
        }

        private string GenerateReplacer() {
            _replacesCount++;
            return $"EXPRESSION_RESULT_{_replacesCount}";
        }

        public void Parse(string s, OperatorPriority priority) {
            s = s.Replace(" ", "");
            // разбить строку на переменные и операторы
            string[] variables = s.Split(_operators.Keys.ToArray());
            List<char> operators = new List<char>();
            for (int i = 0; i < s.Length; i++) {
                if (_operators.Keys.Contains(s[i])) {
                    operators.Add(s[i]);
                }
            }
            // определить операторы, которые будем чекать на текущем уровне
            IEnumerable<char> currentIterationOperators = from op in _operators where op.Value == priority select op.Key;
            for (int i = 1; i < operators.Count; i++) {
                if (currentIterationOperators.Contains(operators[i])) {
                    Operation o = new Operation();
                    o.eval1 = variables[i];
                    o.eval2 = variables[i + 1];
                    if (!(IsNumber(o.eval1) || IsReplacer(o.eval1))) {
                        _possibleVariables.Add(o.eval1);
                    }
                    if (!(IsNumber(o.eval2) || IsReplacer(o.eval2))) {
                        _possibleVariables.Add(o.eval2);
                    }
                    o.op = operators[i];
                    string replacer = GenerateReplacer();
                    o.outval = replacer;
                    queue.Enqueue(o);
                    string expr = $"{variables[i]}{operators[i]}{variables[i + 1]}";
                    s = s.Replace(expr, replacer);
                }
            }
            if (priority == OperatorPriority.PRIORITY_THIRD) {
                foreach(Operation o in queue) {
                    o.Print();
                }
                return;
            }
            else {
                Parse(s, priority + 1);
            }
        }
    }
}
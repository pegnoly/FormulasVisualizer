using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        private readonly IReadOnlyDictionary<char, OperatorPriority> _delimeters = new Dictionary<char, OperatorPriority>() {
            ['^'] = OperatorPriority.PRIORITY_FIRST,
            ['*'] = OperatorPriority.PRIORITY_SECOND,
            ['/'] = OperatorPriority.PRIORITY_SECOND,
            ['+'] = OperatorPriority.PRIORITY_THIRD,
            ['-'] = OperatorPriority.PRIORITY_THIRD
        };

        public Queue<Operation> queue { get; } = new Queue<Operation>();
        private char replacer = 'a';

        public void Parse(string s, OperatorPriority priority) {
            s = s.Replace(" ", "");
            IEnumerable<char> ops = from op in _delimeters where op.Value == priority select op.Key;
            string[] variables = s.Split(_delimeters.Keys.ToArray());
            List<char> operators = new List<char>();
            for (int i = 0; i < s.Length; i++) {
                if (_delimeters.Keys.Contains(s[i])) {
                    operators.Add(s[i]);
                }
            }
            for(int i = 1; i < operators.Count; i++) {
                if (ops.Contains(operators[i])) {
                    Operation o = new Operation();
                    o.eval1 = variables[i];
                    o.eval2 = variables[i + 1];
                    o.op = operators[i];
                    o.outval = replacer.ToString();
                    queue.Enqueue(o);
                    string expr = $"{variables[i]}{operators[i]}{variables[i + 1]}";
                    s = s.Replace(expr, replacer.ToString());
                    replacer++;
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
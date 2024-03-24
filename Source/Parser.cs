using System.Collections.Generic;
using System.Linq;

namespace FormulasVisualizer.Source {
    /*
     * Парсит строку с формулой(пока тест логика)
     */
    public static class FormulaParser {

        private static IReadOnlyList<char> _delimeters = new List<char>() {
            '+', '-', '*', '/', '^', '(', ')', '='
        };

        public static string[] Parse(string s) {
            List<string> splittedString = s.Split(_delimeters.ToArray()).ToList();
            splittedString.RemoveAt(0);
            return splittedString.ToArray();
        }
    }
}
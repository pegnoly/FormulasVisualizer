using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FormulasVisualizer.Source {
    /// <summary>
    /// Форма информации, необходимой для отрисовки графика
    /// </summary>
    /// <param name="xAxisName">Имя оси X графика</param>
    /// <param name="yAxisName">Имя оси Y графика</param>
    /// <param name="domain">Значения основной переменной</param>
    /// <param name="codomain">Значения результатов расчета</param>
    public record CalculationResults(string xAxisName, string yAxisName, double[] domain, double[] codomain);

	/*
	 * Производит последовательные расчеты переданных выражений.
	 */
	public class FormulaCalculator {

		private FormulaElements? _formulaElements;

		// Вызывается, когда значения посчитаны и готовы для отображения.
		public delegate void Calculated(CalculationResults results);
		public event Calculated? OnCalculated;

        /// <summary>
        /// Назначает информацию, по которой будут идти расчеты
        /// </summary>
        /// <param name="formulaElements">Ссылка на объект с информацией о формуле</param>
		public void AssignElements(FormulaElements formulaElements) {
            _formulaElements = formulaElements;
        }

        /// <summary>
        /// Считает значение данного выражения.
        /// </summary>
        /// <param name="expression">Выражение</param>
        /// <param name="calculationRound">По факту костыль, чтобы определить, какое значение основной переменной брать, если она нужна(пока хз, как лучше)</param>
        /// <returns>Результат посчитанного выражения</returns>
        private double CalculateExpression(Expression expression, int calculationRound) {
            ExpressionElement left = expression._left!;
            ExpressionElement right = expression._right!;
            // если у элемента есть имя, то
            // если это - имя основной переменной, используем её текущее значение, иначе получаем значение переменной по ее имени.
            if(!String.IsNullOrEmpty(left._name)) {
                left._value = left._name == _formulaElements!._mainVariable.Key ? 
                        _formulaElements._mainVariable.Value[calculationRound] : 
                        _formulaElements._variables![left._name];
            }
            if (!String.IsNullOrEmpty(right._name)) {
                right._value = right._name == _formulaElements!._mainVariable.Key ?
                        _formulaElements._mainVariable.Value[calculationRound] : 
                        _formulaElements._variables![right._name];
            }
            double result = 0.0;
            // считаем
            switch (expression._operator) {
                case '*':
                    result = left._value * right._value;
                    break;
                case '/':
                    result = left._value / right._value;
                    break;
                case '+':
                    result = left._value + right._value;
                    break;
                case '-':
                    result = left._value - right._value;
                    break;
                case '^':
                    result = Math.Pow(left._value, right._value);
                    break;
                default:
                    break;
            }
            // записываем рассчитанную переменную в словарь, чтобы другие выражения могли её использовать
            _formulaElements!._variables![expression._outValueName] = result;
            return result;
        }

        /// <summary>
        /// Считает все возможные варианты формулы.
        /// </summary>
        public void Calculate() {
            Dictionary<double, double> results = new Dictionary<double, double>();
            // используем все возможные значения основной переменной
            for (int i = 0; i < _formulaElements!._mainVariable.Value.Count; i++) {
                double lastResult = 0.0;
                foreach (Expression expression in _formulaElements._expressions) {
                    lastResult = CalculateExpression(expression, i);
                }
                // посчитали всё - записываем значение основной переменной и полученный с его помощью результат
                results.Add(_formulaElements._mainVariable.Value[i], lastResult);
            }
            OnCalculated!(new CalculationResults(
                xAxisName: _formulaElements._result!,
                yAxisName: _formulaElements._mainVariable.Key,
                results.Keys.ToArray(), results.Values.ToArray()
            ));
		}
	}
}

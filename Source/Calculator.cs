using System.Collections.Generic;

namespace FormulasVisualizer.Source {

	/*
	 * Считает значения, основываясь на том, какая переменная считается основной и какие текущие значения имеют другие переменные.
	 * (пока для теста просто суммирует всё)
	 * Производит два массива значений для отображения на графике.
	 */
	public class FormulaCalculator {
		private KeyValuePair<string, List<double>> _mainVariable;
		private Dictionary<string, double>? _variables = new Dictionary<string, double>();

		// Вызывается, когда значения посчитаны и готовы для отображения.
		public delegate void Calculated(double[] domain, double[] codomain);
		public event Calculated? OnCalculated;

		public FormulaCalculator() { }

		// !TODO - добавить перерасчет, когда меняется основная переменная.
		public void AssignMainVariable(string name, List<double> values) { 
			_mainVariable = new KeyValuePair<string, List<double>> (name, values);
		}

		public void AssignVariableValue(string name, double value) {
			if (_variables!.ContainsKey(name)) {
				_variables[name] = value;
			}
			else {
				_variables.Add(name, value);
			}
			Calculate();
		}
		
		// !TODO - реализовать расчет по конкретной формуле вместо тестового суммирования.
		private void Calculate() {
			List<double> results = new List<double>();
			foreach(double variable in  _mainVariable.Value) {
				double currentResult = variable;
				foreach(KeyValuePair<string, double> element in _variables!) {
					currentResult += element.Value;
				}
				results.Add(currentResult);
			}
			OnCalculated!(_mainVariable.Value.ToArray(), results.ToArray());
		}
	}
}

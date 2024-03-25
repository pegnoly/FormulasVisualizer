using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FormulasVisualizer.Source {

	/*
	 * Считает значения, основываясь на том, какая переменная считается основной и какие текущие значения имеют другие переменные.
	 * (пока для теста просто суммирует всё)
	 * Производит два массива значений для отображения на графике.
	 */
	public class FormulaCalculator {
		private Queue<Operation> _operations = new Queue<Operation>();
		private KeyValuePair<string, List<double>> _mainVariable;
		private Dictionary<string, double>? _variables = new Dictionary<string, double>();

		// Вызывается, когда значения посчитаны и готовы для отображения.
		public delegate void Calculated(double[] domain, double[] codomain);
		public event Calculated? OnCalculated;

		public FormulaCalculator() {
		}

		public void AddOperations(Queue<Operation> operations) {
			_operations = operations;
		}

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
			//Calculate();
		}
		
		// !TODO - реализовать расчет по конкретной формуле вместо тестового суммирования.
		public void Calculate() {
			Dictionary<double, double> results = new Dictionary<double, double>();
			foreach(float mainValue in _mainVariable.Value) {
                foreach (Operation operation in _operations) {
                    double val1 = operation.eval1 == _mainVariable.Key ? mainValue : _variables![operation.eval1];
                    double val2 = operation.eval2 == _mainVariable.Key ? mainValue : _variables![operation.eval2];
                    double result = 0.0;
                    switch (operation.op) {
                        case '*':
                            result = val1 * val2;
                            break;
                        case '/':
                            result = val1 / val2;
                            break;
                        case '+':
                            result = val1 + val2;
                            break;
                        case '-':
                            result = val1 - val2;
                            break;
                        default:
                            break;
                    }
                    if (_variables!.ContainsKey(operation.outval)) {
                        _variables[operation.outval] = result;
                    }
                    else {
                        _variables.Add(operation.outval, result);
                    }
                    operation.Print();
                    Trace.WriteLine($"result {result}");
					results[mainValue] = result;
                }
            }
			OnCalculated!(results.Keys.ToArray(), results.Values.ToArray());
		}
	}
}

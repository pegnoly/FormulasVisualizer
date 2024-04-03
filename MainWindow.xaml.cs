using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using FormulasVisualizer.Source;

namespace FormulasVisualizer {
    /*
     * Основная логика приложения.
     * Ввод формулы[1] -> парсинг[2] -> заполнение элемента VariablesGrid[3] -> 
     * добавление значений в калькулятор на основе её данных[4] -> отображение рассчитанных калькулятором результатов[5]
     */
    public partial class MainWindow : Window {

        private FormulaCalculator _calculator = new FormulaCalculator();

        public MainWindow() {
            InitializeComponent();
            // 1.
            _parseFormulaButton.Click += ParseButtonClick;
            // 4.
            _variablesGrid.OnMainVariableSelected += MainVariableSelectedCallback;
            _variablesGrid.OnVariableValueChanged += VariableValueChangedCallback;
            // 5.
            _calculator.OnCalculated += _plotter.Plot;
            _calculateButton.Click += StartCalculation;
        }

        private void MainVariableSelectedCallback(string name, List<double> values) {
            _plotter.SetYAxisName(name);
            _calculator.AssignMainVariable(name, values);
        }

        private void VariableValueChangedCallback(string name, double value) {
            _plotter.SetXAxisName(name);
            _calculator.AssignVariableValue(name, value);
        }

        private void ParseButtonClick(object sender, RoutedEventArgs e) {
            // 2.
            FormulaElements formulaElements = new FormulaElements();
            FormulaParser parser = new FormulaParser(formulaElements);
            parser.Parse(_formulaTextBox.Text);
            //_calculator.AddOperations(parser.queue);
            //// 3.
            _variablesGrid.Fill(formulaElements._variables!.ToArray());
            _plotter.SetYAxisName(formulaElements._result!);
        }

        private void StartCalculation(object sender, RoutedEventArgs e) {
            _calculator.Calculate();
        }
    }
}

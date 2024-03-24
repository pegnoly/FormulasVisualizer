using System.Windows;

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
            _variablesGrid.OnMainVariableSelected += _calculator.AssignMainVariable;
            _variablesGrid.OnVariableValueChanged += _calculator.AssignVariableValue;
            // 5.
            _calculator.OnCalculated += _plotter.Plot;
        }

        private void ParseButtonClick(object sender, RoutedEventArgs e) {
            // 2.
            string[] elements = FormulaParser.Parse(_formulaTextBox.Text);
            // 3.
            _variablesGrid.Fill(elements);
        }
    }
}

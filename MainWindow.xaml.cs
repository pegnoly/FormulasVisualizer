using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using FormulasVisualizer.Source;
using FormulasVisualizer.Components;

namespace FormulasVisualizer {
    /*
     * Основная логика приложения.
     */
    public partial class MainWindow : Window {

        private FormulaCalculator? _calculator = new FormulaCalculator();

        public MainWindow() {
            InitializeComponent();
            _parseFormulaButton.Click += ParseButtonClick;
            _calculateButton.Click += StartCalculation;
            _calculator!.OnCalculated += _plotter.Plot;
        }

        private void ParseButtonClick(object sender, RoutedEventArgs e) {
            // 2.
            FormulaElements formulaElements = new FormulaElements();
            FormulaParser parser = new FormulaParser(formulaElements);
            _calculator!.AssignElements(formulaElements);
            parser.Parse(_formulaTextBox.Text);
            _variablesPanel.Fill(formulaElements._variables!.Keys.ToArray(), formulaElements);
        }

        private void StartCalculation(object sender, RoutedEventArgs e) {
            _variablesPanel.OnUpdated += _calculator!.Calculate;
            _calculator!.Calculate();
        }
    }
}

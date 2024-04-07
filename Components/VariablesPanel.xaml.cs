using FormulasVisualizer.Source;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FormulasVisualizer.Components {
    /// <summary>
    /// Элемент, содержащий элементы отдельных переменных формулы. Отслеживает назначение основной переменной и изменение переменных.
    /// </summary>
    public partial class VariablesPanel : UserControl {
        /// <summary>
        /// Словарь элементов отдельных переменных формулы. Ключ - имя переменной.
        /// </summary>
        private Dictionary<string, FormulaVariableControl> _variableControls = new Dictionary<string, FormulaVariableControl>();
        /// <summary>
        /// Элемент, содержащий текущую основную переменную формулу
        /// </summary>
        private FormulaVariableControl? _currentMainVariableControl;
        /// <summary>
        /// Список имен переменных, которые можно назначить основными
        /// </summary>
        private ObservableCollection<string> _mainVariableSelectorVariants = new ObservableCollection<string>();

        private FormulaElements? _formulaElements;

        public delegate void Updated();
        /// <summary>
        /// Вызывается, когда юзер выбирает новую основную переменную или меняет значение одной из переменных.
        /// </summary>
        public event Updated? OnUpdated;

        public VariablesPanel() {
            InitializeComponent();
            _mainVariableSelector.ItemsSource = _mainVariableSelectorVariants;
            _mainVariableSelector.SelectionChanged += MainVariableSelectedCallback;
        }

        /// <summary>
        /// Создает элементы конкретных переменных, используя их возможные имена.
        /// </summary>
        /// <param name="elements">Массив имен переменных</param>
        /// <param name="formulaElements">Ссылка на актуальную информацию о формуле</param>
        public void Fill(string[] elements, FormulaElements formulaElements) {
            _formulaElements = formulaElements;
            foreach (string element in elements) {
                _mainVariableSelectorVariants.Add(element);
                FormulaVariableControl formulaVariableControl = new FormulaVariableControl(element);
                formulaVariableControl.OnChanged += VariableValueUpdatedCallback;
                _variableControls.Add(element, formulaVariableControl);
                _variablesPanel.Children.Add(formulaVariableControl);
            }
        }

        /// <summary>
        /// Вызывается, когда юзер выбирает значение в селекторе основной переменной
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainVariableSelectedCallback(object sender, RoutedEventArgs e) {
            string variableName = ((sender as ComboBox)!.SelectedItem as string)!;
            if (_currentMainVariableControl != null) {
                _currentMainVariableControl.SetDefault();
            }
            FormulaVariableControl variableControl = _variableControls[variableName];
            variableControl.SetMain();
            _currentMainVariableControl = variableControl;
            _formulaElements!._mainVariable = variableControl.AsMainVariable();
            if(OnUpdated != null) {
                OnUpdated!();
            }
        }

        /// <summary>
        /// Вызывается, когда юзер изменяет конкретную переменную.
        /// </summary>
        /// <param name="name">Имя переменной</param>
        /// <param name="value">Новое значение переменной</param>
        private void VariableValueUpdatedCallback(string name, double value) {
            _formulaElements!.UpdateVariable(name, value);
            if (OnUpdated != null) {
                OnUpdated!();
            }
        }
    }
}

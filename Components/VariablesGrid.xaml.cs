using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FormulasVisualizer.Components {
    /* Контейнер для возможных переменных формулы.
       Отслеживает назначение главной переменной, изменения переменных, формирует информацию, необходимую для расчетов. */
    public partial class VariablesGrid : UserControl {
        
        private List<FormulaVariableControl> _variableControls = new List<FormulaVariableControl>();

        // Вызывается, когда юзер выбирает основную переменную формулы.
        public delegate void MainVariableSelected(string name, List<double> values);
        public event MainVariableSelected? OnMainVariableSelected;

        // Вызывается, когда юзер выбирает новое значение переменной.
        public delegate void VariableValueChanged(string name, double value);
        public event VariableValueChanged? OnVariableValueChanged;

        private ObservableCollection<string> _mainVariableSelectorVariants = new ObservableCollection<string>();

        public VariablesGrid() {
            InitializeComponent();
            _mainVariableSelector.ItemsSource = _mainVariableSelectorVariants;
        }
        
        private void SelectMainVariable(string variableName, ObservableCollection<string> values) {
            List<double> data = new List<double>();
            foreach (string value in values) {
                if (!String.IsNullOrEmpty(value)) {
                    data.Add(Convert.ToDouble(value));
                }
            }
            OnMainVariableSelected!(variableName, data);
        }

        private void SetVariableValue(string variableName, string value) {
            double data = Convert.ToDouble(value);
            OnVariableValueChanged!(variableName, data);
        } 

        public void Fill(string[] elements) {
            foreach (string element in elements) {
                _mainVariableSelectorVariants.Add(element);
                FormulaVariableControl formulaVariableControl = new FormulaVariableControl(element);
                formulaVariableControl.OnSelected += SelectMainVariable;
                formulaVariableControl.OnChanged += SetVariableValue;
                _variableControls.Add(formulaVariableControl);
                _variablesPanel.Children.Add(formulaVariableControl);
            }
        }
    }
}

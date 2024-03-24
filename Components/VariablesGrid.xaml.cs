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

        public VariablesGrid() {
            InitializeComponent();
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
            _grid.RowDefinitions.Add(new RowDefinition());
            int currentColumn = -1;
            foreach (string element in elements) { 
                currentColumn++;
                _grid.ColumnDefinitions.Add(new ColumnDefinition());
                FormulaVariableControl formulaVariableControl = new FormulaVariableControl(element);
                formulaVariableControl.OnSelected += SelectMainVariable;
                formulaVariableControl.OnChanged += SetVariableValue;
                _variableControls.Add(formulaVariableControl);
                Grid.SetColumn(formulaVariableControl, currentColumn);
                Grid.SetRow(formulaVariableControl, 0);
                _grid.Children.Add(formulaVariableControl);
            }
        }
    }
}

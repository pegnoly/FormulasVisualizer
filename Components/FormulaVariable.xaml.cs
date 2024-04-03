using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace FormulasVisualizer.Components {

    /*
     * Элемент, представляющий переменную формулы.
     * Управляет добавлением новых значений этой переменной и назначением её основной для отображения.
     */
    public partial class FormulaVariableControl : UserControl {

        private string? _name = String.Empty;
        private ObservableCollection<string>? _values = new ObservableCollection<string>();
        
        // Вызывается, когда юзер назначает эту переменную основной.
        public delegate void Selected(string variableName, ObservableCollection<string> values);
        public event Selected? OnSelected;

        // Вызывается, когда юзер меняет эту переменную.
        public delegate void Changed(string variableName, string value);
        public event Changed? OnChanged;

        public FormulaVariableControl(string variableName) {
            InitializeComponent();
            _name = variableName;
            _variableNameLabel.Content = variableName;
            _valueSelector.ItemsSource = _values;
            _valueSelector.SelectionChanged += ChangeValue;
        }

        private void AddPossibleValues(object sender, RoutedEventArgs e) {
            foreach (string value in _valueAdder.Text.Split(',')) {
                _values!.Add(value.Trim());
            }
            _valueAdder.Text = String.Empty;
        }

        private void SelectVariable(object sender, RoutedEventArgs e) {
            OnSelected!(_name!, _values!);
        }

        private void ChangeValue(object sender, RoutedEventArgs e) {
            OnChanged!(_name!, _valueSelector.SelectedItem.ToString()!);
        }
    }
}

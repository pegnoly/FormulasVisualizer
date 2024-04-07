using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FormulasVisualizer.Components {

    /*
     * Элемент, представляющий переменную формулы.
     * Управляет добавлением новых значений этой переменной и выбором этих значений.
     */
    public partial class FormulaVariableControl : UserControl {
        /// <summary>
        /// Имя переменной
        /// </summary>
        private string? _name;
        /// <summary>
        /// Значения переменной
        /// </summary>
        private ObservableCollection<double>? _values = new ObservableCollection<double>();

        public delegate void Changed(string variableName, double value);
        /// <summary>
        /// Вызывается, когда юзер меняет значение переменной
        /// </summary>
        public event Changed? OnChanged;

        public FormulaVariableControl(string variableName) {
            InitializeComponent();
            _variableNameLabel.Content = variableName;
            _name = variableName;
            _valueSelector.ItemsSource = _values;
            _valueSelector.SelectionChanged += ProvideChangedValue;
        }

        /// <summary>
        /// Вызывается, когда юзер жмет кнопку добавления новых значений.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPossibleValues(object sender, RoutedEventArgs e) {
            foreach (string value in _valueAdder.Text.Split(',')) {
                string trimmedValue = value.Replace(" ", "");
                _values!.Add(Convert.ToDouble(trimmedValue));
            }
            _valueAdder.Text = String.Empty;
        }

        /// <summary>
        /// Вызывается, когда юзер меняет значение в селекторе переменной
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProvideChangedValue(object sender, RoutedEventArgs e) {
            OnChanged!(_name!, Convert.ToDouble(_valueSelector.SelectedItem!));
        }

        /// <summary>
        /// Возвращает переменную в форме [имя] = список значений. Таким образом представляется основная переменная формулы.
        /// </summary>
        /// <returns>Пара ключ-значение, где ключ - имя переменной, значение - список значений переменной.</returns>
        public KeyValuePair<string, List<double>> AsMainVariable() {
            return new KeyValuePair<string, List<double>>(_name!, _values!.ToList());
        }

        /// <summary>
        /// Отмечает этот элемент, как элемент основной переменной. В этом состоянии нельзя менять её значения.
        /// </summary>
        public void SetMain() {
            _valueSelector.IsEnabled = false;
        }

        /// <summary>
        /// Отмечает этот элемент, как элемент обычной переменной. Возвращается возможность выбирать значения.
        /// </summary>
        public void SetDefault() {
            _valueSelector.IsEnabled = true;
        }
    }
}

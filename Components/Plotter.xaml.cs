using FormulasVisualizer.Source;
using ScottPlot.AxisPanels;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Linq;

namespace FormulasVisualizer.Components {
    /*
     * Управляет отображением данных в виде графика.
     */
    public partial class Plotter : UserControl {

        public Plotter() {
            InitializeComponent();
        }

        public void Plot(CalculationResults results) {
            _plot.Reset();
            _plot.Plot.Axes.Bottom.Label.Text = results.yAxisName!;
            _plot.Plot.Axes.Left.Label.Text = results.xAxisName!;
            _plot.Plot.Axes.SetLimitsX(results.domain.Min() - 1, results.domain.Max() + 1);
            _plot.Plot.Axes.SetLimitsY(results.codomain.Min() - 1, results.codomain.Max() + 1);
            _plot.Plot.Add.Scatter(results.domain, results.codomain);
            _plot.Refresh();
        }
    }
}
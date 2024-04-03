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

        private string? _yAxisName;
        private string? _xAxisName;

        public Plotter() {
            InitializeComponent();
        }

        public void SetXAxisName(string name) {
            _xAxisName = name;
        }

        public void SetYAxisName(string name) {
            _yAxisName = name;
        }

        public void Plot(double[] domain, double[] codomain) {
            _plot.Reset();
            _plot.Plot.Axes.Bottom.Label.Text = _xAxisName!;
            _plot.Plot.Axes.Left.Label.Text = _yAxisName!;
            _plot.Plot.Axes.SetLimitsX(domain.Min() - 1, domain.Max() + 1);
            _plot.Plot.Axes.SetLimitsY(codomain.Min() - 1, codomain.Max() + 1);
            _plot.Plot.Add.Scatter(domain, codomain);
            _plot.Refresh();
        }
    }
}

using System.Windows.Controls;

namespace FormulasVisualizer.Components {
    /*
     * Управляет отображением данных в виде графика.
     */
    public partial class Plotter : UserControl {

        public Plotter() {
            InitializeComponent();
        }

        public void Plot(double[] domain, double[] codomain) {
            _plot.Plot.Add.Scatter(domain, codomain);
            _plot.Refresh();
        }
    }
}

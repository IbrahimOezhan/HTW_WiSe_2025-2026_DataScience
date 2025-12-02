using ScottPlot;

namespace DataScienceSteam
{
    internal class Plot_Utilties
    {
        public static Plot GeneratePlot(
            double[] values, string[] labels, Color barColor, string titel, 
            string leftLabel, string bottomLabel,string fileNameWitoutExtension)
        {
            Plot plot = new();

            var bars = plot.Add.Bars(values);

            bars.Color = barColor;

            double[] positions = new double[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                positions[i] = i;
            }

            plot.Axes.Bottom.SetTicks(positions, labels);

            plot.Title(titel);
            plot.Axes.Left.Label.Text = leftLabel;
            plot.Axes.Bottom.Label.Text = bottomLabel;

            plot.Grid.IsVisible = false;

            plot.SavePng(fileNameWitoutExtension + ".png", 800, 600);

            return plot;
        }
    }
}

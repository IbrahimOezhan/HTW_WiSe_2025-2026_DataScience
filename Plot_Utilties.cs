using ScottPlot;
using ScottPlot.Plottables;

namespace DataScienceSteam
{
    internal class Plot_Utilties
    {
        public static Plot GeneratePlot(
            double[] values, string[] labels,Color barColor, string titel, 
            string leftLabel, string bottomLabel,string fileNameWitoutExtension)
        {
            Plot plot = new();

            int space = 30;

            for (int i = 0; i < values.Length; i++)
            {
                BarPlot barPlot = plot.Add.Bar(position: i * space, value: values[i]);
                barPlot.Color = barColor;
            }

            List<Tick> ticks = new();

            for (int i = 0; i < labels.Length; i++)
            {
                ticks.Add(new Tick(i * space, labels[i]));
            }

            plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks.ToArray());

            plot.Axes.Bottom.TickLabelStyle.Rotation = 90;
            plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.UpperRight;
            plot.Axes.Bottom.TickLabelStyle.OffsetX = 20;
            plot.Axes.Bottom.TickLabelStyle.OffsetY = -15;
            plot.Axes.Bottom.TickLabelStyle.ForeColor = Color.FromColor(System.Drawing.Color.Red);

            plot.Axes.Bottom.MajorTickStyle.Length = 0;

            plot.HideGrid();

            plot.Axes.Margins(bottom: 0);

            plot.Title(titel);

            plot.Axes.Left.Label.Text = leftLabel;

            plot.Axes.Bottom.Label.Text = bottomLabel;

            plot.Axes.AutoScale();

            plot.SavePng(fileNameWitoutExtension + ".png", 800, 600);

            return plot;
        }

        public static Plot GeneratePlot(
    double[] values, double[] secondValue, string[] labels, Color barColor, string titel,
    string leftLabel, string bottomLabel, string fileNameWitoutExtension)
        {
            Plot plot = new();

            int space = 20;
            int barWidth = 4;
            int barSpace = 2;

            Color blue = Color.FromColor(System.Drawing.Color.Blue);
            Color organge = Color.FromColor(System.Drawing.Color.Orange);

            for (int i = 0; i < values.Length; i++)
            {
                int pos = i * space;
                Bar bar = new()
                {
                    Position = pos - barSpace,
                    FillColor = blue,
                    LineColor = blue,
                    Value = values[i],
                };

                Bar bar2 = new()
                {
                    Position = pos + barSpace,
                    FillColor = organge,
                    LineColor = organge,
                    Value = secondValue[i],
                };


                bar.LineWidth = barWidth;
                bar2.LineWidth = barWidth;

                plot.Add.Bar(bar);
                plot.Add.Bar(bar2);
            }

            // build the legend manually
            plot.Legend.IsVisible = true;
            plot.Legend.Alignment = Alignment.UpperLeft;
            plot.Legend.ManualItems.Add(new() { LabelText = "Average Playtime", FillColor = blue });
            plot.Legend.ManualItems.Add(new() { LabelText = "Stability Score", FillColor = organge });

            List<Tick> ticks = new();

            for (int i = 0; i < labels.Length; i++)
            {
                ticks.Add(new Tick(i * space, labels[i]));
            }

            plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks.ToArray());

            plot.Axes.Bottom.TickLabelStyle.Rotation = 90;
            plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.UpperRight;
            plot.Axes.Bottom.TickLabelStyle.OffsetX = 20;
            plot.Axes.Bottom.TickLabelStyle.OffsetY = -15;
            plot.Axes.Bottom.TickLabelStyle.ForeColor = Color.FromColor(System.Drawing.Color.Black);

            plot.Axes.Bottom.MajorTickStyle.Length = 0;

            plot.HideGrid();

            plot.Axes.Margins(bottom: 0);

            plot.Title(titel);

            plot.Axes.Left.Label.Text = leftLabel;

            plot.Axes.Bottom.Label.Text = bottomLabel;

            plot.Axes.AutoScale();

            plot.SavePng(fileNameWitoutExtension + ".png", 1000, 1000);

            return plot;
        }
    }
}

using DatabaseModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;

public static class PriceHistoryPlotter
{
    public static string GeneratePriceChartBase64(List<PriceHistory> priceHistory)
    {
        if (priceHistory.Count < 2)
        {
            return GenerateNoDataImage();
        }
        
        var model = new PlotModel
        {
            Background = OxyColor.Parse("#FFE4AA"),
            TextColor = OxyColor.Parse("#40027E"),
            Title = "История изменения цены",
            TitleFontSize = 20,
            TitleColor = OxyColor.Parse("#40027E")
        };

        var dates = priceHistory.Select(p => DateTime.Parse(p.ChangeDate.ToString())).ToList();
        var prices = priceHistory.Select(p => (double)p.Price).ToList();

        double minPrice = prices.Min();
        double maxPrice = prices.Max();

        double yMin = Math.Max(0, minPrice - 1_000_000);
        double yMax = maxPrice + 1_000_000;
        
        model.Axes.Add(new DateTimeAxis
        {
            Position = AxisPosition.Bottom,
            StringFormat = "dd MMM",
            IntervalType = DateTimeIntervalType.Days,
            Title = "Дата",
            TitleColor = OxyColor.Parse("#40027E"),
            MajorGridlineStyle = LineStyle.Dash,
            AxislineColor = OxyColor.Parse("#40027E"),
            TextColor = OxyColor.Parse("#40027E"),
        });
        
        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Minimum = yMin,
            Maximum = yMax,
            Title = "Цена, руб.",
            TitleColor = OxyColor.Parse("#40027E"),
            AxislineColor = OxyColor.Parse("#40027E"),
            TextColor = OxyColor.Parse("#40027E"),
            MajorGridlineStyle = LineStyle.Dash,
            StringFormat = "#,0"
        });

        var lineSeries = new LineSeries
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 4,
            MarkerFill = OxyColor.Parse("#40027E"),
            Color = OxyColor.Parse("#40027E"),
            StrokeThickness = 2
        };

        for (int i = 0; i < dates.Count; i++)
        {
            lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(dates[i], prices[i]));
        }

        model.Series.Add(lineSeries);
        
        var areaSeries = new AreaSeries
        {
            Color = OxyColor.FromAColor(60, OxyColor.Parse("#40027E")),
            MarkerType = MarkerType.None
        };

        for (int i = 0; i < dates.Count; i++)
        {
            areaSeries.Points.Add(DateTimeAxis.CreateDataPoint(dates[i], prices[i]));
            areaSeries.Points2.Add(DateTimeAxis.CreateDataPoint(dates[i], yMin));
        }

        model.Series.Add(areaSeries);

        var imageBytes = GetPngImage(model);
        return Convert.ToBase64String(imageBytes);
    }

    public static string GenerateNoDataImage()
    {
        var model = new PlotModel
        {
            Background = OxyColor.Parse("#FFE4AA"),
            Title = "История изменения цен не найдена",
            TitleColor = OxyColor.Parse("#40027E"),
            TitleFontSize = 30
        };
        
        byte[] pngBytes = GetPngImage(model, width: 800, height: 400);
        return Convert.ToBase64String(pngBytes);
    }
    
    private static byte[] GetPngImage(PlotModel model, int width = 1000, int height = 600)
    {
        using var stream = new MemoryStream();
        var exporter = new PngExporter { Width = width, Height = height };
        exporter.Export(model, stream);
        return stream.ToArray();
    }
}
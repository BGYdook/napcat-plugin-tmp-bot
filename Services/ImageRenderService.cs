using SkiaSharp;

namespace NapCatTmpBot.Services;

public class ImageRenderService
{
    private readonly SKPaint _textPaint;
    private readonly SKPaint _titlePaint;
    private readonly SKPaint _backgroundPaint;

    public ImageRenderService()
    {
        _backgroundPaint = new SKPaint
        {
            Color = new SKColor(40, 44, 52),
            Style = SKPaintStyle.Fill
        };

        _titlePaint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = 24,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal)
        };

        _textPaint = new SKPaint
        {
            Color = new SKColor(220, 221, 222),
            TextSize = 16,
            Typeface = SKTypeface.FromFamilyName("Arial")
        };
    }

    public byte[] GenerateDlcImage(List<(string name, string nameZh, string game)> dlcs)
    {
        const int width = 700;
        const int itemHeight = 40;
        const int padding = 20;
        const int titleHeight = 60;

        var height = titleHeight + (dlcs.Count * itemHeight) + (padding * 2);

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;

        canvas.DrawRect(0, 0, width, height, _backgroundPaint);
        canvas.DrawText("🗺️ 地图 DLC 列表", padding, 40, _titlePaint);

        var linePaint = new SKPaint
        {
            Color = new SKColor(100, 104, 112),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };
        canvas.DrawLine(padding, titleHeight, width - padding, titleHeight, linePaint);

        var y = titleHeight + 30;
        var currentGame = string.Empty;

        foreach (var dlc in dlcs)
        {
            if (dlc.game != currentGame)
            {
                currentGame = dlc.game;
                y += 10;
                var headerPaint = new SKPaint
                {
                    Color = new SKColor(100, 180, 255),
                    TextSize = 18,
                    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal)
                };
                canvas.DrawText(currentGame == "ETS2" ? "【欧洲卡车模拟2】" : "【美国卡车模拟】", padding, y, headerPaint);
                y += 30;
            }
            canvas.DrawText($"  • {dlc.name} ({dlc.nameZh})", padding, y, _textPaint);
            y += itemHeight;
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}

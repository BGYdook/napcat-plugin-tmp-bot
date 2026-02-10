using SkiaSharp;

namespace NapCatTmpBot.Services;

/// <summary>
/// 图片渲染服务
/// </summary>
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

    /// <summary>
    /// 生成文本图片
    /// </summary>
    public byte[] GenerateTextImage(string title, List<string> lines, int width = 600)
    {
        const int padding = 20;
        const int lineHeight = 30;
        const int titleHeight = 50;

        var height = titleHeight + (lines.Count * lineHeight) + (padding * 2);

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;

        canvas.DrawRect(0, 0, width, height, _backgroundPaint);
        canvas.DrawText(title, padding, 35, _titlePaint);

        var y = titleHeight + padding + 20;
        foreach (var line in lines)
        {
            canvas.DrawText(line, padding, y, _textPaint);
            y += lineHeight;
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    /// <summary>
    /// 生成地图图片
    /// </summary>
    public byte[] GenerateMapImage(string title, List<(string name, double x, double y)> players)
    {
        const int width = 800;
        const int height = 600;

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;

        _backgroundPaint.Color = new SKColor(30, 30, 40);
        canvas.DrawRect(0, 0, width, height, _backgroundPaint);

        _titlePaint.TextSize = 28;
        canvas.DrawText(title, 20, 40, _titlePaint);

        var gridPaint = new SKPaint
        {
            Color = new SKColor(60, 64, 72),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };

        for (var x = 50; x < width - 50; x += 100)
        {
            canvas.DrawLine(x, 70, x, height - 20, gridPaint);
        }
        for (var y = 70; y < height - 20; y += 100)
        {
            canvas.DrawLine(50, y, width - 50, y, gridPaint);
        }

        var playerPaint = new SKPaint
        {
            Color = new SKColor(255, 100, 100),
            Style = SKPaintStyle.Fill
        };

        foreach (var player in players.Take(20))
        {
            var px = width / 2 + (int)(player.x * 0.1);
            var py = height / 2 + (int)(player.y * 0.1);
            px = Math.Max(50, Math.Min(width - 50, px));
            py = Math.Max(70, Math.Min(height - 20, py));
            canvas.DrawCircle(px, py, 6, playerPaint);
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    /// <summary>
    /// 生成足迹地图
    /// </summary>
    public byte[] GenerateTrackMap(string title, List<(string name, double x, double y)> trackPoints)
    {
        const int width = 800;
        const int height = 600;

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;

        _backgroundPaint.Color = new SKColor(30, 30, 40);
        canvas.DrawRect(0, 0, width, height, _backgroundPaint);

        _titlePaint.TextSize = 28;
        canvas.DrawText(title, 20, 40, _titlePaint);

        var gridPaint = new SKPaint
        {
            Color = new SKColor(60, 64, 72),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };

        for (var x = 50; x < width - 50; x += 100)
        {
            canvas.DrawLine(x, 70, x, height - 20, gridPaint);
        }
        for (var y = 70; y < height - 20; y += 100)
        {
            canvas.DrawLine(50, y, width - 50, y, gridPaint);
        }

        if (trackPoints.Count > 1)
        {
            var linePaint = new SKPaint
            {
                Color = new SKColor(100, 180, 255),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 3,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            };

            using var path = new SKPath();
            var first = trackPoints[0];
            var px1 = width / 2 + (int)(first.x * 0.1);
            var py1 = height / 2 + (int)(first.y * 0.1);
            path.MoveTo(px1, py1);

            foreach (var point in trackPoints.Skip(1))
            {
                var px = width / 2 + (int)(point.x * 0.1);
                var py = height / 2 + (int)(point.y * 0.1);
                px = Math.Max(50, Math.Min(width - 50, px));
                py = Math.Max(70, Math.Min(height - 20, py));
                path.LineTo(px, py);
            }
            canvas.DrawPath(path, linePaint);
        }

        var pointPaint = new SKPaint
        {
            Color = new SKColor(255, 100, 100),
            Style = SKPaintStyle.Fill
        };

        var pointBorderPaint = new SKPaint
        {
            Color = SKColors.White,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2
        };

        foreach (var point in trackPoints)
        {
            var px = width / 2 + (int)(point.x * 0.1);
            var py = height / 2 + (int)(point.y * 0.1);
            px = Math.Max(50, Math.Min(width - 50, px));
            py = Math.Max(70, Math.Min(height - 20, py));
            canvas.DrawCircle(px, py, 6, pointPaint);
            canvas.DrawCircle(px, py, 8, pointBorderPaint);
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    /// <summary>
    /// 生成 DLC 列表图片
    /// </summary>
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

    /// <summary>
    /// 将图片数据保存到临时文件并返回路径
    /// </summary>
    public string SaveToTempFile(byte[] imageData, string prefix = "tmp_bot_")
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "NapCatTmpBot");
        if (!Directory.Exists(tempDir))
        {
            Directory.CreateDirectory(tempDir);
        }

        var fileName = $"{prefix}{Guid.NewGuid():N}.png";
        var filePath = Path.Combine(tempDir, fileName);
        File.WriteAllBytes(filePath, imageData);
        return filePath;
    }
}

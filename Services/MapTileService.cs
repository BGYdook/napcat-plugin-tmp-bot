using NapCatTmpBot.Models;
using SkiaSharp;

namespace NapCatTmpBot.Services;

/// <summary>
/// 地图瓦片服务
/// </summary>
public class MapTileService
{
    private readonly HttpClient _http;
    private readonly MapCoordinateService _coordService;
    private readonly Dictionary<string, byte[]> _tileCache = new();
    private readonly object _cacheLock = new();

    public MapTileService(HttpClient http, MapCoordinateService coordService)
    {
        _http = http;
        _coordService = coordService;
    }

    /// <summary>
    /// 下载瓦片（带缓存）
    /// </summary>
    public async Task<byte[]?> DownloadTileAsync(MapType mapType, int z, int x, int y)
    {
        var config = _coordService.GetConfig(mapType);
        var url = config.TileUrl.Replace("{z}", z.ToString()).Replace("{x}", x.ToString()).Replace("{y}", y.ToString());
        var cacheKey = $"{mapType}_{z}_{x}_{y}";

        // 检查缓存
        lock (_cacheLock)
        {
            if (_tileCache.TryGetValue(cacheKey, out var cached))
                return cached;
        }

        try
        {
            var data = await _http.GetByteArrayAsync(url);
            
            // 缓存最多 100 个瓦片
            lock (_cacheLock)
            {
                if (_tileCache.Count > 100)
                {
                    _tileCache.Clear();
                }
                _tileCache[cacheKey] = data;
            }
            
            return data;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 生成地图图片（使用真实瓦片）
    /// </summary>
    public async Task<byte[]?> GenerateMapImageAsync(MapType mapType, double centerX, double centerY, List<(string name, double x, double y, bool isCurrent)> players)
    {
        const int width = 500;
        const int height = 320;
        const int zoom = 7;

        // 转换中心坐标
        var (mapX, mapY) = _coordService.GameToMapCoordinate(centerX, centerY, mapType);
        var config = _coordService.GetConfig(mapType);
        var scale = Math.Pow(2, zoom);
        
        // 计算中心瓦片坐标
        var tileX = (int)(mapX / 256 / scale);
        var tileY = (int)(mapY / 256 / scale);
        
        // 计算偏移
        var offsetX = (int)(mapX % 256) - 128;
        var offsetY = (int)(mapY % 256) - 128;

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;

        // 绘制背景
        var bgPaint = new SKPaint
        {
            Color = new SKColor(93, 93, 93),
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRect(0, 0, width, height, bgPaint);

        // 下载并绘制瓦片（3x3 网格）
        var tileSize = 256;
        var startX = -tileSize + offsetX;
        var startY = -tileSize + offsetY;

        for (var ty = -1; ty <= 1; ty++)
        {
            for (var tx = -1; tx <= 1; tx++)
            {
                var tileData = await DownloadTileAsync(mapType, zoom, tileX + tx, tileY + ty);
                if (tileData != null)
                {
                    using var bitmap = SKBitmap.Decode(tileData);
                    if (bitmap != null)
                    {
                        canvas.DrawBitmap(
                            bitmap,
                            new SKRect(
                                startX + tx * tileSize,
                                startY + ty * tileSize,
                                startX + (tx + 1) * tileSize,
                                startY + (ty + 1) * tileSize
                            )
                        );
                    }
                }
            }
        }

        // 绘制玩家点
        foreach (var player in players)
        {
            var (px, py) = _coordService.GameToMapCoordinate(player.x, player.y, mapType);
            
            // 相对于中心的偏移
            var relX = (px - mapX) / scale + width / 2 - offsetX;
            var relY = (py - mapY) / scale + height / 2 - offsetY;
            
            // 绘制玩家标记
            var borderColor = new SKColor(47, 47, 47);
            var fillColor = player.isCurrent ? new SKColor(28, 183, 21) : new SKColor(21, 140, 251);
            
            // 边框
            var borderPaint = new SKPaint
            {
                Color = borderColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                IsAntialias = true
            };
            canvas.DrawCircle((float)relX, (float)relY, 5, borderPaint);
            
            // 填充
            var fillPaint = new SKPaint
            {
                Color = fillColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            canvas.DrawCircle((float)relX, (float)relY, 5, fillPaint);
        }

        // 裁剪到指定区域
        using var image = surface.Snapshot();
        using var cropped = image.Subset(new SKRectI(0, 0, width, height));
        using var data = cropped.Encode(SKEncodedImageFormat.Png, 90);
        return data.ToArray();
    }
}

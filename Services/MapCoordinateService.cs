using NapCatTmpBot.Models;

namespace NapCatTmpBot.Services;

/// <summary>
/// 地图坐标转换服务
/// </summary>
public class MapCoordinateService
{
    private readonly Dictionary<MapType, MapTileConfig> _configs;

    public MapCoordinateService()
    {
        _configs = new Dictionary<MapType, MapTileConfig>
        {
            {
                MapType.Ets,
                new MapTileConfig
                {
                    TileUrl = "https://ets-map.oss-cn-beijing.aliyuncs.com/ets2/05102019/{z}/{x}/{y}.png",
                    MaxX = 65536,
                    MaxY = 65536,
                    MaxZoom = 8,
                    MinZoom = 2,
                    GameX1 = -113177.313,
                    GameX2 = 97925.625,
                    GameY1 = -122648.086,
                    GameY2 = 88454.85
                }
            },
            {
                MapType.ProMods,
                new MapTileConfig
                {
                    TileUrl = "https://ets-map.oss-cn-beijing.aliyuncs.com/ets2/05102019promods/{z}/{x}/{y}.png",
                    MaxX = 65536,
                    MaxY = 65536,
                    MaxZoom = 8,
                    MinZoom = 2,
                    GameX1 = -135110.156,
                    GameX2 = 168923.75,
                    GameY1 = -190095.016,
                    GameY2 = 113938.891
                }
            }
        };
    }

    /// <summary>
    /// 获取地图配置
    /// </summary>
    public MapTileConfig GetConfig(MapType mapType)
    {
        return _configs[mapType];
    }

    /// <summary>
    /// 判断是否为 ProMods 服务器
    /// </summary>
    public bool IsProModsServer(int serverId)
    {
        return Array.IndexOf(MapTileConfig.ProModsServerIds, serverId) >= 0;
    }

    /// <summary>
    /// 游戏坐标转地图坐标
    /// </summary>
    /// <returns>(x, y) 地图坐标</returns>
    public (double MapX, double MapY) GameToMapCoordinate(double gameX, double gameY, MapType mapType)
    {
        var config = _configs[mapType];
        const double MAX_X = 65536;
        const double MAX_Y = 65536;

        var xtot = config.GameX2 - config.GameX1;
        var ytot = config.GameY2 - config.GameY1;

        var xrel = (gameX - config.GameX1) / xtot;
        var yrel = (gameY - config.GameY1) / ytot;

        return (xrel * MAX_X, yrel * MAX_Y);
    }

    /// <summary>
    /// 计算地图边界
    /// </summary>
    public (double X1, double Y1, double X2, double Y2) GetMapBounds(MapType mapType, int zoom)
    {
        var config = _configs[mapType];
        var scale = Math.Pow(2, zoom);
        return (
            0,
            config.MaxY / scale,
            config.MaxX / scale,
            0
        );
    }
}

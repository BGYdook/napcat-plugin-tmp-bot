namespace NapCatTmpBot.Models;

/// <summary>
/// 地图类型
/// </summary>
public enum MapType
{
    Ets,
    ProMods
}

/// <summary>
/// 地图瓦片配置
/// </summary>
public class MapTileConfig
{
    /// <summary>
    /// 瓦片URL模板
    /// </summary>
    public string TileUrl { get; set; } = string.Empty;

    /// <summary>
    /// 最大X坐标
    /// </summary>
    public int MaxX { get; set; }

    /// <summary>
    /// 最大Y坐标
    /// </summary>
    public int MaxY { get; set; }

    /// <summary>
    /// 最大缩放级别
    /// </summary>
    public int MaxZoom { get; set; }

    /// <summary>
    /// 最小缩放级别
    /// </summary>
    public int MinZoom { get; set; }

    /// <summary>
    /// 游戏坐标X1
    /// </summary>
    public double GameX1 { get; set; }

    /// <summary>
    /// 游戏坐标X2
    /// </summary>
    public double GameX2 { get; set; }

    /// <summary>
    /// 游戏坐标Y1
    /// </summary>
    public double GameY1 { get; set; }

    /// <summary>
    /// 游戏坐标Y2
    /// </summary>
    public double GameY2 { get; set; }

    /// <summary>
    /// ProMods 服务器ID列表
    /// </summary>
    public static readonly int[] ProModsServerIds = { 50, 51 };
}

/// <summary>
/// 玩家地图数据
/// </summary>
public class PlayerMapData
{
    /// <summary>
    /// 地图类型
    /// </summary>
    public MapType MapType { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    public string Avatar { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 服务器名称
    /// </summary>
    public string ServerName { get; set; } = string.Empty;

    /// <summary>
    /// 国家
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    public string RealName { get; set; } = string.Empty;

    /// <summary>
    /// 当前玩家ID
    /// </summary>
    public long CurrentPlayerId { get; set; }

    /// <summary>
    /// 中心X坐标
    /// </summary>
    public double CenterX { get; set; }

    /// <summary>
    /// 中心Y坐标
    /// </summary>
    public double CenterY { get; set; }

    /// <summary>
    /// 玩家列表（含坐标）
    /// </summary>
    public List<MapPlayer> PlayerList { get; set; } = new();
}

/// <summary>
/// 地图上的玩家
/// </summary>
public class MapPlayer
{
    public long TmpId { get; set; }
    public double AxisX { get; set; }
    public double AxisY { get; set; }
    public bool IsCurrentPlayer { get; set; }
}

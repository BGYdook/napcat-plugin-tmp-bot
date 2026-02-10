namespace NapCatTmpBot.Models;

public class TmpPlayerInfo
{
    public long TmpId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SteamId { get; set; } = string.Empty;
    public long RegisterTime { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public bool IsJoinVtc { get; set; }
    public string? VtcName { get; set; }
    public string? VtcRole { get; set; }
    public bool IsBan { get; set; }
    public bool BanHide { get; set; }
    public long? BanUntil { get; set; }
    public string? BanReason { get; set; }
    public string? BanReasonZh { get; set; }
    public int BanCount { get; set; }
    public double? Mileage { get; set; }
    public double? TodayMileage { get; set; }
    public long? LastOnlineTime { get; set; }
    public bool IsSponsor { get; set; }
    public bool SponsorHide { get; set; }
    public double SponsorAmount { get; set; }
    public double? SponsorCumulativeAmount { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
}

public class PlayerMapInfo
{
    public bool Online { get; set; }
    public int? ServerId { get; set; }
    public ServerDetails? ServerDetails { get; set; }
    public Location? Location { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string? Error { get; set; }
}

public class ServerDetails
{
    public string Name { get; set; } = string.Empty;
    public int Id { get; set; }
}

public class Location
{
    public Poi? Poi { get; set; }
}

public class Poi
{
    public string Country { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
}

public class ServerInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Game { get; set; } = string.Empty;
    public bool Online { get; set; }
    public int Players { get; set; }
    public int MaxPlayers { get; set; }
    public string ShortName { get; set; } = string.Empty;
}

public class DlcInfo
{
    public string Name { get; set; } = string.Empty;
    public string NameZh { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}

public class MileageRankingItem
{
    public long TmpId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Mileage { get; set; }
    public int Rank { get; set; }
}

public class MapPlayer
{
    public long TmpId { get; set; }
    public double AxisX { get; set; }
    public double AxisY { get; set; }
    public bool IsCurrentPlayer { get; set; }
}

public class ApiResponse<T>
{
    public int Code { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
}

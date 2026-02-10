using NapCatTmpBot.Models;
using NapCatTmpBot.Services;

namespace NapCatTmpBot.Commands;

public static class PositionCommand
{
    public static async Task<string> Execute(
        CommandContext context,
        PluginConfig config,
        TmpApiService tmpApi,
        LocationTranslationService locationTranslate,
        BaiduTranslateService baiduTranslate,
        BindService bindService,
        MapTileService tileService,
        MapCoordinateService coordService)
    {
        long? tmpId = null;

        if (!string.IsNullOrWhiteSpace(context.Args) && long.TryParse(context.Args, out var parsedId))
        {
            tmpId = parsedId;
        }
        else
        {
            var bind = bindService.GetBind(context.Platform, context.UserId);
            if (bind == null)
            {
                return "请输入正确的玩家编号\n用法: 定位 123456 或先绑定 TMP ID";
            }
            tmpId = bind.TmpId;
        }

        if (!tmpId.HasValue || tmpId.Value <= 0)
        {
            return "请输入正确的玩家编号";
        }

        var playerResult = await tmpApi.PlayerInfoAsync(tmpId.Value);
        if (playerResult.Code != 200 || playerResult.Data == null)
        {
            return "查询玩家信息失败，请重试";
        }

        var player = playerResult.Data;

        var mapResult = await tmpApi.PlayerMapInfoAsync(tmpId.Value);
        if (mapResult.Code != 200 || mapResult.Data == null)
        {
            return "查询玩家位置信息失败，请重试";
        }

        var mapInfo = mapResult.Data;

        if (!mapInfo.Online)
        {
            if (player.LastOnlineTime.HasValue)
            {
                var lastOnline = DateTimeOffset.FromUnixTimeSeconds(player.LastOnlineTime.Value).DateTime;
                var timeDiff = DateTime.UtcNow - lastOnline;
                return $"玩家当前离线\n上次在线: {FormatTimeDiff(timeDiff)}";
            }
            return "玩家当前离线";
        }

        var message = new System.Text.StringBuilder();
        message.AppendLine($"🆔TMP编号: {player.TmpId}");
        message.AppendLine($"😀玩家名称: {player.Name}");
        message.AppendLine($"📶在线状态: 在线🟢");

        if (mapInfo.ServerDetails != null)
        {
            message.AppendLine($"🖥️所在服务器: {mapInfo.ServerDetails.Name}");
        }

        string country = "", city = "";
        if (mapInfo.Location?.Poi != null)
        {
            country = locationTranslate.Translate(mapInfo.Location.Poi.Country);
            city = locationTranslate.Translate(mapInfo.Location.Poi.RealName);
            
            if (config.BaiduTranslateEnable)
            {
                country = await baiduTranslate.TranslateAsync(country);
                city = await baiduTranslate.TranslateAsync(city);
            }
            
            message.AppendLine($"🌍当前位置: {country} - {city}");
        }

        try
        {
            var mapType = MapType.Ets;
            if (mapInfo.ServerId.HasValue && coordService.IsProModsServer(mapInfo.ServerId.Value))
            {
                mapType = MapType.ProMods;
            }

            var players = new List<(string name, double x, double y, bool isCurrent)>
            {
                (player.Name, mapInfo.X, mapInfo.Y, true)
            };

            var imageData = await tileService.GenerateMapImageAsync(mapType, mapInfo.X, mapInfo.Y, players);
            if (imageData != null)
            {
                var imagePath = Path.Combine(Path.GetTempPath(), "NapCatTmpBot", $"position_{Guid.NewGuid():N}.png");
                if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!);
                File.WriteAllBytes(imagePath, imageData);
                message.AppendLine($"\n[CQ:image,file=file:///{imagePath.Replace("\\", "/")}]");
            }
            else
            {
                message.AppendLine("\n📍 地图图片生成失败");
            }
        }
        catch (Exception ex)
        {
            message.AppendLine($"\n📍 地图生成异常: {ex.Message}");
        }

        return message.ToString();
    }

    private static string FormatTimeDiff(TimeSpan diff)
    {
        if (diff.TotalDays >= 1)
            return $"{(int)diff.TotalDays}天前";
        if (diff.TotalHours >= 1)
            return $"{(int)diff.TotalHours}小时前";
        if (diff.TotalMinutes >= 1)
            return $"{(int)diff.TotalMinutes}分钟前";
        return "刚刚";
    }
}

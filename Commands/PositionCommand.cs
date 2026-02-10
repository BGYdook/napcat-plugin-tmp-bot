using NapCatTmpBot.Models;
using NapCatTmpBot.Services;

namespace NapCatTmpBot.Commands;

/// <summary>
/// 定位命令
/// </summary>
public static class PositionCommand
{
    /// <summary>
    /// 执行定位命令
    /// </summary>
    public static async Task<string> Execute(
        CommandContext context,
        PluginConfig config,
        TmpApiService tmpApi,
        BaiduTranslateService translateService,
        BindService bindService,
        ImageRenderService imageRenderService)
    {
        long? tmpId = null;

        // 解析参数
        if (!string.IsNullOrWhiteSpace(context.Args) && long.TryParse(context.Args, out var parsedId))
        {
            tmpId = parsedId;
        }
        else
        {
            // 尝试从绑定获取
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

        // 查询在线信息
        var mapResult = await tmpApi.PlayerMapInfoAsync(tmpId.Value);
        if (mapResult.Code != 200 || mapResult.Data == null)
        {
            return "查询玩家位置信息失败，请重试";
        }

        var mapInfo = mapResult.Data;

        if (!mapInfo.Online)
        {
            // 查询玩家基本信息获取最后在线时间
            var playerResult = await tmpApi.PlayerInfoAsync(tmpId.Value);
            if (playerResult.Code == 200 && playerResult.Data?.LastOnlineTime.HasValue == true)
            {
                var lastOnline = DateTimeOffset.FromUnixTimeSeconds(playerResult.Data.LastOnlineTime.Value).DateTime;
                var timeDiff = DateTime.UtcNow - lastOnline;
                return $"玩家当前离线\n上次在线: {FormatTimeDiff(timeDiff)}";
            }
            return "玩家当前离线";
        }

        var message = new System.Text.StringBuilder();
        message.AppendLine($"🆔TMP编号: {tmpId.Value}");
        message.AppendLine($"📶在线状态: 在线🟢");

        if (mapInfo.ServerDetails != null)
        {
            message.AppendLine($"🖥️所在服务器: {mapInfo.ServerDetails.Name}");
        }

        if (mapInfo.Location?.Poi != null)
        {
            var country = await translateService.TranslateAsync(mapInfo.Location.Poi.Country);
            var city = await translateService.TranslateAsync(mapInfo.Location.Poi.RealName);
            message.AppendLine($"🌍当前位置: {country} - {city}");
        }

        // 生成地图图片
        try
        {
            // TODO: 实现真实的坐标获取和转换
            var imageData = imageRenderService.GenerateMapImage($"玩家定位 - {mapInfo.ServerDetails?.Name ?? "未知"}", []);
            var imagePath = imageRenderService.SaveToTempFile(imageData, "position_");
            message.AppendLine($"\n[CQ:image,file=file:///{imagePath.Replace("\\", "/")}]");
        }
        catch
        {
            message.AppendLine("\n📍 地图图片生成失败");
        }

        return message.ToString();
    }

    /// <summary>
    /// 格式化时间差
    /// </summary>
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

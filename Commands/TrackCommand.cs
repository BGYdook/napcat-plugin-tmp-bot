using NapCatTmpBot.Models;
using NapCatTmpBot.Services;

namespace NapCatTmpBot.Commands;

/// <summary>
/// 足迹命令
/// </summary>
public static class TrackCommand
{
    private static readonly Dictionary<string, string> ServerMap = new(StringComparer.OrdinalIgnoreCase)
    {
        {"s1", "Simulation 1"},
        {"s2", "Simulation 2"},
        {"p", "ProMods"},
        {"a", "Arcade"}
    };

    /// <summary>
    /// 执行足迹命令
    /// </summary>
    public static async Task<string> Execute(
        CommandContext context,
        PluginConfig config,
        TmpApiService tmpApi,
        BindService bindService,
        ImageRenderService imageRenderService)
    {
        // 解析参数: 足迹 [服务器简称] [TMP ID]
        var args = context.Args.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (args.Length < 1)
        {
            return "请指定服务器简称\n用法: 足迹 s1/s2/p/a [TMP ID]";
        }

        // 解析服务器
        var serverShortName = args[0];
        if (!ServerMap.TryGetValue(serverShortName, out var serverName))
        {
            return "无效的服务器简称\n支持的服务器: s1, s2, p, a";
        }

        // 解析 TMP ID
        long? tmpId = null;
        if (args.Length >= 2 && long.TryParse(args[1], out var parsedId))
        {
            tmpId = parsedId;
        }
        else
        {
            var bind = bindService.GetBind(context.Platform, context.UserId);
            if (bind == null)
            {
                return "请输入 TMP ID 或先绑定";
            }
            tmpId = bind.TmpId;
        }

        if (!tmpId.HasValue || tmpId.Value <= 0)
        {
            return "请输入正确的 TMP ID";
        }

        // 获取服务器列表
        var serverResult = await tmpApi.ServerListAsync();
        if (serverResult.Code != 200 || serverResult.Data == null)
        {
            return "获取服务器信息失败";
        }

        var targetServer = serverResult.Data.FirstOrDefault(s => s.Name == serverName);
        if (targetServer == null)
        {
            return $"未找到服务器: {serverName}";
        }

        // 查询玩家信息
        var playerResult = await tmpApi.PlayerInfoAsync(tmpId.Value);
        if (playerResult.Code != 200 || playerResult.Data == null)
        {
            return "查询玩家信息失败";
        }

        var player = playerResult.Data;

        // 获取该服务器上的玩家列表（用于显示足迹）
        // 这里简化处理，实际需要从 API 获取历史轨迹数据
        var mapResult = await tmpApi.PlayerMapInfoAsync(tmpId.Value);
        
        var message = new System.Text.StringBuilder();
        message.AppendLine($"🆔TMP编号: {player.TmpId}");
        message.AppendLine($"😀玩家名称: {player.Name}");
        message.AppendLine($"🖥️服务器: {serverName}");
        
        if (mapResult.Code == 200 && mapResult.Data?.Online == true)
        {
            message.AppendLine("📶在线状态: 在线🟢");
            if (mapResult.Data.Location?.Poi != null)
            {
                message.AppendLine($"🌍位置: {mapResult.Data.Location.Poi.Country} - {mapResult.Data.Location.Poi.RealName}");
            }
        }
        else
        {
            message.AppendLine("📶在线状态: 离线⚫");
        }

        // TODO: 实现完整的足迹地图功能
        message.AppendLine("\n📍 足迹地图功能待完善");

        // 生成图片
        try
        {
            var imageData = imageRenderService.GenerateTrackMap($"足迹 - {player.Name}", []);
            var imagePath = imageRenderService.SaveToTempFile(imageData, "track_");
            message.AppendLine($"[CQ:image,file=file:///{imagePath.Replace("\\", "/")}]");
        }
        catch
        {
            message.AppendLine("\n📍 图片生成失败");
        }

        return message.ToString();
    }
}

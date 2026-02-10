using NapCatTmpBot.Models;
using NapCatTmpBot.Services;

namespace NapCatTmpBot.Commands;

/// <summary>
/// 路况命令
/// </summary>
public static class TrafficCommand
{
    private static readonly Dictionary<string, string> ServerMap = new(StringComparer.OrdinalIgnoreCase)
    {
        {"s1", "Simulation 1"},
        {"s2", "Simulation 2"},
        {"p", "ProMods"},
        {"a", "Arcade"}
    };

    /// <summary>
    /// 执行路况命令
    /// </summary>
    public static async Task<string> Execute(
        CommandContext context,
        PluginConfig config,
        TmpApiService tmpApi,
        BaiduTranslateService translateService,
        ImageRenderService imageRenderService)
    {
        var serverName = context.Args.Trim();
        if (string.IsNullOrEmpty(serverName))
        {
            return "请指定服务器\n用法: 路况 s1|s2|p|a";
        }

        // 解析服务器名称
        string targetServer;
        if (ServerMap.TryGetValue(serverName, out var mapped))
        {
            targetServer = mapped;
        }
        else
        {
            return "无效的服务器名称\n支持的服务器: s1, s2, p, a";
        }

        // 获取服务器列表
        var serverResult = await tmpApi.ServerListAsync();
        if (serverResult.Code != 200 || serverResult.Data == null)
        {
            return "获取服务器信息失败";
        }

        var targetServerInfo = serverResult.Data.FirstOrDefault(s => s.Name == targetServer);
        if (targetServerInfo == null)
        {
            return $"未找到服务器: {targetServer}";
        }

        // 获取该服务器的热门地点玩家（示例：获取巴黎附近玩家）
        // 巴黎坐标: x= -1, y= -100 (欧卡地图)
        var trafficResult = await tmpApi.MapPlayerListAsync(targetServerInfo.Id, -2000, -5000, 2000, 5000);
        
        var message = new System.Text.StringBuilder();
        message.AppendLine($"🚦 {targetServer} 路况信息");
        message.AppendLine($"📊 在线人数: {targetServerInfo.Players}/{targetServerInfo.MaxPlayers}");
        
        if (config.TmpTrafficType == 2 || trafficResult.Code != 200 || trafficResult.Data == null)
        {
            // 文字模式
            if (trafficResult.Code == 200 && trafficResult.Data != null && trafficResult.Data.Count > 0)
            {
                message.AppendLine($"\n📍 当前区域玩家数: {trafficResult.Data.Count}");
                message.AppendLine("\n附近玩家:");
                var count = 0;
                foreach (var player in trafficResult.Data.Take(10))
                {
                    message.AppendLine($"  - {player.Name} (ID: {player.TmpId})");
                    count++;
                }
                if (trafficResult.Data.Count > 10)
                {
                    message.AppendLine($"  ... 还有 {trafficResult.Data.Count - 10} 位玩家");
                }
            }
            else
            {
                message.AppendLine("\n📍 当前区域暂无玩家");
            }
        }
        else
        {
            // 图片模式
            try
            {
                var players = trafficResult.Data?.Select(p => (p.Name, 0.0, 0.0)).ToList() ?? [];
                var imageData = imageRenderService.GenerateMapImage($"路况 - {targetServer}", players);
                var imagePath = imageRenderService.SaveToTempFile(imageData, "traffic_");
                message.AppendLine($"\n[CQ:image,file=file:///{imagePath.Replace("\\", "/")}]");
            }
            catch
            {
                message.AppendLine("\n📷 路况图片生成失败，已切换为文字模式");
            }
        }

        return message.ToString();
    }
}

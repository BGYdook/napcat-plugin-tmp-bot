using NapCatTmpBot.Models;
using NapCatTmpBot.Services;

namespace NapCatTmpBot.Commands;

/// <summary>
/// 服务器命令
/// </summary>
public static class ServerCommand
{
    /// <summary>
    /// 执行服务器命令
    /// </summary>
    public static async Task<string> Execute(CommandContext context, TmpApiService tmpApi)
    {
        var result = await tmpApi.ServerListAsync();
        if (result.Code != 200 || result.Data == null)
        {
            return "获取服务器信息失败";
        }

        var message = new System.Text.StringBuilder();
        message.AppendLine("🖥️ 欧卡/美卡服务器列表");
        message.AppendLine();

        // 欧卡服务器
        message.AppendLine("【欧洲卡车模拟2】");
        var ets2Servers = result.Data.Where(s => s.Game == "ETS2").ToList();
        foreach (var server in ets2Servers)
        {
            var status = server.Online ? "🟢" : "🔴";
            message.AppendLine($"  {status} {server.Name}");
            message.AppendLine($"     玩家: {server.Players}/{server.MaxPlayers}");
        }

        // 美卡服务器
        message.AppendLine("\n【美国卡车模拟】");
        var atsServers = result.Data.Where(s => s.Game == "ATS").ToList();
        foreach (var server in atsServers)
        {
            var status = server.Online ? "🟢" : "🔴";
            message.AppendLine($"  {status} {server.Name}");
            message.AppendLine($"     玩家: {server.Players}/{server.MaxPlayers}");
        }

        return message.ToString();
    }
}

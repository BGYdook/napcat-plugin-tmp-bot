using NapCatTmpBot.Models;
using NapCatTmpBot.Services;

namespace NapCatTmpBot.Commands;

/// <summary>
/// 帮助命令
/// </summary>
public static class HelpCommand
{
    /// <summary>
    /// 执行帮助命令
    /// </summary>
    public static async Task<string> Execute(CommandContext context)
    {
        var help = @"""
TMP查询插件使用说明

可用命令:
1. 绑定 [TMP ID] - 绑定您的 TMP ID
2. 解绑 - 解除 TMP ID 绑定
3. 查询 [TMP ID] - 查询玩家信息
4. 定位 [TMP ID] - 查询玩家位置
5. 路况 [s1/s2/p/a] - 查询路况
6. 服务器 - 查询服务器列表
7. 足迹 [s1/s2/p/a] [TMP ID] - 查看足迹
8. 总里程排行 - 总里程排行榜
9. 今日里程排行 - 今日里程排行榜
10. DLC - 地图 DLC 列表
11. 插件版本 - 查看插件版本
12. 帮助 - 显示此帮助信息

使用提示: 绑定后可直接发送 查询/定位/足迹
""";
        return help;
    }
}

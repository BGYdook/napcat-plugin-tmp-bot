using NapCatTmpBot.Models;
using NapCatTmpBot.Services;

namespace NapCatTmpBot.Commands;

/// <summary>
/// 里程排行榜命令
/// </summary>
public static class MileageRankingCommand
{
    /// <summary>
    /// 执行里程排行命令
    /// </summary>
    public static async Task<string> Execute(
        CommandContext context,
        PluginConfig config,
        TmpApiService tmpApi,
        BaiduTranslateService translateService,
        string rankingType)
    {
        var title = rankingType == "today" ? "今日里程排行榜" : "总里程排行榜";
        
        // 解析参数，如果有指定 TMP ID
        long? tmpId = null;
        if (!string.IsNullOrWhiteSpace(context.Args) && long.TryParse(context.Args, out var parsedId))
        {
            tmpId = parsedId;
        }
        else
        {
            // 尝试从绑定获取
            var bind = config.EnableBindFeature ? 
                await Task.Run(() => tmpId) : null;
        }

        var result = await tmpApi.MileageRankingListAsync(rankingType, tmpId);
        if (result.Code != 200 || result.Data == null)
        {
            return $"获取{title}失败";
        }

        var message = new System.Text.StringBuilder();
        message.AppendLine($"🏆 {title}");
        message.AppendLine();

        foreach (var item in result.Data)
        {
            var medal = item.Rank switch
            {
                1 => "🥇",
                2 => "🥈",
                3 => "🥉",
                _ => $"{item.Rank}."
            };
            var mileage = FormatMileage(item.Mileage);
            message.AppendLine($"  {medal} {item.Name} (ID:{item.TmpId})");
            message.AppendLine($"     里程: {mileage}");
        }

        return message.ToString();
    }

    /// <summary>
    /// 格式化里程
    /// </summary>
    private static string FormatMileage(double meters)
    {
        if (meters > 1000)
        {
            return $"{(meters / 1000):F1}公里";
        }
        return $"{meters}米";
    }
}

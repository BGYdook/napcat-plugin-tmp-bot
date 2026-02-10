using NapCatTmpBot.Models;
using NapCatTmpBot.Services;

namespace NapCatTmpBot.Commands;

public static class QueryCommand
{
    private static readonly Dictionary<string, string> UserGroups = new()
    {
        {"Player", "玩家"},
        {"Retired Legend", "退役"},
        {"Game Developer", "游戏开发者"},
        {"Retired Team Member", "退休团队成员"},
        {"Add-On Team", "附加组件团队"},
        {"Game Moderator", "游戏管理员"}
    };

    public static async Task<string> Execute(
        CommandContext context,
        PluginConfig config,
        TmpApiService tmpApi,
        BaiduTranslateService translateService,
        BindService bindService)
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
                return "请输入正确的玩家编号\n用法: 查询 123456 或先绑定 TMP ID";
            }
            tmpId = bind.TmpId;
        }

        if (!tmpId.HasValue || tmpId.Value <= 0)
        {
            return "请输入正确的玩家编号";
        }

        var playerResult = await tmpApi.PlayerInfoAsync(tmpId.Value);
        if (playerResult.Code == 10001)
        {
            return "玩家不存在";
        }
        if (playerResult.Code != 200 || playerResult.Data == null)
        {
            return "查询玩家信息失败，请重试";
        }

        var player = playerResult.Data;

        var mapResult = await tmpApi.PlayerMapInfoAsync(tmpId.Value);
        var mapInfo = mapResult.Code == 200 ? mapResult.Data : null;

        var message = new System.Text.StringBuilder();

        if (config.QueryShowAvatarEnable && !string.IsNullOrEmpty(player.AvatarUrl))
        {
            message.AppendLine($"[CQ:image,file={player.AvatarUrl}]\n");
        }

        message.AppendLine($"🆔TMP编号: {player.TmpId}");
        message.AppendLine($"😀玩家名称: {player.Name}");
        message.AppendLine($"🎮SteamID: {player.SteamId}");

        var registerDate = DateTimeOffset.FromUnixTimeSeconds(player.RegisterTime).DateTime;
        var daysDiff = (int)(DateTime.UtcNow - registerDate).TotalDays;
        message.AppendLine($"📑注册日期: {registerDate:yyyy年MM月dd日} ({daysDiff}天)");

        var groupName = UserGroups.TryGetValue(player.GroupName, out var translated) ? translated : player.GroupName;
        message.AppendLine($"💼所属分组: {groupName}");

        if (player.IsJoinVtc)
        {
            if (!string.IsNullOrEmpty(player.VtcName))
                message.AppendLine($"🚚所属车队: {player.VtcName}");
            if (!string.IsNullOrEmpty(player.VtcRole))
                message.AppendLine($"🚚车队角色: {player.VtcRole}");
        }

        message.AppendLine($"🚫是否封禁: {(player.IsBan ? "是" : "否")}");
        message.AppendLine($"🚫封禁次数: {player.BanCount}");

        if (player.IsBan)
        {
            message.Append("🚫封禁截止: ");
            if (player.BanHide)
            {
                message.AppendLine("隐藏");
            }
            else
            {
                if (!player.BanUntil.HasValue)
                {
                    message.AppendLine("永久");
                }
                else
                {
                    var banUntil = DateTimeOffset.FromUnixTimeSeconds(player.BanUntil.Value).DateTime;
                    message.AppendLine($"{banUntil:yyyy年MM月dd日 HH:mm}");
                }
                var reason = !string.IsNullOrEmpty(player.BanReasonZh) ? player.BanReasonZh : player.BanReason;
                if (!string.IsNullOrEmpty(reason))
                {
                    message.AppendLine($"🚫封禁原因: {reason}");
                }
            }
        }

        if (player.Mileage.HasValue)
        {
            var mileage = player.Mileage.Value;
            message.AppendLine($"🚩历史里程: {FormatMileage(mileage)}");
        }

        if (player.TodayMileage.HasValue)
        {
            var todayMileage = player.TodayMileage.Value;
            message.AppendLine($"🚩今日里程: {FormatMileage(todayMileage)}");
        }

        if (mapInfo != null && mapInfo.Online)
        {
            message.Append("📶在线状态: 在线🟢");
            if (mapInfo.ServerDetails != null)
            {
                message.Append($" ({mapInfo.ServerDetails.Name})");
            }
            message.AppendLine();

            if (mapInfo.Location?.Poi != null)
            {
                var country = await translateService.TranslateAsync(mapInfo.Location.Poi.Country);
                var city = await translateService.TranslateAsync(mapInfo.Location.Poi.RealName);
                message.AppendLine($"🌍线上位置: {country} - {city}");
            }
        }
        else if (player.LastOnlineTime.HasValue)
        {
            var lastOnline = DateTimeOffset.FromUnixTimeSeconds(player.LastOnlineTime.Value).DateTime;
            var timeDiff = DateTime.UtcNow - lastOnline;
            message.AppendLine($"📶上次在线: {FormatTimeDiff(timeDiff)}");
        }

        if (player.IsSponsor)
        {
            message.Append("🎁赞助用户");
            if (!player.SponsorHide)
            {
                message.AppendLine($": $${Math.Floor(player.SponsorAmount / 100)}");
            }
            else
            {
                message.AppendLine();
            }
        }

        if (player.SponsorCumulativeAmount.HasValue)
        {
            message.AppendLine($"🎁累计赞助: $${Math.Floor(player.SponsorCumulativeAmount.Value / 100)}");
        }

        return message.ToString();
    }

    private static string FormatMileage(double meters)
    {
        if (meters > 1000)
        {
            return $"{(meters / 1000):F1}公里";
        }
        return $"{meters}米";
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

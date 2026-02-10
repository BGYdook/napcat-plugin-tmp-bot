using NapCatTmpBot.Models;
using NapCatTmpBot.Services;

namespace NapCatTmpBot.Commands;

/// <summary>
/// 绑定命令
/// </summary>
public static class BindCommand
{
    /// <summary>
    /// 执行绑定命令
    /// </summary>
    public static async Task<string> Execute(CommandContext context, BindService bindService)
    {
        if (string.IsNullOrWhiteSpace(context.Args) || !long.TryParse(context.Args, out var tmpId))
        {
            return "请输入正确的玩家编号\n用法: 绑定 123456";
        }

        if (tmpId <= 0)
        {
            return "TMP ID 必须为正整数";
        }

        bindService.AddBind(context.Platform, context.UserId, tmpId);
        return $"绑定成功！TMP ID: {tmpId}";
    }

    /// <summary>
    /// 执行解绑命令
    /// </summary>
    public static async Task<string> Unbind(CommandContext context, BindService bindService)
    {
        var removed = bindService.RemoveBind(context.Platform, context.UserId);
        return removed ? "解绑成功" : "未找到绑定信息";
    }
}

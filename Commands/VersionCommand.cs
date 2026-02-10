using NapCatTmpBot.Models;
using NapCatTmpBot.Services;

namespace NapCatTmpBot.Commands;

/// <summary>
/// 插件版本命令
/// </summary>
public static class VersionCommand
{
    /// <summary>
    /// 执行版本命令
    /// </summary>
    public static async Task<string> Execute(CommandContext context, TmpApiService tmpApi)
    {
        var pluginVersion = Main.GetVersion();
        var message = new System.Text.StringBuilder();
        
        message.AppendLine("🔧 插件版本信息");
        message.AppendLine($"📦 插件版本: v{pluginVersion}");
        message.AppendLine("🏠 项目地址: https://github.com/BGYdook/napcat-plugin-tmp-bot");
        message.AppendLine("👤 作者: BGYdook, Goodnight_An");
        
        // 获取 API 版本
        var versionResult = await tmpApi.VersionAsync();
        if (versionResult.Code == 200 && versionResult.Data != null)
        {
            // API 返回的是原始对象，简单输出
            message.AppendLine("\n🌐 TMP API 状态: 正常");
        }
        else
        {
            message.AppendLine("\n🌐 TMP API 状态: 异常");
        }
        
        return message.ToString();
    }
}

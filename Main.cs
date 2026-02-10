using NapCatTmpBot.Commands;
using NapCatTmpBot.Models;
using NapCatTmpBot.Services;
using System.Reflection;

namespace NapCatTmpBot;

/// <summary>
/// NapCat TMP Bot 插件主类
/// </summary>
public class Main
{
    private readonly PluginConfig _config;
    private readonly HttpClient _httpClient;
    private readonly BindService _bindService;
    private readonly TmpApiService _tmpApi;
    private readonly BaiduTranslateService _translateService;
    private readonly ImageRenderService _imageRenderService;
    private readonly Dictionary<string, Func<CommandContext, Task<string>>> _commands;

    public Main(PluginConfig config)
    {
        _config = config;
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_config.ApiTimeoutSeconds) };
        _bindService = new BindService();
        _tmpApi = new TmpApiService(_httpClient);
        _translateService = new BaiduTranslateService(_httpClient, _config);
        _imageRenderService = new ImageRenderService();

        _commands = new Dictionary<string, Func<CommandContext, Task<string>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["绑定"] = HandleBind,
            ["解绑"] = HandleUnbind,
            ["查询"] = HandleQuery,
            ["查"] = HandleQuery,
            ["定位"] = HandlePosition,
            ["路况"] = HandleTraffic,
            ["服务器"] = HandleServer,
            ["插件版本"] = HandleVersion,
            ["DLC列表"] = HandleDlc,
            ["地图DLC"] = HandleDlc,
            ["总里程排行"] = HandleMileageRanking,
            ["今日里程排行"] = HandleMileageRanking,
            ["排行"] = HandleMileageRanking
        };
    }

    /// <summary>
    /// 处理消息
    /// </summary>
    public async Task<string?> HandleMessageAsync(CommandContext context)
    {
        var content = context.Message.Trim();

        // 解析命令
        foreach (var cmd in _commands.Keys.OrderByDescending(x => x.Length))
        {
            if (content.StartsWith(cmd, StringComparison.OrdinalIgnoreCase))
            {
                var args = content.Substring(cmd.Length).Trim();
                context.Args = args;
                context.RawArgs = args;
                context.CommandName = cmd;
                return await _commands[cmd](context);
            }
        }

        return null;
    }

    #region Command Handlers

    private async Task<string> HandleBind(CommandContext context)
    {
        if (!_config.EnableBindFeature)
        {
            return "绑定功能已禁用";
        }

        return await BindCommand.Execute(context, _bindService);
    }

    private async Task<string> HandleUnbind(CommandContext context)
    {
        if (!_config.EnableBindFeature)
        {
            return "绑定功能已禁用";
        }

        return await BindCommand.Unbind(context, _bindService);
    }

    private async Task<string> HandleQuery(CommandContext context)
    {
        return await QueryCommand.Execute(context, _config, _tmpApi, _translateService, _bindService);
    }

    private async Task<string> HandlePosition(CommandContext context)
    {
        return await PositionCommand.Execute(context, _config, _tmpApi, _translateService, _bindService, _imageRenderService);
    }

    private async Task<string> HandleTraffic(CommandContext context)
    {
        return await TrafficCommand.Execute(context, _config, _tmpApi, _translateService, _imageRenderService);
    }

    private async Task<string> HandleServer(CommandContext context)
    {
        return await ServerCommand.Execute(context, _tmpApi);
    }

    private async Task<string> HandleVersion(CommandContext context)
    {
        return await VersionCommand.Execute(context, _tmpApi);
    }

    private async Task<string> HandleDlc(CommandContext context)
    {
        return await DlcCommand.Execute(context, _config, _imageRenderService);
    }

    private async Task<string> HandleMileageRanking(CommandContext context)
    {
        var type = context.CommandName == "今日里程排行" ? "today" : "total";
        return await MileageRankingCommand.Execute(context, _config, _tmpApi, _translateService, type);
    }

    #endregion

    /// <summary>
    /// 获取插件版本信息
    /// </summary>
    public static string GetVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.7.4";
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

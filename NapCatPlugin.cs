using Newtonsoft.Json;
using NapCatTmpBot.Models;

namespace NapCatTmpBot;

/// <summary>
/// NapCat 插件接口适配器
/// </summary>
public class NapCatPlugin
{
    private Main? _bot;
    private PluginConfig _config = new();

    /// <summary>
    /// 插件加载
    /// </summary>
    public void OnLoad(string configJson)
    {
        try
        {
            if (!string.IsNullOrEmpty(configJson))
            {
                _config = JsonConvert.DeserializeObject<PluginConfig>(configJson) ?? new PluginConfig();
            }
            _bot = new Main(_config);
        }
        catch (Exception ex)
        {
            throw new Exception($"插件加载失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 处理消息
    /// </summary>
    public string OnMessage(long userId, long? groupId, string platform, string message)
    {
        if (_bot == null)
        {
            return string.Empty;
        }

        try
        {
            var context = new CommandContext
            {
                Message = message,
                Platform = platform,
                UserId = userId,
                GroupId = groupId
            };

            var task = _bot.HandleMessageAsync(context);
            task.Wait();
            return task.Result ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 插件卸载
    /// </summary>
    public void OnUnload()
    {
        _bot?.Dispose();
        _bot = null;
    }

    /// <summary>
    /// 获取插件信息
    /// </summary>
    public string GetPluginInfo()
    {
        var version = Main.GetVersion();
        var info = new
        {
            name = "NapCatTmpBot",
            version = version,
            author = "BGYdook, Goodnight_An",
            description = "欧洲卡车模拟2 TMP查询机器人 - NapCat原生插件",
            homepage = "https://github.com/BGYdook/napcat-plugin-tmp-bot"
        };
        return JsonConvert.SerializeObject(info, Formatting.Indented);
    }
}

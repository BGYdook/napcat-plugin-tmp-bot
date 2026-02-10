using NapCatTmpBot.Models;
using Newtonsoft.Json;

namespace NapCatTmpBot.Services;

/// <summary>
/// 绑定服务 - 使用JSON文件存储
/// </summary>
public class BindService
{
    private const string BindFile = "data/tmp_binds.json";
    private readonly List<BindData> _binds = new();
    private readonly object _lock = new();

    public BindService()
    {
        LoadBinds();
    }

    /// <summary>
    /// 从文件加载绑定数据
    /// </summary>
    private void LoadBinds()
    {
        try
        {
            if (File.Exists(BindFile))
            {
                lock (_lock)
                {
                    var json = File.ReadAllText(BindFile);
                    _binds.AddRange(JsonConvert.DeserializeObject<List<BindData>>(json) ?? []);
                }
            }
        }
        catch
        {
            // 忽略加载错误
        }
    }

    /// <summary>
    /// 保存绑定数据到文件
    /// </summary>
    private void SaveBinds()
    {
        try
        {
            lock (_lock)
            {
                var dir = Path.GetDirectoryName(BindFile);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var json = JsonConvert.SerializeObject(_binds, Formatting.Indented);
                File.WriteAllText(BindFile, json);
            }
        }
        catch
        {
            // 忽略保存错误
        }
    }

    /// <summary>
    /// 获取用户的绑定信息
    /// </summary>
    public BindData? GetBind(string platform, long userId)
    {
        lock (_lock)
        {
            return _binds.FirstOrDefault(b => b.Platform == platform && b.UserId == userId);
        }
    }

    /// <summary>
    /// 添加绑定
    /// </summary>
    public bool AddBind(string platform, long userId, long tmpId)
    {
        lock (_lock)
        {
            var existing = _binds.FirstOrDefault(b => b.Platform == platform && b.UserId == userId);
            if (existing != null)
            {
                existing.TmpId = tmpId;
                existing.BindTime = DateTime.UtcNow;
            }
            else
            {
                _binds.Add(new BindData
                {
                    Platform = platform,
                    UserId = userId,
                    TmpId = tmpId
                });
            }
        }
        SaveBinds();
        return true;
    }

    /// <summary>
    /// 删除绑定
    /// </summary>
    public bool RemoveBind(string platform, long userId)
    {
        lock (_lock)
        {
            var bind = _binds.FirstOrDefault(b => b.Platform == platform && b.UserId == userId);
            if (bind != null)
            {
                _binds.Remove(bind);
                SaveBinds();
                return true;
            }
        }
        return false;
    }
}

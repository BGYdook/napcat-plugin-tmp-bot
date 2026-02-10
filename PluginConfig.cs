namespace NapCatTmpBot;

/// <summary>
/// 插件配置
/// </summary>
public class PluginConfig
{
    /// <summary>
    /// 查询时显示头像
    /// </summary>
    public bool QueryShowAvatarEnable { get; set; } = true;

    /// <summary>
    /// 启用百度翻译
    /// </summary>
    public bool BaiduTranslateEnable { get; set; } = true;

    /// <summary>
    /// 百度翻译App ID
    /// </summary>
    public string BaiduTranslateAppId { get; set; } = string.Empty;

    /// <summary>
    /// 百度翻译密钥
    /// </summary>
    public string BaiduTranslateKey { get; set; } = string.Empty;

    /// <summary>
    /// 启用翻译缓存
    /// </summary>
    public bool BaiduTranslateCacheEnable { get; set; } = false;

    /// <summary>
    /// API超时时间（秒）
    /// </summary>
    public int ApiTimeoutSeconds { get; set; } = 10;

    /// <summary>
    /// 优先使用VTCM里程数据
    /// </summary>
    public bool PreferVtcmMileage { get; set; } = true;

    /// <summary>
    /// 启用绑定功能
    /// </summary>
    public bool EnableBindFeature { get; set; } = true;

    /// <summary>
    /// DLC列表使用图片输出
    /// </summary>
    public bool DlcListImage { get; set; } = false;

    /// <summary>
    /// 查询类型（1=图片，2=文字）
    /// </summary>
    public int TmpQueryType { get; set; } = 1;

    /// <summary>
    /// 路况查询类型（1=图片，2=文字）
    /// </summary>
    public int TmpTrafficType { get; set; } = 1;
}

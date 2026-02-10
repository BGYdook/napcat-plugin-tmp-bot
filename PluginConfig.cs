namespace NapCatTmpBot;

public class PluginConfig
{
    public bool QueryShowAvatarEnable { get; set; } = true;
    
    /// <summary>
    /// 启用百度翻译（每月 200 万字符免费额度）
    /// </summary>
    public bool BaiduTranslateEnable { get; set; } = true;
    
    public string BaiduTranslateAppId { get; set; } = string.Empty;
    public string BaiduTranslateKey { get; set; } = string.Empty;
    public bool BaiduTranslateCacheEnable { get; set; } = false;
    
    public int ApiTimeoutSeconds { get; set; } = 10;
    public bool PreferVtcmMileage { get; set; } = true;
    public bool EnableBindFeature { get; set; } = true;
    public bool DlcListImage { get; set; } = false;
    public int TmpQueryType { get; set; } = 1;
    public int TmpTrafficType { get; set; } = 1;
}

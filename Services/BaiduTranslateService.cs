using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace NapCatTmpBot.Services;

/// <summary>
/// 百度翻译服务
/// </summary>
public class BaiduTranslateService
{
    private const string ApiUrl = "https://fanyi-api.baidu.com/api/trans/vip/translate";
    private readonly HttpClient _http;
    private readonly PluginConfig _config;
    private readonly Dictionary<string, string> _cache = new();
    private readonly object _cacheLock = new();

    // 地方名称修正表
    private static readonly Dictionary<string, string> LocationCorrections = new()
    {
        {"United Kingdom", "英国"},
        {"Germany", "德国"},
        {"France", "法国"},
        {"Italy", "意大利"},
        {"Spain", "西班牙"},
        {"Poland", "波兰"},
        {"Czech Republic", "捷克"},
        {"Belgium", "比利时"},
        {"Netherlands", "荷兰"},
        {"Austria", "奥地利"},
        {"Switzerland", "瑞士"},
        {"Hungary", "匈牙利"},
        {"Romania", "罗马尼亚"},
        {"Bulgaria", "保加利亚"},
        {"Norway", "挪威"},
        {"Sweden", "瑞典"},
        {"Denmark", "丹麦"},
        {"Finland", "芬兰"},
        {"Portugal", "葡萄牙"},
        {"Turkey", "土耳其"},
        {"Russia", "俄罗斯"},
        {"Lithuania", "立陶宛"},
        {"Latvia", "拉脱维亚"},
        {"Estonia", "爱沙尼亚"},
        {"Slovenia", "斯洛文尼亚"},
        {"Slovakia", "斯洛伐克"},
        {"Croatia", "克罗地亚"},
        {"Serbia", "塞尔维亚"},
        {"Luxembourg", "卢森堡"},
        {"Greece", "希腊"}
    };

    public BaiduTranslateService(HttpClient http, PluginConfig config)
    {
        _http = http;
        _config = config;
    }

    /// <summary>
    /// 翻译文本
    /// </summary>
    public async Task<string> TranslateAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        // 检查修正表
        if (LocationCorrections.TryGetValue(text, out var corrected))
            return corrected;

        if (!_config.BaiduTranslateEnable || string.IsNullOrEmpty(_config.BaiduTranslateAppId) || string.IsNullOrEmpty(_config.BaiduTranslateKey))
            return text;

        // 检查缓存
        if (_config.BaiduTranslateCacheEnable)
        {
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(text, out var cached))
                    return cached;
            }
        }

        try
        {
            var salt = DateTime.UtcNow.Ticks.ToString();
            var sign = MD5Hash(_config.BaiduTranslateAppId + text + salt + _config.BaiduTranslateKey);
            
            var url = $"{ApiUrl}?q={Uri.EscapeDataString(text)}&from=auto&to=zh&appid={_config.BaiduTranslateAppId}&salt={salt}&sign={sign}";
            var response = await _http.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            var result = JsonConvert.DeserializeObject<BaiduTranslateResponse>(content);
            if (result?.Error_code == null && result?.Trans_result?.Count > 0)
            {
                var translated = result.Trans_result[0].Dst;
                
                if (_config.BaiduTranslateCacheEnable)
                {
                    lock (_cacheLock)
                    {
                        _cache[text] = translated;
                    }
                }
                
                return translated;
            }
        }
        catch
        {
            // 翻译失败，返回原文
        }

        return text;
    }

    /// <summary>
    /// MD5 哈希
    /// </summary>
    private static string MD5Hash(string input)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// 百度翻译响应
    /// </summary>
    private class BaiduTranslateResponse
    {
        public string? Error_code { get; set; }
        public string? Error_msg { get; set; }
        public List<BaiduTranslateItem>? Trans_result { get; set; }
    }

    private class BaiduTranslateItem
    {
        public string Src { get; set; } = string.Empty;
        public string Dst { get; set; } = string.Empty;
    }
}

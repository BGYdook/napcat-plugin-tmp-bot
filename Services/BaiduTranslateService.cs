using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace NapCatTmpBot.Services;

/// <summary>
/// 百度翻译服务（可选）
/// </summary>
public class BaiduTranslateService
{
    private const string ApiUrl = "https://fanyi-api.baidu.com/api/trans/vip/translate";
    private readonly HttpClient _http;
    private readonly PluginConfig _config;
    private readonly Dictionary<string, string> _cache = new();
    private readonly object _cacheLock = new();

    public BaiduTranslateService(HttpClient http, PluginConfig config)
    {
        _http = http;
        _config = config;
    }

    /// <summary>
    /// 翻译文本（如果配置了百度翻译 API 则使用，否则返回原文）
    /// </summary>
    public async Task<string> TranslateAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        // 检查是否启用了百度翻译
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

    private static string MD5Hash(string input)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

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

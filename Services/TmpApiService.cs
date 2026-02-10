using Newtonsoft.Json;
using NapCatTmpBot.Models;

namespace NapCatTmpBot.Services;

/// <summary>
/// TMP API 服务
/// </summary>
public class TmpApiService
{
    private const string BaseApi = "https://api.truckersmp.com/v2";
    private const string EvmApi = "https://da.vtcm.link";
    private const string TruckyApiBase = "https://api.truckyapp.com";
    private const string TruckyApiProxy = "https://api.codetabs.com/v1/proxy/?quest=https://api.truckyapp.com";
    private readonly HttpClient _http;

    public TmpApiService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// 查询玩家信息
    /// </summary>
    public async Task<ApiResponse<TmpPlayerInfo>> PlayerInfoAsync(long tmpId)
    {
        try
        {
            var response = await _http.GetAsync($"{EvmApi}/player/info?tmpId={tmpId}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<TmpPlayerInfo>>(content);
            
            if (result?.Code == 200 && result.Data != null)
            {
                result.Data.AvatarUrl = $"https://static.truckersmp.com/avatars/{tmpId}.png";
                return new ApiResponse<TmpPlayerInfo> { Code = 200, Data = result.Data };
            }
            
            return new ApiResponse<TmpPlayerInfo> { Code = result?.Code ?? 10001, Message = "玩家不存在" };
        }
        catch
        {
            return new ApiResponse<TmpPlayerInfo> { Code = 500, Message = "查询失败" };
        }
    }

    /// <summary>
    /// 查询玩家在线信息（Trucky API）
    /// </summary>
    public async Task<ApiResponse<PlayerMapInfo>> PlayerMapInfoAsync(long tmpId)
    {
        try
        {
            var url = $"{TruckyApiProxy}/v3/map/online?playerID={tmpId}";
            var response = await _http.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            // Trucky API 直接返回数据，不是 ApiResponse 格式
            var result = JsonConvert.DeserializeObject<PlayerMapInfo>(content);
            
            if (result != null && !string.IsNullOrEmpty(result.Error))
            {
                return new ApiResponse<PlayerMapInfo> { Code = 500, Message = result.Error };
            }
            
            return new ApiResponse<PlayerMapInfo> { Code = 200, Data = result };
        }
        catch
        {
            return new ApiResponse<PlayerMapInfo> { Code = 500, Message = "查询失败" };
        }
    }

    /// <summary>
    /// 查询服务器列表
    /// </summary>
    public async Task<ApiResponse<List<ServerInfo>>> ServerListAsync()
    {
        try
        {
            var response = await _http.GetAsync($"{EvmApi}/server/list");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<List<ServerInfo>>>(content);
            
            return result ?? new ApiResponse<List<ServerInfo>> { Code = 500, Message = "查询失败" };
        }
        catch
        {
            return new ApiResponse<List<ServerInfo>> { Code = 500, Message = "查询失败" };
        }
    }

    /// <summary>
    /// 查询地图玩家列表
    /// </summary>
    public async Task<ApiResponse<List<MapPlayer>>> MapPlayerListAsync(int serverId, double ax, double ay, double bx, double by)
    {
        try
        {
            var url = $"{EvmApi}/map/playerList?aAxisX={ax}&aAxisY={ay}&bAxisX={bx}&bAxisY={by}&serverId={serverId}";
            var response = await _http.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<List<MapPlayer>>>(content);
            
            return result ?? new ApiResponse<List<MapPlayer>> { Code = 500, Message = "查询失败" };
        }
        catch
        {
            return new ApiResponse<List<MapPlayer>> { Code = 500, Message = "查询失败" };
        }
    }

    /// <summary>
    /// 查询 DLC 列表
    /// </summary>
    public async Task<ApiResponse<List<DlcInfo>>> DlcListAsync(string type = "all")
    {
        try
        {
            var response = await _http.GetAsync($"{EvmApi}/dlc/list?type={type}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<List<DlcInfo>>>(content);
            
            return result ?? new ApiResponse<List<DlcInfo>> { Code = 500, Message = "查询失败" };
        }
        catch
        {
            return new ApiResponse<List<DlcInfo>> { Code = 500, Message = "查询失败" };
        }
    }

    /// <summary>
    /// 查询里程排行榜
    /// </summary>
    public async Task<ApiResponse<List<MileageRankingItem>>> MileageRankingListAsync(string rankingType, long? tmpId = null)
    {
        try
        {
            var tmpIdParam = tmpId.HasValue ? $"&tmpId={tmpId.Value}" : "";
            var url = $"{EvmApi}/statistics/mileageRankingList?rankingType={rankingType}{tmpIdParam}&rankingCount=10";
            var response = await _http.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<List<MileageRankingItem>>>(content);
            
            return result ?? new ApiResponse<List<MileageRankingItem>> { Code = 500, Message = "查询失败" };
        }
        catch
        {
            return new ApiResponse<List<MileageRankingItem>> { Code = 500, Message = "查询失败" };
        }
    }

    /// <summary>
    /// 查询游戏版本
    /// </summary>
    public async Task<ApiResponse<object>> VersionAsync()
    {
        try
        {
            var response = await _http.GetAsync($"{BaseApi}/version");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<object>(content);
            
            return new ApiResponse<object> { Code = 200, Data = result };
        }
        catch
        {
            return new ApiResponse<object> { Code = 500, Message = "查询失败" };
        }
    }
}

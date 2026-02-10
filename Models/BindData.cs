namespace NapCatTmpBot.Models;

/// <summary>
/// 绑定数据模型
/// </summary>
public class BindData
{
    /// <summary>
    /// 平台
    /// </summary>
    public string Platform { get; set; } = "qq";

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 绑定的 TMP ID
    /// </summary>
    public long TmpId { get; set; }

    /// <summary>
    /// 绑定时间
    /// </summary>
    public DateTime BindTime { get; set; } = DateTime.UtcNow;
}

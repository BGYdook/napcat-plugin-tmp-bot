namespace NapCatTmpBot.Models;

/// <summary>
/// 命令执行上下文
/// </summary>
public class CommandContext
{
    /// <summary>
    /// 消息内容
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 命令名称
    /// </summary>
    public string CommandName { get; set; } = string.Empty;

    /// <summary>
    /// 解析后的参数
    /// </summary>
    public string Args { get; set; } = string.Empty;

    /// <summary>
    /// 原始参数
    /// </summary>
    public string RawArgs { get; set; } = string.Empty;

    /// <summary>
    /// 平台
    /// </summary>
    public string Platform { get; set; } = "qq";

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 群组ID（私聊时为空）
    /// </summary>
    public long? GroupId { get; set; }

    /// <summary>
    /// 是否群聊
    /// </summary>
    public bool IsGroup => GroupId.HasValue;
}

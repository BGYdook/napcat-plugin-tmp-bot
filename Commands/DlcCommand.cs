using NapCatTmpBot.Models;
using NapCatTmpBot.Services;

namespace NapCatTmpBot.Commands;

/// <summary>
/// DLC 命令
/// </summary>
public static class DlcCommand
{
    private static readonly List<(string name, string nameZh, string game)> DlcList = new()
    {
        // 欧卡 DLC
        ("Scandinavia", "斯堪的纳维亚", "ETS2"),
        ("Going East!", "向东", "ETS2"),
        ("Vive la France!", "法国", "ETS2"),
        ("Italia", "意大利", "ETS2"),
        ("Beyond the Baltic Sea", "波罗的海", "ETS2"),
        ("Road to the Black Sea", "黑海", "ETS2"),
        ("Iberia", "伊比利亚", "ETS2"),
        ("Heart of Russia", "俄罗斯", "ETS2"),
        ("West Balkans", "西巴尔干", "ETS2"),
        // 美卡 DLC
        ("Arizona", "亚利桑那", "ATS"),
        ("New Mexico", "新墨西哥", "ATS"),
        ("Oregon", "俄勒冈", "ATS"),
        ("Washington", "华盛顿", "ATS"),
        ("Utah", "犹他", "ATS"),
        ("Idaho", "爱达荷", "ATS"),
        ("Colorado", "科罗拉多", "ATS"),
        ("Wyoming", "怀俄明", "ATS"),
        ("Montana", "蒙大拿", "ATS"),
        ("Texas", "德克萨斯", "ATS"),
        ("Oklahoma", "俄克拉荷马", "ATS"),
        ("Kansas", "堪萨斯", "ATS"),
        ("Nebraska", "内布拉斯加", "ATS"),
        ("Arkansas", "阿肯色", "ATS"),
        ("Missouri", "密苏里", "ATS"),
        ("Louisiana", "路易斯安那", "ATS"),
        ("Iowa", "爱荷华", "ATS"),
        ("Wisconsin", "威斯康星", "ATS"),
        ("Minnesota", "明尼苏达", "ATS")
    };

    /// <summary>
    /// 执行 DLC 命令
    /// </summary>
    public static async Task<string> Execute(CommandContext context, PluginConfig config, ImageRenderService imageRenderService)
    {
        if (config.DlcListImage)
        {
            try
            {
                var imageData = imageRenderService.GenerateDlcImage(DlcList);
                var imagePath = imageRenderService.SaveToTempFile(imageData, "dlc_");
                return $"[CQ:image,file=file:///{imagePath.Replace("\\", "/")}]";
            }
            catch
            {
                return GenerateTextDlcList();
            }
        }
        else
        {
            return GenerateTextDlcList();
        }
    }

    /// <summary>
    /// 生成文字版 DLC 列表
    /// </summary>
    private static string GenerateTextDlcList()
    {
        var message = new System.Text.StringBuilder();
        message.AppendLine("🗺️ 地图 DLC 列表");
        message.AppendLine();

        var currentGame = string.Empty;
        foreach (var dlc in DlcList)
        {
            if (dlc.game != currentGame)
            {
                currentGame = dlc.game;
                message.AppendLine(currentGame == "ETS2" ? "【欧洲卡车模拟2】" : "【美国卡车模拟】");
            }
            message.AppendLine($"  - {dlc.name} ({dlc.nameZh})");
        }

        return message.ToString();
    }
}

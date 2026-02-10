# NapCatTmpBot

欧洲卡车模拟2 TMP查询机器人 - NapCat 原生插件

## 功能特性

- 🎮 玩家信息查询 - 查询 TMP 玩家的基本信息和游戏数据
- 📍 位置查询 - 实时查询玩家在游戏中的位置信息（带真实地图瓦片）
- 🖥️ 服务器状态 - 查询欧卡/美卡各服务器的在线状态
- 🚦 路况信息 - 查询热门地点的实时路况信息
- 🏆 排行榜 - 查看总里程和今日里程排行榜
- 🔗 绑定功能 - 绑定 TMP ID 后可省略在命令中输入 ID
- 🗺️ DLC 列表 - 列出地图相关 DLC（支持文字和图片输出）
- 🌐 地名翻译 - 内置地名翻译字典（完全免费）

## 支持的命令

| 命令 | 描述 | 用法 |
|------|------|------|
| 绑定 | 绑定 TMP ID | `绑定 123456` |
| 解绑 | 解除 TMP ID 绑定 | `解绑` |
| 查询 | 查询 TMP 玩家信息 | `查询 123456` 或 `查询`（使用绑定ID） |
| 定位 | 查询玩家位置信息 | `定位 123456` 或 `定位`（使用绑定ID） |
| 路况 | 查询热门路况 | `路况 s1\|s2\|p\|a` |
| 服务器 | 查询服务器信息列表 | `服务器` |
| 足迹 | 查看玩家足迹 | `足迹 s1 123456` |
| 排行 | 查看里程排行榜 | `总里程排行` / `今日里程排行` |
| DLC | 列出地图相关 DLC | `DLC列表` 或 `地图DLC` |
| 插件版本 | 查看插件/接口版本信息 | `插件版本` |
| 帮助 | 查看使用说明 | `帮助` 或 `菜单` |

## 支持的服务器简称

- Simulation 1 (简称: s1)
- Simulation 2 (简称: s2)
- ProMods (简称: p)
- Arcade (简称: a)

## 安装方法

### 前置要求

- .NET 8.0 SDK
- NapCat 框架

### 编译插件

```bash
cd /path/to/napcat-plugin-tmp-bot
dotnet build -c Release
```

### 安装到 NapCat

1. 将编译后的 `NapCatTmpBot.dll` 及依赖文件复制到 NapCat 的插件目录
2. 将 `config.json` 复制到插件目录，并根据需要修改配置
3. 重启 NapCat

## 配置说明

```json
{
  "QueryShowAvatarEnable": true,        // 查询时显示头像
  "BaiduTranslateEnable": false,        // 启用百度翻译（可选）
  "BaiduTranslateAppId": "",           // 百度翻译 App ID（可选）
  "BaiduTranslateKey": "",              // 百度翻译密钥（可选）
  "BaiduTranslateCacheEnable": false,   // 启用翻译缓存
  "ApiTimeoutSeconds": 10,              // API 超时时间（秒）
  "PreferVtcmMileage": true,            // 优先使用 VTCM 里程数据
  "EnableBindFeature": true,            // 启用绑定功能
  "DlcListImage": false,                // DLC 列表使用图片输出
  "TmpQueryType": 1,                    // 查询类型（1=图片，2=文字）
  "TmpTrafficType": 1                   // 路况查询类型（1=图片，2=文字）
}
```

## 地名翻译

本插件内置了完整的地名翻译字典，包括：
- 30+ 个国家翻译
- 300+ 个常见城市翻译（覆盖欧卡/美卡主要城市）

**翻译是免费的，无需配置！**

如需扩展翻译范围，可配置百度翻译 API（每月 200 万字符免费额度）：

1. 访问 [百度翻译开放平台](https://fanyi-api.baidu.com/)
2. 注册/登录并开通「通用翻译API」
3. 获取 `App ID` 和 `密钥`
4. 在配置文件中填入并设置 `BaiduTranslateEnable: true`

## 依赖项

- .NET 8.0
- Newtonsoft.Json 13.0.3+
- SkiaSharp 2.88.8+

## 数据来源

- TMP 官方 API: https://api.truckersmp.com/v2
- EVM 开放 API: https://da.vtcm.link
- Trucky App API: https://api.truckyapp.com/v2

## 版本

当前版本: v1.7.4

## 作者

BGYdook, Goodnight_An

## 项目地址

https://github.com/BGYdook/napcat-plugin-tmp-bot

## 许可证

MIT License

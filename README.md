# NapCatTmpBot

欧洲卡车模拟2 TMP查询机器人 - NapCat 原生插件

## 功能特性

- 🎮 玩家信息查询 - 查询 TMP 玩家的基本信息和游戏数据
- 📍 位置查询 - 实时查询玩家在游戏中的位置信息
- 🖥️ 服务器状态 - 查询欧卡/美卡各服务器的在线状态
- 🚦 路况信息 - 查询热门地点的实时路况信息
- 🏆 排行榜 - 查看总里程和今日里程排行榜
- 🔗 绑定功能 - 绑定 TMP ID 后可省略在命令中输入 ID
- 🗺️ DLC 列表 - 列出地图相关 DLC（支持文字和图片输出）

## 支持的命令

| 命令 | 描述 | 用法 |
|------|------|------|
| 绑定 | 绑定 TMP ID | `绑定 123456` |
| 解绑 | 解除 TMP ID 绑定 | `解绑` |
| 查询 | 查询 TMP 玩家信息 | `查询 123456` 或 `查询`（使用绑定ID） |
| 定位 | 查询玩家位置信息 | `定位 123456` 或 `定位`（使用绑定ID） |
| 路况 | 查询热门路况 | `路况 s1\|s2\|p\|a` |
| 服务器 | 查询服务器信息列表 | `服务器` |
| 排行 | 查看里程排行榜 | `总里程排行` / `今日里程排行` |
| DLC | 列出地图相关 DLC | `DLC列表` 或 `地图DLC` |
| 插件版本 | 查看插件/接口版本信息 | `插件版本` |

## 支持的服务器简称

- Simulation 1 (简称: s1)
- Simulation 2 (简称: s2)
- ProMods (简称: p)
- Arcade (简称: a)

## 安装方法

### 编译插件

```bash
cd NapCatTmpBot
dotnet build -c Release
```

### 安装到 NapCat

1. 将编译后的 `NapCatTmpBot.dll` 和依赖文件复制到 NapCat 的插件目录
2. 将 `config.json` 复制到插件目录，并根据需要修改配置
3. 重启 NapCat

## 配置说明

```json
{
  "QueryShowAvatarEnable": true,        // 查询时显示头像
  "BaiduTranslateEnable": false,        // 启用百度翻译
  "BaiduTranslateAppId": "",           // 百度翻译 App ID
  "BaiduTranslateKey": "",              // 百度翻译密钥
  "BaiduTranslateCacheEnable": false,   // 启用翻译缓存
  "ApiTimeoutSeconds": 10,              // API 超时时间（秒）
  "PreferVtcmMileage": true,            // 优先使用 VTCM 里程数据
  "EnableBindFeature": true,            // 启用绑定功能
  "DlcListImage": false,                // DLC 列表使用图片输出
  "TmpQueryType": 1,                    // 查询类型（1=图片，2=文字）
  "TmpTrafficType": 1                   // 路况查询类型（1=图片，2=文字）
}
```

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

## 注意事项

1. 百度翻译功能需要先在 [百度翻译开放平台](https://fanyi-api.baidu.com/) 申请 App ID 和密钥
2. 图片生成功能需要系统安装中文字体，否则可能显示乱码
3. 绑定数据存储在 `data/tmp_binds.json` 文件中
4. 临时图片文件存储在系统临时目录的 `NapCatTmpBot` 子目录中

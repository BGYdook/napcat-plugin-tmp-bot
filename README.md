# napcat-plugin-tmp-bot

欧洲卡车模拟2 TMP查询机器人 - NapCat 原生插件

## 功能特性

- 玩家信息查询 - 查询 TMP 玩家的基本信息和游戏数据
- 位置查询 - 实时查询玩家在游戏中的位置信息
- 服务器状态 - 查询欧卡/美卡各服务器的在线状态
- 路况信息 - 查询热门地点的实时路况信息
- 排行榜 - 查看总里程和今日里程排行榜
- 绑定功能 - 绑定 TMP ID 后可省略在命令中输入 ID
- DLC 列表 - 列出地图相关 DLC(支持文字输出)
- 地名翻译 - 内置地名修正表 + 百度翻译 API(可选)

## 支持的命令

| 命令 | 描述 | 用法 |
|------|------|------|
| /绑定 | 绑定 TMP ID | `/绑定 123456` |
| /解绑 | 解除 TMP ID 绑定 | `/解绑` |
| /查询 | 查询 TMP 玩家信息 | `/查询 123456` 或 `/查询`(使用绑定ID) |
| /定位 | 查询玩家位置信息 | `/定位 123456` 或 `/定位`(使用绑定ID) |
| /路况 | 查询热门路况 | `/路况 s1\|s2\|p\|a` |
| /服务器 | 查询服务器信息列表 | `/服务器` |
| /总里程排行 | 查看总里程排行榜 | `/总里程排行` |
| /今日里程排行 | 查看今日里程排行榜 | `/今日里程排行` |
| /DLC列表 | 列出地图相关 DLC | `/DLC列表` |
| /地图DLC | 列出地图相关 DLC | `/地图DLC` |
| /插件版本 | 查看插件/接口版本信息 | `/插件版本` |
| /帮助 | 查看插件命令帮助 | `/帮助` |

## 支持的服务器简称

- Simulation 1 (简称: s1)
- Simulation 2 (简称: s2)
- ProMods (简称: p)
- Arcade (简称: a)

## 安装方法

### 前置要求

- Node.js 16+
- NapCat 或兼容 OneBot11 的框架

### 安装

```bash
npm install napcat-plugin-tmp-bot
```

## 配置说明

复制 `config/config.json.example` 为 `config/config.json` 并根据需要修改配置:

```json
{
  "queryShowAvatarEnable": true,        // 查询时显示头像
  "baiduTranslateEnable": true,         // 启用百度翻译(默认开启)
  "baiduTranslateAppId": "",           // 百度翻译 App ID(需申请)
  "baiduTranslateKey": "",              // 百度翻译密钥(需申请)
  "baiduTranslateCacheEnable": false,   // 启用翻译缓存
  "apiTimeoutSeconds": 10,              // API 超时时间(秒)
  "preferVtcmMileage": true,            // 优先使用 VTCM 里程数据
  "enableBindFeature": true,            // 启用绑定功能
  "dlcListImage": false,                // DLC 列表使用图片输出
  "tmpQueryType": 1,                    // 查询类型(1=文字)
  "tmpTrafficType": 1                   // 路况查询类型(1=文字)
}
```

## 地名翻译

本插件提供两种翻译方式:

1. 内置地名修正表(优先级最高)
   - 覆盖 30+ 个国家和常见城市
   - 完全免费,无需配置

2. 百度翻译 API(可选)
   - 每月 200 万字符免费额度
   - 用于翻译内置表未覆盖的内容
   - 需访问 [百度翻译开放平台](https://fanyi-api.baidu.com/) 申请
   - 默认已启用,如不需要可设置 `baiduTranslateEnable: false`

## 插件加载

将插件放到 NapCat 的 plugins 目录下,或在 NapCat 配置中启用该插件。

插件会自动创建以下目录:
- `config/` - 配置文件目录
- `data/` - 数据存储目录(绑定信息、翻译缓存)

## 依赖项

- dayjs ^1.11.13
- js-md5 ^0.8.3
- node-fetch ^2.7.0

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

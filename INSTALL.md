# NapCat 插件加载说明

## 方法一: 直接加载

1. 将插件文件夹放到 NapCat 的 `plugins` 目录下
2. 在 NapCat 配置文件中添加插件加载配置

## 方法二: 使用 npm 安装

```bash
npm install napcat-plugin-tmp-bot
```

然后在 NapCat 配置中引用该插件。

## 配置文件位置

插件配置文件位于 `config/config.json`,首次运行会自动创建默认配置。

如需自定义配置,请复制 `config/config.json.example` 为 `config/config.json` 并修改相应配置。

## 数据存储

插件会在 `data/` 目录下创建以下文件:
- `bind.json` - 用户 TMP ID 绑定数据
- `translate_cache.json` - 翻译缓存数据

这些文件会被 gitignore 忽略,不会提交到仓库。
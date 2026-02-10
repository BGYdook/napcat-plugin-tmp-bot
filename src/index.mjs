import { createRequire } from 'module';
import * as fs from 'fs/promises';
import * as path from 'path';

const require = createRequire(import.meta.url);

const tmpBind = require('./command/tmpBind');
const tmpQuery = require('./command/tmpQuery/tmpQuery');
const tmpPosition = require('./command/tmpPosition');
const tmpTraffic = require('./command/tmpTraffic/tmpTraffic');
const tmpServer = require('./command/tmpServer');
const tmpVersion = require('./command/tmpVersion');
const tmpDlcMap = require('./command/tmpDlcMap');
const tmpMileageRanking = require('./command/tmpMileageRanking');

let config = {};
const BIND_FILE_NAME = 'bind.json';
const TRANSLATE_CACHE_FILE_NAME = 'translate_cache.json';

export const plugin_init = async (ctx) => {
  ctx.logger.log('【TMP查询机器人】插件加载中...');

  await initDataDir(ctx);
  await loadConfig(ctx);

  ctx.logger.log('【TMP查询机器人】插件加载完成');
};

export const plugin_onmessage = async (ctx, event) => {
  if (event.post_type !== 'message') return;

  const message = event.raw_message || event.message;
  const parsed = parseCommand(message);
  if (!parsed) return;

  const { cmd, args } = parsed;

  let result = '';

  try {
    switch (cmd) {
      case '绑定':
        if (config.enableBindFeature !== false) {
          const tmpId = extractArg(args, 'number');
          const session = createSession(ctx, event);
          result = await tmpBind(ctx, config, session, tmpId);
        } else {
          result = '绑定功能已禁用';
        }
        break;

      case '解绑':
        if (config.enableBindFeature !== false) {
          const session = createSession(ctx, event);
          const bindFilePath = path.join(ctx.dataPath, BIND_FILE_NAME);
          const bindData = JSON.parse(await fs.readFile(bindFilePath, 'utf8'));
          const key = `${session.platform}:${session.userId}`;
          delete bindData[key];
          await fs.writeFile(bindFilePath, JSON.stringify(bindData, null, 2));
          result = '解绑成功';
        } else {
          result = '绑定功能已禁用';
        }
        break;

      case '查询':
        const queryId = extractArg(args, 'number');
        const session = createSession(ctx, event);
        result = await tmpQuery(ctx, config, session, queryId);
        break;

      case '定位':
        const posId = extractArg(args, 'number');
        const session2 = createSession(ctx, event);
        result = await tmpPosition(ctx, config, session2, posId);
        break;

      case '路况':
        const serverName = extractArg(args, 'string');
        result = await tmpTraffic(ctx, config, serverName);
        break;

      case '服务器':
        result = await tmpServer(ctx);
        break;

      case '插件版本':
        result = await tmpVersion(ctx);
        break;

      case 'DLC列表':
      case '地图DLC':
        const session3 = createSession(ctx, event);
        result = await tmpDlcMap(ctx, session3);
        break;

      case '总里程排行':
        const session4 = createSession(ctx, event);
        result = await tmpMileageRanking(ctx, session4, 'total');
        break;

      case '今日里程排行':
        const session5 = createSession(ctx, event);
        result = await tmpMileageRanking(ctx, session5, 'today');
        break;

      case '帮助':
        result = getHelpMessage();
        break;

      default:
        return;
    }

    if (result) {
      await sendReply(ctx, event, result);
    }
  } catch (err) {
    ctx.logger.error('处理命令失败:', err);
    const errorMsg = '命令执行失败: ' + (err?.message || '未知错误');
    await sendReply(ctx, event, errorMsg);
  }
};

export const plugin_cleanup = (ctx) => {
  ctx.logger.log('【TMP查询机器人】插件已卸载');
};

async function initDataDir(ctx) {
  try {
    await fs.mkdir(ctx.dataPath, { recursive: true });

    const bindFilePath = path.join(ctx.dataPath, BIND_FILE_NAME);
    const cacheFilePath = path.join(ctx.dataPath, TRANSLATE_CACHE_FILE_NAME);

    const bindExists = await fs.access(bindFilePath).then(() => true).catch(() => false);
    if (!bindExists) {
      await fs.writeFile(bindFilePath, '{}');
    }

    const cacheExists = await fs.access(cacheFilePath).then(() => true).catch(() => false);
    if (!cacheExists) {
      await fs.writeFile(cacheFilePath, '{}');
    }
  } catch (err) {
    ctx.logger.error('初始化数据目录失败:', err);
  }
}

async function loadConfig(ctx) {
  try {
    const data = await fs.readFile(ctx.configPath, 'utf8');
    config = JSON.parse(data);

    if (config.tmpQueryType == null) config.tmpQueryType = 1;
    if (config.tmpTrafficType == null) config.tmpTrafficType = 1;

    return config;
  } catch (err) {
    ctx.logger.error('加载配置失败:', err);
    return {};
  }
}

function createSession(ctx, event) {
  return {
    platform: 'qq',
    userId: String(event.user_id),
    groupId: event.group_id ? String(event.group_id) : null,
    isPrivate: event.message_type === 'private'
  };
}

function parseCommand(message) {
  const cmdMatch = message.match(/^\/(\S+)(?:\s+(.*))?$/);
  if (!cmdMatch) return null;

  const [, cmd, args] = cmdMatch;
  return { cmd, args: args || '' };
}

function extractArg(args, type) {
  if (!args) return null;
  args = args.trim();

  if (type === 'number') {
    const num = parseInt(args);
    return isNaN(num) ? null : num;
  } else if (type === 'string') {
    return args;
  }

  return args;
}

async function sendReply(ctx, event, message) {
  const params = {
    message: message,
    message_type: event.message_type,
    ...(event.message_type === 'group' && event.group_id
      ? { group_id: String(event.group_id) }
      : {}),
    ...(event.message_type === 'private' && event.user_id
      ? { user_id: String(event.user_id) }
      : {}),
  };

  await ctx.actions.call('send_msg', params, ctx.adapterName, ctx.pluginManager.config);
}

function getHelpMessage() {
  return `【TMP查询机器人 命令帮助】
/绑定 [TMPID] - 绑定 TMP ID
/解绑 - 解除 TMP ID 绑定
/查询 [TMPID] - 查询玩家信息
/定位 [TMPID] - 查询玩家位置
/路况 [服务器] - 查询路况(s1/s2/p/a)
/服务器 - 查看服务器列表
/总里程排行 - 查看总里程排行榜
/今日里程排行 - 查看今日里程排行榜
/DLC列表 - 查看地图DLC
/插件版本 - 查看插件版本
/帮助 - 显示此帮助信息`;
}

export let plugin_config_ui = [
  {
    key: 'queryShowAvatarEnable',
    label: '查询时显示头像',
    type: 'boolean',
    default: true,
    description: '查询玩家信息时是否显示头像'
  },
  {
    key: 'baiduTranslateEnable',
    label: '启用百度翻译',
    type: 'boolean',
    default: true,
    description: '是否使用百度翻译 API'
  },
  {
    key: 'baiduTranslateAppId',
    label: '百度翻译 App ID',
    type: 'string',
    default: '',
    placeholder: '请输入百度翻译 App ID',
    description: '百度翻译开放平台申请的 App ID'
  },
  {
    key: 'baiduTranslateKey',
    label: '百度翻译密钥',
    type: 'string',
    default: '',
    placeholder: '请输入百度翻译密钥',
    description: '百度翻译开放平台申请的密钥'
  },
  {
    key: 'baiduTranslateCacheEnable',
    label: '启用翻译缓存',
    type: 'boolean',
    default: false,
    description: '是否缓存翻译结果以减少 API 调用'
  },
  {
    key: 'apiTimeoutSeconds',
    label: 'API 超时时间(秒)',
    type: 'number',
    default: 10,
    description: '外部 API 请求的超时时间'
  },
  {
    key: 'preferVtcmMileage',
    label: '优先使用 VTCM 里程',
    type: 'boolean',
    default: true,
    description: '查询里程时优先使用 VTCM 数据源'
  },
  {
    key: 'enableBindFeature',
    label: '启用绑定功能',
    type: 'boolean',
    default: true,
    description: '是否允许用户绑定 TMP ID'
  },
  {
    key: 'tmpQueryType',
    label: '查询输出类型',
    type: 'select',
    default: 1,
    description: '查询信息的输出格式',
    options: [
      { label: '文字', value: 1 }
    ]
  },
  {
    key: 'tmpTrafficType',
    label: '路况输出类型',
    type: 'select',
    default: 1,
    description: '路况信息的输出格式',
    options: [
      { label: '文字', value: 1 }
    ]
  }
];
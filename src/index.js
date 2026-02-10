const fs = require('fs').promises;
const path = require('path');
const tmpBind = require('./command/tmpBind');
const tmpQuery = require('./command/tmpQuery/tmpQuery');
const tmpPosition = require('./command/tmpPosition');
const tmpTraffic = require('./command/tmpTraffic/tmpTraffic');
const tmpServer = require('./command/tmpServer');
const tmpVersion = require('./command/tmpVersion');
const tmpDlcMap = require('./command/tmpDlcMap');
const tmpMileageRanking = require('./command/tmpMileageRanking');

const DATA_DIR = path.join(__dirname, '../data');
const CONFIG_FILE = path.join(__dirname, '../config/config.json');
const BIND_FILE = path.join(DATA_DIR, 'bind.json');
const TRANSLATE_CACHE_FILE = path.join(DATA_DIR, 'translate_cache.json');

let config = {};

async function initDataDir() {
  try {
    await fs.mkdir(DATA_DIR, { recursive: true });
    await fs.mkdir(path.join(__dirname, '../config'), { recursive: true });

    const bindExists = await fs.access(BIND_FILE).then(() => true).catch(() => false);
    if (!bindExists) {
      await fs.writeFile(BIND_FILE, '{}');
    }

    const cacheExists = await fs.access(TRANSLATE_CACHE_FILE).then(() => true).catch(() => false);
    if (!cacheExists) {
      await fs.writeFile(TRANSLATE_CACHE_FILE, '{}');
    }

    const configExists = await fs.access(CONFIG_FILE).then(() => true).catch(() => false);
    if (!configExists) {
      const defaultConfig = {
        queryShowAvatarEnable: true,
        baiduTranslateEnable: true,
        baiduTranslateAppId: '',
        baiduTranslateKey: '',
        baiduTranslateCacheEnable: false,
        apiTimeoutSeconds: 10,
        preferVtcmMileage: true,
        enableBindFeature: true,
        dlcListImage: false,
        tmpQueryType: 1,
        tmpTrafficType: 1
      };
      await fs.writeFile(CONFIG_FILE, JSON.stringify(defaultConfig, null, 2));
    }
  } catch (err) {
    console.error('初始化数据目录失败:', err);
  }
}

async function loadConfig() {
  try {
    const data = await fs.readFile(CONFIG_FILE, 'utf8');
    config = JSON.parse(data);
    return config;
  } catch (err) {
    console.error('加载配置失败:', err);
    return {};
  }
}

async function getBindData() {
  try {
    const data = await fs.readFile(BIND_FILE, 'utf8');
    return JSON.parse(data);
  } catch (err) {
    return {};
  }
}

async function saveBindData(data) {
  try {
    await fs.writeFile(BIND_FILE, JSON.stringify(data, null, 2));
  } catch (err) {
    console.error('保存绑定数据失败:', err);
  }
}

async function getTranslateCache() {
  try {
    const data = await fs.readFile(TRANSLATE_CACHE_FILE, 'utf8');
    return JSON.parse(data);
  } catch (err) {
    return {};
  }
}

async function saveTranslateCache(data) {
  try {
    await fs.writeFile(TRANSLATE_CACHE_FILE, JSON.stringify(data, null, 2));
  } catch (err) {
    console.error('保存翻译缓存失败:', err);
  }
}

async function getBindTmpId(platform, userId) {
  const bindData = await getBindData();
  const key = `${platform}:${userId}`;
  return bindData[key];
}

async function setBindTmpId(platform, userId, tmpId) {
  const bindData = await getBindData();
  const key = `${platform}:${userId}`;
  bindData[key] = tmpId;
  await saveBindData(bindData);
}

async function removeBindTmpId(platform, userId) {
  const bindData = await getBindData();
  const key = `${platform}:${userId}`;
  delete bindData[key];
  await saveBindData(bindData);
}

const ctx = {
  config: () => config,
  database: {
    get: getBindTmpId,
    set: setBindTmpId,
    remove: removeBindTmpId,
    getCache: getTranslateCache,
    setCache: saveTranslateCache
  }
};

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

async function handleMessage(event, api) {
  await loadConfig();

  if (event.message_type !== 'private' && event.message_type !== 'group') {
    return;
  }

  const message = event.raw_message || event.message;
  const parsed = parseCommand(message);
  if (!parsed) return;

  const { cmd, args } = parsed;
  const session = {
    platform: 'qq',
    userId: String(event.user_id),
    groupId: event.group_id ? String(event.group_id) : null,
    isPrivate: event.message_type === 'private',
    send: async (msg) => {
      if (event.group_id) {
        await api.send_group_msg(group_id: event.group_id, message: msg);
      } else {
        await api.send_private_msg(user_id: event.user_id, message: msg);
      }
    }
  };

  let result = '';

  try {
    switch (cmd) {
      case '绑定':
        if (config.enableBindFeature !== false) {
          const tmpId = extractArg(args, 'number');
          result = await tmpBind(ctx, config, session, tmpId);
        } else {
          result = '绑定功能已禁用';
        }
        break;

      case '解绑':
        if (config.enableBindFeature !== false) {
          await removeBindTmpId(session.platform, session.userId);
          result = '解绑成功';
        } else {
          result = '绑定功能已禁用';
        }
        break;

      case '查询':
        const queryId = extractArg(args, 'number');
        result = await tmpQuery(ctx, config, session, queryId);
        break;

      case '定位':
        const posId = extractArg(args, 'number');
        result = await tmpPosition(ctx, config, session, posId);
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
        result = await tmpDlcMap(ctx, session);
        break;

      case '总里程排行':
        result = await tmpMileageRanking(ctx, session, 'total');
        break;

      case '今日里程排行':
        result = await tmpMileageRanking(ctx, session, 'today');
        break;

      case '帮助':
        result = getHelpMessage();
        break;

      default:
        return;
    }

    if (result) {
      if (event.group_id) {
        await api.send_group_msg(group_id: event.group_id, message: result);
      } else {
        await api.send_private_msg(user_id: event.user_id, message: result);
      }
    }
  } catch (err) {
    console.error('处理命令失败:', err);
    const errorMsg = '命令执行失败: ' + (err.message || '未知错误');
    if (event.group_id) {
      await api.send_group_msg(group_id: event.group_id, message: errorMsg);
    } else {
      await api.send_private_msg(user_id: event.user_id, message: errorMsg);
    }
  }
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

class NapCatTmpBotPlugin {
  constructor(napcat) {
    this.napcat = napcat;
    this.name = 'napcat-tmp-bot';
    this.version = '1.7.4';
  }

  async onLoad() {
    console.log('【TMP查询机器人】插件加载中...');
    await initDataDir();
    await loadConfig();

    this.napcat.on('message', async (event) => {
      await handleMessage(event, this.napcat.api);
    });

    console.log('【TMP查询机器人】插件加载完成');
  }

  async onUnload() {
    console.log('【TMP查询机器人】插件已卸载');
  }
}

module.exports = NapCatTmpBotPlugin;
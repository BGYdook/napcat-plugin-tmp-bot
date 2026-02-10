/**
 * NapCat 插件 - TMP查询机器人
 */

import * as fs from 'fs/promises';
import * as path from 'path';

let config = {};
const BIND_FILE_NAME = 'bind.json';
const TRANSLATE_CACHE_FILE_NAME = 'translate_cache.json';

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

async function httpGet(url, timeout = 10000) {
  try {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), timeout);
    const response = await fetch(url, { signal: controller.signal });
    clearTimeout(timeoutId);
    return await response.json();
  } catch (err) {
    if (err.name === 'AbortError') {
      return { error: true, message: '请求超时' };
    }
    return { error: true, message: err.message };
  }
}

async function handleBind(ctx, cfg, session, tmpId) {
  if (!tmpId || isNaN(tmpId)) {
    return `请输入正确的玩家编号`;
  }

  const BASE_API = 'https://api.truckersmp.com/v2';
  const result = await httpGet(`${BASE_API}/player/${tmpId}`);

  if (result.error) {
    return '绑定失败 (查询玩家信息失败)';
  }

  const bindFilePath = path.join(ctx.dataPath, BIND_FILE_NAME);
  const bindData = JSON.parse(await fs.readFile(bindFilePath, 'utf8'));
  const key = `${session.platform}:${session.userId}`;
  bindData[key] = tmpId;
  await fs.writeFile(bindFilePath, JSON.stringify(bindData, null, 2));

  return `绑定成功 ( ${result.response.name} )`;
}

async function handleQuery(ctx, cfg, session, tmpId) {
  if (tmpId && isNaN(tmpId)) {
    return `请输入正确的玩家编号`;
  }

  if (!tmpId) {
    const bindFilePath = path.join(ctx.dataPath, BIND_FILE_NAME);
    const bindData = JSON.parse(await fs.readFile(bindFilePath, 'utf8'));
    const key = `${session.platform}:${session.userId}`;
    if (!bindData[key]) {
      return `请输入正确的玩家编号`;
    }
    tmpId = bindData[key];
  }

  const BASE_API = 'https://da.vtcm.link';
  const result = await httpGet(`${BASE_API}/player/info?tmpId=${tmpId}`);

  if (result.error) {
    return '查询玩家信息失败,请重试';
  }

  const dayjs = await import('dayjs');
  const playerInfo = result.data;

  let message = '';
  if (cfg.queryShowAvatarEnable) {
    message += `[CQ:image,file=${playerInfo.avatarUrl}]\n`;
  }
  message += '🆔TMP编号: ' + playerInfo.tmpId;
  message += '\n😀玩家名称: ' + playerInfo.name;
  message += '\n🎮SteamID: ' + playerInfo.steamId;
  const registerDate = dayjs.default(playerInfo.registerTime);
  message += '\n📑注册日期: ' + registerDate.format('YYYY年MM月DD日');
  message += '\n💼所属分组: ' + (playerInfo.groupName || '玩家');
  message += '\n🚫是否封禁: ' + (playerInfo.isBan ? '是' : '否');
  if (playerInfo.mileage) {
    let mileage = playerInfo.mileage;
    let mileageUnit = '米';
    if (mileage > 1000) {
      mileage = (mileage / 1000).toFixed(1);
      mileageUnit = '公里';
    }
    message += '\n🚩历史里程: ' + mileage + mileageUnit;
  }

  return message;
}

async function handlePosition(ctx, cfg, session, tmpId) {
  if (tmpId && isNaN(tmpId)) {
    return `请输入正确的玩家编号`;
  }

  if (!tmpId) {
    const bindFilePath = path.join(ctx.dataPath, BIND_FILE_NAME);
    const bindData = JSON.parse(await fs.readFile(bindFilePath, 'utf8'));
    const key = `${session.platform}:${session.userId}`;
    if (!bindData[key]) {
      return `请输入正确的玩家编号`;
    }
    tmpId = bindData[key];
  }

  const BASE_API = 'https://api.codetabs.com/v1/proxy/?quest=https://api.truckyapp.com';
  const result = await httpGet(`${BASE_API}/v3/map/online?playerID=${tmpId}`);

  if (result.error || !result.response) {
    return '查询玩家信息失败,请重试';
  }

  if (!result.response.online) {
    return '玩家离线';
  }

  let message = '【玩家位置信息】\n';
  message += '🆔TMP编号: ' + tmpId + '\n';
  message += '🎮服务器: ' + (result.response.serverDetails?.name || '未知') + '\n';
  message += '🌍位置: ';
  message += (result.response.location?.poi?.country || '未知');
  message += ' - ';
  message += (result.response.location?.poi?.realName || '未知') + '\n';
  message += '📍坐标: ' + (result.response.x ? Math.round(result.response.x) : '?') + ', ' + (result.response.y ? Math.round(result.response.y) : '?');

  return message;
}

async function handleTraffic(ctx, cfg, serverName) {
  const serverNameAlias = {
    's1': 'sim1',
    's2': 'sim2',
    'p': 'eupromods1',
    'a': 'arc1'
  };

  let serverQueryName = serverNameAlias[serverName];
  if (!serverQueryName) {
    return '请输入正确的服务器名称 (s1, s2, p, a)';
  }

  const BASE_API = 'https://api.codetabs.com/v1/proxy/?quest=https://api.truckyapp.com';
  const result = await httpGet(`${BASE_API}/v2/traffic/top?game=ets2&server=${serverQueryName}`);

  if (result.error) {
    return '查询路况信息失败';
  }

  let message = '';
  const severityToZh = {
    'Fluid': '🟢畅通',
    'Moderate': '🟠正常',
    'Congested': '🔴缓慢',
    'Heavy': '🟣拥堵'
  };

  for (const traffic of result.response) {
    if (message) message += '\n\n';
    message += traffic.country || '未知';
    message += ' - ';
    const name = traffic.name.substring(0, traffic.name.lastIndexOf('(') - 1);
    message += name;
    message += '\n路况: ' + (severityToZh[traffic.newSeverity] || traffic.color || '未知');
    message += ' | 人数: ' + traffic.players;
  }

  return message;
}

async function handleServer(ctx) {
  const BASE_API = 'https://da.vtcm.link';
  const result = await httpGet(`${BASE_API}/server/list`);

  if (result.error) {
    return '查询服务器失败,请稍后重试';
  }

  let message = '';
  for (let server of result.data) {
    if (message) message += '\n\n';
    message += '服务器: ' + ( server.isOnline === 1 ? '🟢' : '⚫' ) + server.serverName;
    message += `\n玩家人数: ${server.playerCount}/${server.maxPlayer}`;
    if (server.queue) {
      message += ` (队列: ${server.queueCount})`;
    }
    let characteristicList = [];
    if (!(server.afkEnable === 1)) {
      characteristicList.push('⏱挂机');
    }
    if (server.collisionsEnable === 1) {
      characteristicList.push('💥碰撞');
    }
    if (characteristicList && characteristicList.length > 0) {
      message += '\n服务器特性: ' + characteristicList.join(' ');
    }
  }
  return message;
}

async function handleVersion(ctx) {
  const BASE_API = 'https://api.truckersmp.com/v2';
  const result = await httpGet(`${BASE_API}/version`);

  if (result.error) {
    return '查询失败,请稍后再试';
  }

  let message = '';
  message += `TMP版本:${result.name}\n`;
  message += `欧卡支持版本: ${result.supported_game_version}\n`;
  message += `美卡支持版本: ${result.supported_ats_game_version}`;
  return message;
}

async function handleDlcMap(ctx, session) {
  const BASE_API = 'https://da.vtcm.link';
  const dlcData = await httpGet(`${BASE_API}/dlc/list?type=1`);

  if (dlcData.error) {
    return '查询DLC数据失败,请稍后重试';
  }

  let message = '【地图DLC列表】\n\n';
  for (const dlc of dlcData.data) {
    message += dlc.name + '\n';
  }

  return message;
}

async function handleMileageRanking(ctx, session, rankingType) {
  const BASE_API = 'https://da.vtcm.link';
  const mileageRankingList = await httpGet(`${BASE_API}/statistics/mileageRankingList?rankingType=${rankingType}&rankingCount=10`);

  if (mileageRankingList.error) {
    return '查询排行榜信息失败';
  } else if (!mileageRankingList.data || mileageRankingList.data.length === 0) {
    return '暂无数据';
  }

  let title = rankingType === 'total' ? '【总里程排行榜】' : '【今日里程排行榜】';
  let message = title + '\n\n';

  for (let i = 0; i < Math.min(10, mileageRankingList.data.length); i++) {
    const player = mileageRankingList.data[i];
    message += `#${i + 1} ${player.name}\n`;
    let mileage = rankingType === 'total' ? player.mileage : player.todayMileage;
    let unit = '米';
    if (mileage > 1000) {
      mileage = (mileage / 1000).toFixed(1);
      unit = '公里';
    }
    message += `里程: ${mileage}${unit}\n`;
  }

  return message;
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

const plugin_init = async (ctx) => {
  ctx.logger.log('【TMP查询机器人】插件加载中...');

  await initDataDir(ctx);
  await loadConfig(ctx);

  ctx.logger.log('【TMP查询机器人】插件加载完成');
};

const plugin_onmessage = async (ctx, event) => {
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
          result = await handleBind(ctx, config, session, tmpId);
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
        result = await handleQuery(ctx, config, session, queryId);
        break;

      case '定位':
        const posId = extractArg(args, 'number');
        const session2 = createSession(ctx, event);
        result = await handlePosition(ctx, config, session2, posId);
        break;

      case '路况':
        const serverName = extractArg(args, 'string');
        result = await handleTraffic(ctx, config, serverName);
        break;

      case '服务器':
        result = await handleServer(ctx);
        break;

      case '插件版本':
        result = await handleVersion(ctx);
        break;

      case 'DLC列表':
      case '地图DLC':
        const session3 = createSession(ctx, event);
        result = await handleDlcMap(ctx, session3);
        break;

      case '总里程排行':
        const session4 = createSession(ctx, event);
        result = await handleMileageRanking(ctx, session4, 'total');
        break;

      case '今日里程排行':
        const session5 = createSession(ctx, event);
        result = await handleMileageRanking(ctx, session5, 'today');
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

const plugin_cleanup = (ctx) => {
  ctx.logger.log('【TMP查询机器人】插件已卸载');
};

export const plugin_config_ui = [
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

export { plugin_init, plugin_onmessage, plugin_cleanup };
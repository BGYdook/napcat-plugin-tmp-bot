const fetch = require('node-fetch');
const md5 = require('js-md5');
const translateCache = require('../database/translateCache');

const USER_GROUP_MAP = {
  'Player': '玩家',
  'Retired Legend': '退役',
  'Game Developer': '游戏开发者',
  'Retired Team Member': '退休团队成员',
  'Add-On Team': '附加组件团队',
  'Game Moderator': '游戏管理员'
};

const LOCATION_CORRECTIONS = {
  'United Kingdom': '英国',
  'Germany': '德国',
  'France': '法国',
  'Italy': '意大利',
  'Spain': '西班牙',
  'Poland': '波兰',
  'Czech Republic': '捷克',
  'Belgium': '比利时',
  'Netherlands': '荷兰',
  'Austria': '奥地利',
  'Switzerland': '瑞士',
  'Hungary': '匈牙利',
  'Romania': '罗马尼亚',
  'Bulgaria': '保加利亚',
  'Norway': '挪威',
  'Sweden': '瑞典',
  'Denmark': '丹麦',
  'Finland': '芬兰',
  'Portugal': '葡萄牙',
  'Turkey': '土耳其',
  'Russia': '俄罗斯',
  'Lithuania': '立陶宛',
  'Latvia': '拉脱维亚',
  'Estonia': '爱沙尼亚',
  'Slovenia': '斯洛文尼亚',
  'Slovakia': '斯洛伐克',
  'Croatia': '克罗地亚',
  'Serbia': '塞尔维亚',
  'Luxembourg': '卢森堡',
  'Greece': '希腊',
  'United States': '美国',
  'USA': '美国',
  'Canada': '加拿大',
  'Mexico': '墨西哥'
};

module.exports = async (ctx, cfg, text) => {
  if (!text) return text;
  text = text.toString().trim();
  if (!text) return text;

  if (LOCATION_CORRECTIONS[text]) {
    return LOCATION_CORRECTIONS[text];
  }

  if (!cfg.baiduTranslateEnable || !cfg.baiduTranslateAppId || !cfg.baiduTranslateKey) {
    return text;
  }

  if (cfg.baiduTranslateCacheEnable) {
    const cacheKey = md5(text);
    const cached = await translateCache.getTranslate(ctx.database, cacheKey);
    if (cached) return cached;
  }

  try {
    const salt = Date.now().toString();
    const sign = md5(cfg.baiduTranslateAppId + text + salt + cfg.baiduTranslateKey);

    const url = `https://fanyi-api.baidu.com/api/trans/vip/translate?q=${encodeURIComponent(text)}&from=auto&to=zh&appid=${cfg.baiduTranslateAppId}&salt=${salt}&sign=${sign}`;
    const result = await fetch(url, { timeout: 10000 });
    const data = await result.json();

    if (data && data.trans_result && data.trans_result.length > 0) {
      const translated = data.trans_result[0].dst;

      if (cfg.baiduTranslateCacheEnable) {
        const cacheKey = md5(text);
        translateCache.save(ctx.database, cacheKey, text, translated);
      }

      return translated;
    }
  } catch (e) {}

  return text;
};

module.exports.USER_GROUP_MAP = USER_GROUP_MAP;

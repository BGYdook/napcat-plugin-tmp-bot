const guildBind = require('../database/guildBind');
const truckyAppApi = require('../api/truckyAppApi');
const truckersMpApi = require('../api/truckersMpApi');
const evmOpenApi = require('../api/evmOpenApi');
const baiduTranslate = require('../util/baiduTranslate');

module.exports = async (ctx, cfg, session, tmpId) => {
  if (tmpId && isNaN(tmpId)) {
    return `请输入正确的玩家编号`;
  }

  if (!tmpId) {
    let guildBindData = await guildBind.get(ctx.database, session.platform, session.userId);
    if (!guildBindData) {
      return `请输入正确的玩家编号`;
    }
    tmpId = guildBindData.tmp_id;
  }

  let playerInfo = await truckersMpApi.player(ctx.http, tmpId);
  if (playerInfo.error) {
    return '查询玩家信息失败,请重试';
  }

  let playerMapInfo = await truckyAppApi.online(ctx.http, tmpId);
  if (playerMapInfo.error) {
    return '查询玩家信息失败,请重试';
  }
  if (!playerMapInfo.data.online) {
    return '玩家离线';
  }

  let areaPlayersData = await evmOpenApi.mapPlayerList(ctx.http, playerMapInfo.data.server,
      playerMapInfo.data.x - 4000,
      playerMapInfo.data.y + 2500,
      playerMapInfo.data.x + 4000,
      playerMapInfo.data.y - 2500);
  let areaPlayerList = [];
  if (!areaPlayersData.error) {
    areaPlayerList = areaPlayersData.data;
    let index = areaPlayerList.findIndex((player) => {
      return player.tmpId.toString() === tmpId.toString();
    });
    if (index !== -1) {
      areaPlayerList.splice(index, 1);
    }
  }

  let message = '【玩家位置信息】\n';
  message += '🆔TMP编号: ' + playerInfo.data.tmpId + '\n';
  message += '😀玩家名称: ' + playerInfo.data.name + '\n';
  message += '🎮服务器: ' + playerMapInfo.data.serverDetails.name + '\n';
  message += '🌍位置: ';
  message += await baiduTranslate(ctx, cfg, playerMapInfo.data.location.poi.country);
  message += ' - ';
  message += await baiduTranslate(ctx, cfg, playerMapInfo.data.location.poi.realName) + '\n';
  message += '📍坐标: X=' + Math.round(playerMapInfo.data.x) + ', Y=' + Math.round(playerMapInfo.data.y) + '\n';
  if (areaPlayerList.length > 0) {
    message += '👥周边玩家: ' + areaPlayerList.length + '人';
  }

  return message;
}

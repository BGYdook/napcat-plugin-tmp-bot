const guildBind = require('../database/guildBind');
const truckersMpApi = require("../api/truckersMpApi");

module.exports = async (ctx, cfg, session, tmpId) => {
  if (!tmpId || isNaN(tmpId)) {
    return `请输入正确的玩家编号`;
  }

  const playerInfo = await truckersMpApi.player(ctx, tmpId);
  if (playerInfo.error) {
    return '绑定失败 (查询玩家信息失败)';
  }

  await guildBind.saveOrUpdate(ctx.database, session.platform, session.userId, tmpId);

  return `绑定成功 ( ${playerInfo.data.name} )`;
}

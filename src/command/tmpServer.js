const truckersMpApi = require('../api/truckersMpApi');
const evmOpenApi = require('../api/evmOpenApi');

module.exports = async (ctx) => {
  let serverData = await evmOpenApi.serverList(ctx.http);
  if (serverData.error) {
    return '查询服务器失败,请稍后重试';
  }

  let message = '';
  for (let server of serverData.data) {
    if (message) {
      message += '\n\n';
    }

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

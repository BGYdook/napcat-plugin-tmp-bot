const evmOpenApi = require('../api/evmOpenApi');

module.exports = async (ctx, session) => {
  let dlcData = await evmOpenApi.dlcList(ctx.http, 1);

  if (dlcData.error) {
    return '查询DLC数据失败,请稍后重试';
  }

  let message = '【地图DLC列表】\n\n';
  for (const dlc of dlcData.data) {
    message += dlc.name + '\n';
  }

  return message;
}

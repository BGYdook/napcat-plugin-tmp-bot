const truckersMpApi = require("../api/truckersMpApi");

module.exports = async (ctx) => {
  let result = await truckersMpApi.version(ctx.http);
  if (result.error) {
    return '查询失败,请稍后再试';
  }

  let message = '';
  message += `TMP版本:${result.data.name}\n`;
  message += `欧卡支持版本: ${result.data.supported_game_version}\n`;
  message += `美卡支持版本: ${result.data.supported_ats_game_version}`;
  return message;
}

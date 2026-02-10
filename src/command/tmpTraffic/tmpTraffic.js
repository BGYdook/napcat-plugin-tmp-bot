const tmpTrafficText = require("./tmpTrafficText");

module.exports = async (ctx, cfg, serverName) => {
  switch (cfg.tmpTrafficType) {
    case 1:
      return await tmpTrafficText(ctx, cfg, serverName);
    case 2:
      return await tmpTrafficText(ctx, cfg, serverName);
    default:
      return '指令配置错误';
  }
}

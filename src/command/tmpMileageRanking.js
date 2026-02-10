const evmOpenApi = require('../api/evmOpenApi');
const guildBind = require('../database/guildBind');

module.exports = async (ctx, session, rankingType) => {
  let mileageRankingList = await evmOpenApi.mileageRankingList(ctx.http, rankingType, null);
  if (mileageRankingList.error) {
    return '查询排行榜信息失败';
  } else if (mileageRankingList.data.length === 0) {
    return '暂无数据';
  }

  let guildBindData = await guildBind.get(ctx.database, session.platform, session.userId);
  let playerMileageRanking = null;
  if (guildBindData) {
    let playerMileageRankingResult = await evmOpenApi.mileageRankingList(ctx.http, rankingType, guildBindData.tmp_id);
    if (!playerMileageRankingResult.error && playerMileageRankingResult.data.length > 0) {
      playerMileageRanking = playerMileageRankingResult.data[0];
    }
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

  if (playerMileageRanking) {
    message += '\n【我的排名】\n';
    message += `排名: #${playerMileageRanking.rank || 'N/A'}\n`;
    let mileage = rankingType === 'total' ? playerMileageRanking.mileage : playerMileageRanking.todayMileage;
    let unit = '米';
    if (mileage > 1000) {
      mileage = (mileage / 1000).toFixed(1);
      unit = '公里';
    }
    message += `里程: ${mileage}${unit}`;
  }

  return message;
}

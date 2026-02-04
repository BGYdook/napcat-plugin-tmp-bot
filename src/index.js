const { Schema } = require('koishi')
const guildModel = require('./database/model')
const tmpBind = require('./command/tmpBind')
const tmpQuery = require('./command/tmpQuery/tmpQuery')
const tmpPosition = require('./command/tmpPosition')
const tmpTraffic = require('./command/tmpTraffic/tmpTraffic')
const tmpServer = require('./command/tmpServer')
const tmpVersion = require('./command/tmpVersion')
const tmpDlcMap = require('./command/tmpDlcMap')
const tmpMileageRanking = require('./command/tmpMileageRanking')

function normalizeConfig(config = {}) {
  const map = {
    query_show_avatar_enable: 'queryShowAvatarEnable',
    baidu_translate_enable: 'baiduTranslateEnable',
    baidu_translate_app_id: 'baiduTranslateAppId',
    baidu_translate_key: 'baiduTranslateKey',
    baidu_translate_cache_enable: 'baiduTranslateCacheEnable',
    api_timeout_seconds: 'apiTimeoutSeconds',
    prefer_vtcm_mileage: 'preferVtcmMileage',
    enable_bind_feature: 'enableBindFeature',
    dlc_list_image: 'dlcListImage',
    tmp_query_type: 'tmpQueryType',
    tmp_traffic_type: 'tmpTrafficType'
  }
  const out = {}
  for (const k in config) {
    const nk = map[k] || k
    out[nk] = config[k]
  }
  if (out.tmpQueryType == null) out.tmpQueryType = 1
  if (out.tmpTrafficType == null) out.tmpTrafficType = 1
  return out
}

const Config = Schema.object({
  queryShowAvatarEnable: Schema.boolean().default(true),
  baiduTranslateEnable: Schema.boolean().default(true),
  baiduTranslateAppId: Schema.string().default(''),
  baiduTranslateKey: Schema.string().default(''),
  baiduTranslateCacheEnable: Schema.boolean().default(false),
  apiTimeoutSeconds: Schema.number().default(10),
  preferVtcmMileage: Schema.boolean().default(true),
  enableBindFeature: Schema.boolean().default(true),
  dlcListImage: Schema.boolean().default(false),
  tmpQueryType: Schema.number().default(1),
  tmpTrafficType: Schema.number().default(1)
})

const apply = (ctx, rawConfig) => {
  const cfg = normalizeConfig(rawConfig || {})
  guildModel(ctx)

  if (cfg.enableBindFeature !== false) {
    ctx.command('绑定 <tmpId:number>', '绑定 TMP ID').action(async ({ session }, tmpId) => {
      return await tmpBind(ctx, cfg, session, tmpId)
    })

    ctx.command('解绑', '解除 TMP ID 绑定').action(async ({ session }) => {
      await ctx.database.remove('tmp_guild_bind', {
        platform: session.platform,
        user_id: session.userId
      })
      return '解绑成功'
    })
  }

  ctx.command('查询 [tmpId:number]', '查询 TMP 玩家信息').action(async ({ session }, tmpId) => {
    return await tmpQuery(ctx, cfg, session, tmpId)
  })

  ctx.command('定位 [tmpId:number]', '查询玩家位置信息').action(async ({ session }, tmpId) => {
    return await tmpPosition(ctx, cfg, session, tmpId)
  })

  ctx.command('路况 <serverName:string>', '查询热门路况 s1|s2|p|a').action(async (_, serverName) => {
    return await tmpTraffic(ctx, cfg, serverName)
  })

  ctx.command('服务器', '查询服务器信息列表').action(async () => {
    return await tmpServer(ctx)
  })

  ctx.command('插件版本', '查询插件/接口版本信息').action(async () => {
    return await tmpVersion(ctx)
  })

  ctx.command('DLC列表', '地图相关 DLC 列表').action(async ({ session }) => {
    return await tmpDlcMap(ctx, session)
  })

  ctx.command('地图DLC', '地图相关 DLC 列表').action(async ({ session }) => {
    return await tmpDlcMap(ctx, session)
  })

  ctx.command('总里程排行', '总里程排行榜').action(async ({ session }) => {
    return await tmpMileageRanking(ctx, session, 'total')
  })

  ctx.command('今日里程排行', '今日里程排行榜').action(async ({ session }) => {
    return await tmpMileageRanking(ctx, session, 'today')
  })
}

module.exports = {
  name: 'napcat-tmp-bot',
  Config,
  apply
}

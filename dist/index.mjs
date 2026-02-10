import fs from 'fs';
import path from 'path';

var plugin_config_ui = [
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

var plugin_init = async (ctx) => {
  ctx.logger.log('【TMP查询机器人】插件加载中...');
  ctx.logger.log('dataPath:', ctx.dataPath);
  ctx.logger.log('configPath:', ctx.configPath);
  
  try {
    fs.mkdirSync(ctx.dataPath, { recursive: true });
    ctx.logger.log('数据目录创建成功');
  } catch (err) {
    ctx.logger.error('数据目录创建失败:', err);
  }
  
  ctx.logger.log('【TMP查询机器人】插件加载完成');
};

var plugin_onmessage = async (ctx, event) => {
  if (event.post_type !== 'message') return;

  const message = event.raw_message || event.message;
  if (message === '/test') {
    await ctx.actions.call('send_msg', {
      message: '插件加载成功!',
      message_type: event.message_type,
      ...(event.message_type === 'group' && event.group_id
        ? { group_id: String(event.group_id) }
        : {}),
      ...(event.message_type === 'private' && event.user_id
        ? { user_id: String(event.user_id) }
        : {}),
    }, ctx.adapterName, ctx.pluginManager.config);
  }
};

var plugin_cleanup = (ctx) => {
  ctx.logger.log('【TMP查询机器人】插件已卸载');
};

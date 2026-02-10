module.exports = {
  async getTranslate(ctx, contentMd5) {
    const cacheData = await ctx.database.getCache();
    return cacheData[contentMd5]?.translate_content || null;
  },

  save(ctx, contentMd5, content, translateContent) {
    ctx.database.setCache({ [contentMd5]: { content, content_md5: contentMd5, translate_content: translateContent } });
  }
}

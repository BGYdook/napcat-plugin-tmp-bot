module.exports = {
  async getTranslate(db, contentMd5) {
    const cacheData = await db.getCache();
    return cacheData[contentMd5]?.translate_content || null;
  },

  save(db, contentMd5, content, translateContent) {
    db.setCache({ [contentMd5]: { content, content_md5: contentMd5, translate_content: translateContent } });
  }
}

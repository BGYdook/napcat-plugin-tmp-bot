module.exports = {
  async getTranslate(ctx, contentMd5) {
    try {
      const fs = require('fs/promises');
      const path = require('path');
      const cacheFilePath = path.join(ctx.dataPath, 'translate_cache.json');
      const data = await fs.readFile(cacheFilePath, 'utf8');
      const cacheData = JSON.parse(data);
      return cacheData[contentMd5]?.translate_content || null;
    } catch (err) {
      return null;
    }
  },

  save(ctx, contentMd5, content, translateContent) {
    const fs = require('fs/promises');
    const path = require('path');
    const cacheFilePath = path.join(ctx.dataPath, 'translate_cache.json');

    fs.readFile(cacheFilePath, 'utf8')
      .then(data => {
        const cacheData = JSON.parse(data);
        cacheData[contentMd5] = { content, content_md5: contentMd5, translate_content: translateContent };
        return fs.writeFile(cacheFilePath, JSON.stringify(cacheData, null, 2));
      })
      .catch(() => {});
  }
}

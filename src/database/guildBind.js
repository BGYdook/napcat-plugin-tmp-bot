module.exports = {
  async get(ctx, platform, userId) {
    try {
      const fs = require('fs/promises');
      const path = require('path');
      const bindFilePath = path.join(ctx.dataPath, 'bind.json');
      const data = await fs.readFile(bindFilePath, 'utf8');
      const bindData = JSON.parse(data);
      const key = `${platform}:${userId}`;
      return bindData[key];
    } catch (err) {
      return null;
    }
  },

  saveOrUpdate(ctx, platform, userId, tmpId) {
    const fs = require('fs/promises');
    const path = require('path');
    const bindFilePath = path.join(ctx.dataPath, 'bind.json');

    fs.readFile(bindFilePath, 'utf8')
      .then(data => {
        const bindData = JSON.parse(data);
        const key = `${platform}:${userId}`;
        bindData[key] = tmpId;
        return fs.writeFile(bindFilePath, JSON.stringify(bindData, null, 2));
      })
      .catch(() => {});
  }
}

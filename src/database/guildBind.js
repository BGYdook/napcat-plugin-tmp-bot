module.exports = {
  async get(db, platform, userId) {
    return await db.get(platform, userId);
  },

  saveOrUpdate(db, platform, userId, tmpId) {
    db.set(platform, userId, tmpId);
  }
}

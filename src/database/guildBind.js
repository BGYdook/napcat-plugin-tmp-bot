module.exports = {
  async get(ctx, platform, userId) {
    return await ctx.database.get(platform, userId);
  },

  saveOrUpdate(ctx, platform, userId, tmpId) {
    ctx.database.set(platform, userId, tmpId);
  }
}

const fetch = require('node-fetch');
const BASE_API = 'https://da.vtcm.link';

module.exports = {
  async serverList(http) {
    try {
      const result = await fetch(`${BASE_API}/server/list`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: data.code !== 200,
        data: data.data
      };
    } catch {
      return { error: true };
    }
  },

  async mapPlayerList(http, serverId, ax, ay, bx, by) {
    try {
      const result = await fetch(`${BASE_API}/map/playerList?aAxisX=${ax}&aAxisY=${ay}&bAxisX=${bx}&bAxisY=${by}&serverId=${serverId}`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: data.code !== 200,
        data: data.data
      };
    } catch {
      return { error: true };
    }
  },

  async playerInfo(http, tmpId) {
    try {
      const result = await fetch(`${BASE_API}/player/info?tmpId=${tmpId}`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        code: data.code,
        error: data.code !== 200,
        data: data.data
      };
    } catch {
      return { error: true };
    }
  },

  async dlcList(http, type) {
    try {
      const result = await fetch(`${BASE_API}/dlc/list?type=${type}`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: data.code !== 200,
        data: data.data
      };
    } catch {
      return { error: true };
    }
  },

  async mileageRankingList(http, rankingType, tmpId) {
    try {
      const url = `${BASE_API}/statistics/mileageRankingList?rankingType=${rankingType}&rankingCount=10`;
      const result = await fetch(tmpId ? `${url}&tmpId=${tmpId}` : url, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: data.code !== 200,
        data: data.data
      };
    } catch {
      return { error: true };
    }
  }
}

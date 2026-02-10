const fetch = require('node-fetch');
const BASE_API = 'https://api.codetabs.com/v1/proxy/?quest=https://api.truckyapp.com';

module.exports = {
  async online(http, tmpId) {
    try {
      const result = await fetch(`${BASE_API}/v3/map/online?playerID=${tmpId}`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: !data || data.error,
        data: data.response || data
      };
    } catch {
      return { error: true };
    }
  },

  async trafficTop(http, serverName) {
    try {
      const result = await fetch(`${BASE_API}/v2/traffic/top?game=ets2&server=${serverName}`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: !data || !data.response || data.response.length <= 0,
        data: data.response || data
      };
    } catch {
      return { error: true };
    }
  }
}

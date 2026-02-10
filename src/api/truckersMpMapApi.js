const fetch = require('node-fetch');
const BASE_API = 'https://tracker.ets2map.com';

module.exports = {
  async area(http, serverId, x1, y1, x2, y2) {
    try {
      const result = await fetch(`${BASE_API}/v3/area?x1=${x1}&y1=${y1}&x2=${x2}&y2=${y2}&server=${serverId}`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: !data || !data.Success,
        data: data.Data || []
      };
    } catch {
      return { error: true };
    }
  }
}

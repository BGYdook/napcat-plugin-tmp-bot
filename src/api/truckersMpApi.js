const fetch = require('node-fetch');
const BASE_API = 'https://api.truckersmp.com/v2';

module.exports = {
  async player(http, tmpId) {
    try {
      const result = await fetch(`${BASE_API}/player/${tmpId}`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: data.error || false,
        data: data.response || data
      };
    } catch {
      return { error: true };
    }
  },

  async servers(http) {
    try {
      const result = await fetch(`${BASE_API}/servers`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: data.error || false,
        data: data.response || data
      };
    } catch {
      return { error: true };
    }
  },

  async bans(http, tmpId) {
    try {
      const result = await fetch(`${BASE_API}/bans/${tmpId}`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: data.error || false,
        data: data.response || data
      };
    } catch {
      return { error: true };
    }
  },

  async version(http) {
    try {
      const result = await fetch(`${BASE_API}/version`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: false,
        data: data
      };
    } catch {
      return { error: true };
    }
  },

  async vtcMember(http, vtcId, memberId) {
    try {
      const result = await fetch(`${BASE_API}/vtc/${vtcId}/member/${memberId}`, {
        timeout: 10000
      });
      const data = await result.json();
      return {
        error: data.error || false,
        data: data.response || data
      };
    } catch {
      return { error: true };
    }
  }
}

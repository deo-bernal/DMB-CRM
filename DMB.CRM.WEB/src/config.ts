export const crmApiConfig = {
  crm_api_url: process.env.REACT_APP_CRM_API_URL || "http://localhost:5088/api",
};

export function resolveOAuthApiBaseUrl() {
  if (typeof window !== "undefined" && /(?:^|\.)dmbwebsolutions\.com$/i.test(window.location.hostname)) {
    return `${window.location.origin}/crm/api`;
  }

  return crmApiConfig.crm_api_url;
}

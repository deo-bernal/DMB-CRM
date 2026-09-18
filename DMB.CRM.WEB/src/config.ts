export const crmApiConfig = {
  crm_api_url: process.env.REACT_APP_CRM_API_URL || "http://localhost:5088/api",
};

/** Prefer same-origin /crm/api on the live site to avoid CORS preflight to Render. */
export function resolveApiBaseUrl() {
  if (typeof window !== "undefined" && /(?:^|\.)dmbwebsolutions\.com$/i.test(window.location.hostname)) {
    return `${window.location.origin}/crm/api`;
  }
  return crmApiConfig.crm_api_url;
}

export function resolveOAuthApiBaseUrl() {
  return resolveApiBaseUrl();
}

export function warmupApi() {
  if (typeof window === "undefined") return;
  const health = `${resolveApiBaseUrl().replace(/\/$/, "")}/health`;
  void fetch(health, { method: "GET", cache: "no-store", credentials: "omit" }).catch(() => undefined);
}

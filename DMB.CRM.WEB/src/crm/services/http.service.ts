import axios from "axios";
import { crmApiConfig } from "../../config";

const http = axios.create({
  baseURL: crmApiConfig.crm_api_url,
  headers: { "Content-type": "application/json" },
});

http.interceptors.request.use((config) => {
  const token = localStorage.getItem("crm_token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  const locationId = localStorage.getItem("crm_locationId");
  if (locationId) {
    config.headers["X-Location-Id"] = locationId;
  }
  return config;
});

http.interceptors.response.use(
  (response) => response,
  (error) => {
    const status = error?.response?.status;
    const requestUrl = String(error?.config?.url ?? "");
    const isAuthEndpoint = /\/auth\/(login|logout|external|me)/i.test(requestUrl);
    if (status === 401 && !isAuthEndpoint && localStorage.getItem("crm_token")) {
      localStorage.removeItem("crm_token");
      window.dispatchEvent(new Event("crm:unauthorized"));
    }
    return Promise.reject(error);
  }
);

export default http;

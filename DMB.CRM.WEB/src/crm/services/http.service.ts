import axios from "axios";
import { crmApiConfig } from "../../config";
import { beginLoading, endLoading, tracksPageLoading } from "../utils/loadingGate";

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
  if (tracksPageLoading(config.url)) {
    beginLoading();
  }
  return config;
});

http.interceptors.response.use(
  (response) => {
    if (tracksPageLoading(response.config.url)) {
      endLoading();
    }
    return response;
  },
  (error) => {
    const requestUrl = String(error?.config?.url ?? "");
    if (tracksPageLoading(requestUrl)) {
      endLoading();
    }
    const status = error?.response?.status;
    const isAuthEndpoint = /\/auth\/(login|logout|external|me)/i.test(requestUrl);
    if (status === 401 && !isAuthEndpoint && localStorage.getItem("crm_token")) {
      localStorage.removeItem("crm_token");
      window.dispatchEvent(new Event("crm:unauthorized"));
    }
    return Promise.reject(error);
  }
);

export default http;

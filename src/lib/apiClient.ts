import axios from "axios";
import { getToken } from "./auth";

declare module "axios" {
  export interface AxiosRequestConfig {
    skipAuthRedirect?: boolean;
  }
}

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5000/api",
  withCredentials: true,
  timeout: 10000,
});

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error),
);

let interceptorRegistered = false;

export function registerApiInterceptors() {
  if (interceptorRegistered) return;

  apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
      const status = error?.response?.status as number | undefined;
      const url: string = error?.config?.url ?? "";
      // Auth calls (login/signup) must never redirect — the form handles the error inline.
      if (url.includes("/auth/")) return Promise.reject(error);
      // Callers can opt out of the global redirect for background fetches by setting
      // `config.skipAuthRedirect = true` (used for batch state hydration where a 403
      // on an admin-only endpoint must not boot the current user).
      const skipRedirect = Boolean(error?.config?.skipAuthRedirect);

      if (status === 401 && getToken() && !skipRedirect) {
        window.location.assign("/error/401");
      } else if (status === 403 && !skipRedirect) {
        window.location.assign("/error/403");
      } else if (status && status >= 500 && !skipRedirect) {
        window.location.assign("/error/500");
      }
      return Promise.reject(error);
    },
  );

  interceptorRegistered = true;
}

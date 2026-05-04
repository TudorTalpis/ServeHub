import React, { createContext, useReducer, useEffect, useState, useContext, type ReactNode } from "react";
import type { AppState, AppAction, Role } from "@/types";
import { appReducer } from "./appReducer";
import type { Currency, ExchangeRates } from "@/lib/currency";
import { DEFAULT_RATES } from "@/lib/currency";
import { getSession } from "@/lib/auth";
import { fetchAppStateFromAPI } from "@/lib/apiSync";

function getSessionState() {
  const s = getSession();
  return s
    ? { userId: s.userId, role: s.role as Role }
    : { userId: null as null, role: "GUEST" as Role };
}

function getEmptyState(): AppState {
  return {
    session: getSessionState(),
    users: [],
    providerProfiles: [],
    categories: [],
    services: [],
    availability: [],
    timeoff: [],
    bookings: [],
    applications: [],
    notifications: [],
    reviews: [],
  };
}

function getCurrentUser(state: AppState) {
  if (!state.session.userId) return null;
  return state.users.find((u) => u.id === state.session.userId) ?? null;
}

function getProvider(state: AppState) {
  if (!state.session.userId) return null;
  return state.providerProfiles.find((p) => p.userId === state.session.userId) ?? null;
}

export interface AppContextValue {
  state: AppState;
  dispatch: React.Dispatch<AppAction>;
  currentUser: ReturnType<typeof getCurrentUser>;
  currentProvider: ReturnType<typeof getProvider>;
  hasRole: (roles: Role[]) => boolean;
  resetData: () => void;
  currency: Currency;
  setCurrency: (c: Currency) => void;
  exchangeRates: ExchangeRates;
}

export const AppContext = createContext<AppContextValue | null>(null);

export function AppProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(appReducer, undefined, getEmptyState);
  const [currency, setCurrency] = useState<Currency>("MDL");
  const exchangeRates = DEFAULT_RATES;

  useEffect(() => {
    fetchAppStateFromAPI()
      .then((data) => {
        dispatch({
          type: "SET_STATE",
          payload: { ...getEmptyState(), ...data } as AppState,
        });
      })
      .catch(() => {
        // API unavailable — keep empty state, app degrades gracefully
      });
  }, []);

  const currentUser = getCurrentUser(state);
  const currentProvider = getProvider(state);
  const hasRole = (roles: Role[]) => roles.includes(currentUser?.role ?? state.session.role);

  const resetData = () => {
    fetchAppStateFromAPI()
      .then((data) => {
        dispatch({
          type: "SET_STATE",
          payload: { ...getEmptyState(), ...data } as AppState,
        });
      })
      .catch(() => {});
  };

  return (
    <AppContext.Provider
      value={{
        state,
        dispatch,
        currentUser,
        currentProvider,
        hasRole,
        resetData,
        currency,
        setCurrency,
        exchangeRates,
      }}
    >
      {children}
    </AppContext.Provider>
  );
}

export function useAppStore() {
  const ctx = useContext(AppContext);
  if (!ctx) throw new Error("useAppStore must be used inside AppProvider");
  return ctx;
}

export function useCurrentUser() {
  const ctx = useContext(AppContext);
  if (!ctx) throw new Error("useCurrentUser must be used inside AppProvider");
  return ctx.currentUser;
}

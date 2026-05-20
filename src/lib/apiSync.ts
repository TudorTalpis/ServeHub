/**
 * API Integration Helper
 *
 * This module provides functions to sync the AppContext state with the backend API.
 * It allows gradually migrating from localStorage to API-based data fetching.
 */

import type {
  AppState,
  AppAction,
  Category,
  ProviderProfile,
  Service,
  Availability,
  TimeOff,
  Booking,
  ProviderApplication,
  Review,
  AppNotification,
  AppUser,
} from "@/types";
import { apiClient } from "@/lib/apiClient";
import { getSession } from "@/lib/auth";
import type {
  CategoryDto,
  ProviderProfileDto,
  ServiceDto,
  AvailabilityDto,
  TimeOffDto,
  BookingDto,
  ProviderApplicationDto,
  ReviewDto,
  NotificationDto,
  UserDto,
} from "@/api/types";

// Silent variants of the GET-all calls: same endpoints as `@/api`, but with
// `skipAuthRedirect` so a 403 on an admin-only endpoint won't navigate the
// current user to /error/403. Used only for background state hydration.
const silent = { skipAuthRedirect: true } as const;
const silentGet =
  <T>(path: string) =>
  () =>
    apiClient.get<T>(path, silent).then((r) => r.data);

const silentCategories     = silentGet<CategoryDto[]>("/categories");
const silentProviders      = silentGet<ProviderProfileDto[]>("/providers");
const silentServices       = silentGet<ServiceDto[]>("/services");
const silentAvailabilities = silentGet<AvailabilityDto[]>("/availability");
const silentTimeOffs       = silentGet<TimeOffDto[]>("/timeoff");
const silentBookings       = silentGet<BookingDto[]>("/bookings");
const silentApplications   = silentGet<ProviderApplicationDto[]>("/applications");
const silentReviews        = silentGet<ReviewDto[]>("/reviews");
const silentNotifications  = silentGet<NotificationDto[]>("/notifications");
const silentUsers          = silentGet<UserDto[]>("/users");

/**
 * Fetches all data from the backend API and returns an AppState object.
 * Use this to initialize the app state from the API instead of localStorage.
 * Admin-only endpoints (/users, /applications) are only requested when the
 * current session has the ADMIN role to avoid expected 403s.
 */
export async function fetchAppStateFromAPI(): Promise<Partial<AppState>> {
  const catchEmpty = (label: string) => (err: unknown) => {
    console.warn(`[API] Failed to load ${label}:`, err);
    return [];
  };

  const isAdmin = getSession()?.role === "ADMIN";

  const [
    categories,
    providers,
    services,
    availability,
    timeoff,
    bookings,
    applications,
    reviews,
    notifications,
    users,
  ] = await Promise.all([
    silentCategories().catch(catchEmpty("categories")),
    silentProviders().catch(catchEmpty("providers")),
    silentServices().catch(catchEmpty("services")),
    silentAvailabilities().catch(catchEmpty("availability")),
    silentTimeOffs().catch(catchEmpty("timeoff")),
    silentBookings().catch(catchEmpty("bookings")),
    isAdmin ? silentApplications().catch(catchEmpty("applications")) : Promise.resolve([]),
    silentReviews().catch(catchEmpty("reviews")),
    silentNotifications().catch(catchEmpty("notifications")),
    isAdmin ? silentUsers().catch(catchEmpty("users")) : Promise.resolve([]),
  ]);

  return {
    categories: categories as Category[],
    providerProfiles: providers as ProviderProfile[],
    services: services as Service[],
    availability: availability as Availability[],
    timeoff: timeoff as TimeOff[],
    bookings: bookings as Booking[],
    applications: applications as ProviderApplication[],
    reviews: reviews as Review[],
    notifications: notifications as AppNotification[],
    users: users as AppUser[],
  };
}

/**
 * Creates a data sync function that can be dispatched to update state.
 * Returns an action that can be dispatched to set the state from API data.
 */
export function createSetStateFromAPIAction(data: Partial<AppState>): AppAction {
  return {
    type: "SET_STATE",
    payload: {
      session: { userId: null, role: "GUEST" },
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
      ...data,
    } as AppState,
  };
}

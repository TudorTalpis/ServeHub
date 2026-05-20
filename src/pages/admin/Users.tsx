import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { useAppStore } from "@/store/AppContext";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { Search, Edit2, Users as UsersIcon, Mail, Phone, Shield, ChevronLeft, ChevronRight } from "lucide-react";
import { AdminPanelLayout } from "@/components/AdminPanelLayout";
import { cn } from "@/lib/utils";
import type { Role } from "@/types";

const PANEL_CLASS = "rounded-2xl border border-border/60 bg-card p-6 shadow-card";
const PAGE_SIZE = 10;

const ROLE_COLORS: Record<Role, string> = {
  ADMIN: "bg-purple-600 text-white border-transparent",
  PROVIDER: "bg-blue-600 text-white border-transparent",
  USER: "bg-green-600 text-white border-transparent",
  GUEST: "bg-gray-500 text-white border-transparent",
};

const AdminUsers = () => {
  const { state } = useAppStore();
  const [searchQuery, setSearchQuery] = useState("");
  const [roleFilter, setRoleFilter] = useState<Role | "ALL">("ALL");
  const [page, setPage] = useState(1);

  const filteredUsers = useMemo(() => {
    const query = searchQuery.toLowerCase();
    return state.users.filter((user) => {
      const matchesSearch =
        user.name.toLowerCase().includes(query) ||
        user.email.toLowerCase().includes(query) ||
        user.phone.includes(searchQuery);
      const matchesRole = roleFilter === "ALL" || user.role === roleFilter;
      return matchesSearch && matchesRole;
    });
  }, [state.users, searchQuery, roleFilter]);

  const userStats = useMemo(
    () => ({
      total: state.users.length,
      admins: state.users.filter((u) => u.role === "ADMIN").length,
      providers: state.users.filter((u) => u.role === "PROVIDER").length,
      users: state.users.filter((u) => u.role === "USER").length,
    }),
    [state.users],
  );

  const totalPages = Math.max(1, Math.ceil(filteredUsers.length / PAGE_SIZE));
  const currentPage = Math.min(page, totalPages);
  const pagedUsers = useMemo(
    () => filteredUsers.slice((currentPage - 1) * PAGE_SIZE, currentPage * PAGE_SIZE),
    [filteredUsers, currentPage],
  );

  return (
    <AdminPanelLayout>
      <div className="animate-fade-in space-y-6">
        <section className="space-y-2">
          <h2 className="font-display text-2xl font-bold">Users</h2>
          <p className="text-sm text-muted-foreground">Manage platform users and their details</p>
        </section>

        {/* Stats */}
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <div className={cn(PANEL_CLASS, "p-4")}>
            <div className="flex items-center justify-between">
              <div>
                <p className="text-xs text-muted-foreground">Total Users</p>
                <p className="text-2xl font-bold">{userStats.total}</p>
              </div>
              <UsersIcon className="h-8 w-8 text-muted-foreground/50" />
            </div>
          </div>
          <div className={cn(PANEL_CLASS, "p-4")}>
            <div className="flex items-center justify-between">
              <div>
                <p className="text-xs text-muted-foreground">Admins</p>
                <p className="text-2xl font-bold text-purple-500">{userStats.admins}</p>
              </div>
              <Shield className="h-8 w-8 text-purple-500/50" />
            </div>
          </div>
          <div className={cn(PANEL_CLASS, "p-4")}>
            <div className="flex items-center justify-between">
              <div>
                <p className="text-xs text-muted-foreground">Providers</p>
                <p className="text-2xl font-bold text-blue-500">{userStats.providers}</p>
              </div>
              <UsersIcon className="h-8 w-8 text-blue-500/50" />
            </div>
          </div>
          <div className={cn(PANEL_CLASS, "p-4")}>
            <div className="flex items-center justify-between">
              <div>
                <p className="text-xs text-muted-foreground">Regular Users</p>
                <p className="text-2xl font-bold text-green-500">{userStats.users}</p>
              </div>
              <UsersIcon className="h-8 w-8 text-green-500/50" />
            </div>
          </div>
        </div>

        {/* Filters */}
        <div className="flex flex-col gap-3 sm:flex-row">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              placeholder="Search by name, email, or phone..."
              value={searchQuery}
              onChange={(e) => {
                setSearchQuery(e.target.value);
                setPage(1);
              }}
              className="pl-10 rounded-xl"
            />
          </div>
          <select
            value={roleFilter}
            onChange={(e) => {
              setRoleFilter(e.target.value as Role | "ALL");
              setPage(1);
            }}
            className="rounded-xl border border-border bg-background px-4 py-2 text-sm"
          >
            <option value="ALL">All Roles</option>
            <option value="ADMIN">Admin</option>
            <option value="PROVIDER">Provider</option>
            <option value="USER">User</option>
          </select>
        </div>

        {/* Users List */}
        <section className={PANEL_CLASS}>
          <h3 className="mb-4 text-sm font-semibold uppercase tracking-wider text-muted-foreground">
            Users ({filteredUsers.length})
          </h3>

          {filteredUsers.length === 0 ? (
            <div className="rounded-2xl border border-dashed bg-secondary/30 p-8 text-center">
              <p className="text-sm text-muted-foreground">No users found.</p>
            </div>
          ) : (
            <div className="space-y-2">
              {pagedUsers.map((user) => (
                <div
                  key={user.id}
                  className="rounded-2xl border border-border/60 bg-background/40 p-4"
                >
                  <div className="flex items-center justify-between gap-4">
                    <div className="flex items-center gap-3 flex-1 min-w-0">
                      <div className="flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-xl bg-primary text-sm font-bold text-white">
                        {user.name[0]}
                      </div>
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2">
                          <h4 className="text-sm font-semibold truncate">{user.name}</h4>
                          <Badge className={cn("text-[10px] capitalize", ROLE_COLORS[user.role])}>
                            {user.role}
                          </Badge>
                        </div>
                        <div className="mt-1 flex flex-wrap gap-x-4 gap-y-1 text-xs text-muted-foreground">
                          <span className="flex items-center gap-1">
                            <Mail className="h-3 w-3" /> {user.email}
                          </span>
                          <span className="flex items-center gap-1">
                            <Phone className="h-3 w-3" /> {user.phone}
                          </span>
                        </div>
                      </div>
                    </div>
                    <Link to={`/admin/users/${user.id}`}>
                      <Button size="sm" variant="outline" className="h-8 gap-1 rounded-full px-3 text-xs flex-shrink-0">
                        <Edit2 className="h-3 w-3" /> Edit
                      </Button>
                    </Link>
                  </div>
                </div>
              ))}
            </div>
          )}

          {totalPages > 1 && (
            <div className="mt-6 flex items-center justify-between gap-2">
              <p className="text-xs text-muted-foreground">
                Page {currentPage} of {totalPages}
              </p>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  className="h-8 rounded-lg"
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  disabled={currentPage === 1}
                >
                  <ChevronLeft className="h-4 w-4" /> Prev
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  className="h-8 rounded-lg"
                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                  disabled={currentPage === totalPages}
                >
                  Next <ChevronRight className="h-4 w-4" />
                </Button>
              </div>
            </div>
          )}
        </section>
      </div>
    </AdminPanelLayout>
  );
};

export default AdminUsers;

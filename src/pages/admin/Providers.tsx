import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAppStore } from "@/store/AppContext";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { Search, Star, Ban, Award, Megaphone, Eye, ChevronLeft, ChevronRight } from "lucide-react";
import { AdminPanelLayout } from "@/components/AdminPanelLayout";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { cn } from "@/lib/utils";
import { getCategoryNames } from "@/lib/categories";

const PANEL_CLASS = "rounded-2xl border border-border/60 bg-card p-6 shadow-card";
const PAGE_SIZE = 10;

type ProviderFilter = "ALL" | "ACTIVE" | "BLOCKED" | "FEATURED" | "SPONSORED";

const AdminProviders = () => {
  const { state, dispatch } = useAppStore();
  const navigate = useNavigate();
  const providers = state.providerProfiles;
  const [searchQuery, setSearchQuery] = useState("");
  const [filter, setFilter] = useState<ProviderFilter>("ALL");
  const [page, setPage] = useState(1);

  const activeProviders = useMemo(() => providers.filter((p) => !p.blocked).length, [providers]);
  const blockedProviders = useMemo(() => providers.filter((p) => p.blocked).length, [providers]);

  const filteredProviders = useMemo(() => {
    const query = searchQuery.trim().toLowerCase();
    return providers.filter((provider) => {
      const categoryNames = getCategoryNames(state.categories, provider.categoryIds).join(" ").toLowerCase();
      const matchesSearch =
        query === "" ||
        provider.name.toLowerCase().includes(query) ||
        provider.location.toLowerCase().includes(query) ||
        categoryNames.includes(query);
      const matchesFilter =
        filter === "ALL" ||
        (filter === "ACTIVE" && !provider.blocked) ||
        (filter === "BLOCKED" && provider.blocked) ||
        (filter === "FEATURED" && provider.featured) ||
        (filter === "SPONSORED" && provider.sponsored);
      return matchesSearch && matchesFilter;
    });
  }, [providers, searchQuery, filter, state.categories]);

  const totalPages = Math.max(1, Math.ceil(filteredProviders.length / PAGE_SIZE));
  const currentPage = Math.min(page, totalPages);
  const pagedProviders = useMemo(
    () => filteredProviders.slice((currentPage - 1) * PAGE_SIZE, currentPage * PAGE_SIZE),
    [filteredProviders, currentPage],
  );

  return (
    <AdminPanelLayout>
      <div className="animate-fade-in space-y-6">
        <section className="space-y-2">
          <h2 className="font-display text-2xl font-bold">Providers</h2>
          <p className="text-sm text-muted-foreground">Manage featured placement, sponsored visibility, and account access.</p>
        </section>

        <div className="flex flex-col gap-3 sm:flex-row">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              placeholder="Search providers by name, location, or category..."
              value={searchQuery}
              onChange={(e) => {
                setSearchQuery(e.target.value);
                setPage(1);
              }}
              className="pl-10 rounded-xl"
            />
          </div>
          <select
            value={filter}
            onChange={(e) => {
              setFilter(e.target.value as ProviderFilter);
              setPage(1);
            }}
            className="rounded-xl border border-border bg-background px-4 py-2 text-sm"
          >
            <option value="ALL">All Providers</option>
            <option value="ACTIVE">Active</option>
            <option value="BLOCKED">Blocked</option>
            <option value="FEATURED">Featured</option>
            <option value="SPONSORED">Sponsored</option>
          </select>
        </div>

        <section className={PANEL_CLASS}>
          <div className="mb-4 flex flex-wrap items-center justify-between gap-2">
            <h3 className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">
              {filteredProviders.length} of {providers.length} Provider{providers.length !== 1 ? "s" : ""}
            </h3>
            <div className="flex items-center gap-2 text-[11px]">
              <Badge variant="outline" className="rounded-full border-success/30 bg-success/10 text-success">{activeProviders} active</Badge>
              <Badge variant="outline" className="rounded-full border-destructive/30 bg-destructive/10 text-destructive">{blockedProviders} blocked</Badge>
            </div>
          </div>

          {filteredProviders.length === 0 ? (
            <div className="rounded-2xl border border-dashed bg-secondary/30 p-8 text-center">
              <p className="text-sm text-muted-foreground">
                {providers.length === 0 ? "No providers yet." : "No providers match your filters."}
              </p>
            </div>
          ) : (
            <div className="space-y-3">
              {pagedProviders.map((provider) => {
                const categoryNames = getCategoryNames(state.categories, provider.categoryIds);
                return (
                  <div
                    key={provider.id}
                    className={cn(
                      "rounded-2xl border border-border/60 bg-background/40 p-5 transition-all duration-200 hover:shadow-elevated",
                      provider.blocked && "opacity-60",
                    )}
                  >
                    <div className="flex min-w-0 flex-wrap items-center justify-between gap-3">
                      <div className="min-w-0 flex-1">
                        <div className="flex flex-wrap items-center gap-2">
                          <h4 className="text-sm font-semibold">{provider.name}</h4>
                          {(provider.pendingCategoryNames?.length ?? 0) > 0 && (
                            <Badge variant="outline" className="rounded-full border-warning/40 px-2 text-[10px] text-warning">
                              Pending categories: {provider.pendingCategoryNames.length}
                            </Badge>
                          )}
                          {provider.featured && (
                            <Badge className="rounded-full border-0 bg-primary px-2 text-[10px] text-primary-foreground">Featured</Badge>
                          )}
                          {provider.sponsored && (
                            <Badge className="rounded-full border-0 bg-accent px-2 text-[10px] text-accent-foreground">Sponsored</Badge>
                          )}
                          {provider.blocked && (
                            <Badge className="rounded-full border-0 bg-destructive px-2 text-[10px] text-destructive-foreground">Blocked</Badge>
                          )}
                        </div>
                        <div className="mt-1.5 flex flex-wrap items-center gap-3 text-xs text-muted-foreground">
                          {categoryNames.length > 0 && <span>{categoryNames.join(", ")}</span>}
                          <span className="flex items-center gap-0.5"><Star className="h-3 w-3 fill-warning text-warning" /> {provider.rating}</span>
                          <span>{provider.location}</span>
                        </div>
                      </div>

                      <div className="flex gap-1">
                        <Tooltip>
                          <TooltipTrigger asChild>
                            <Button variant="ghost" size="sm" onClick={() => navigate(`/admin/providers/${provider.id}`)} className="h-8 w-8 rounded-lg p-0 text-muted-foreground hover:text-primary">
                              <Eye className="h-4 w-4" />
                            </Button>
                          </TooltipTrigger>
                          <TooltipContent>Manage Provider</TooltipContent>
                        </Tooltip>

                        <Tooltip>
                          <TooltipTrigger asChild>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => dispatch({ type: "TOGGLE_FEATURED", payload: provider.id })}
                              className={cn(
                                "h-8 w-8 rounded-lg p-0",
                                provider.featured ? "bg-primary/10 text-primary" : "text-muted-foreground",
                              )}
                            >
                              <Award className="h-4 w-4" />
                            </Button>
                          </TooltipTrigger>
                          <TooltipContent>Toggle Featured</TooltipContent>
                        </Tooltip>

                        <Tooltip>
                          <TooltipTrigger asChild>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => dispatch({ type: "TOGGLE_SPONSORED", payload: provider.id })}
                              className={cn(
                                "h-8 w-8 rounded-lg p-0",
                                provider.sponsored ? "bg-accent/10 text-accent" : "text-muted-foreground",
                              )}
                            >
                              <Megaphone className="h-4 w-4" />
                            </Button>
                          </TooltipTrigger>
                          <TooltipContent>Toggle Sponsored</TooltipContent>
                        </Tooltip>

                        <Tooltip>
                          <TooltipTrigger asChild>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => dispatch({ type: "TOGGLE_BLOCKED", payload: provider.id })}
                              className={cn(
                                "h-8 w-8 rounded-lg p-0",
                                provider.blocked ? "bg-destructive/10 text-destructive" : "text-muted-foreground",
                              )}
                            >
                              <Ban className="h-4 w-4" />
                            </Button>
                          </TooltipTrigger>
                          <TooltipContent>Toggle Block</TooltipContent>
                        </Tooltip>
                      </div>
                    </div>
                  </div>
                );
              })}
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

export default AdminProviders;

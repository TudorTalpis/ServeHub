import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { useAppStore } from "@/store/AppContext";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Info, Search } from "lucide-react";
import { Input } from "@/components/ui/input";
import { AdminPanelLayout } from "@/components/AdminPanelLayout";
import { cn } from "@/lib/utils";
import { getCategoryNames } from "@/lib/categories";

const PANEL_CLASS = "rounded-2xl border border-border/60 bg-card p-6 shadow-card";

type StatusFilter = "ALL" | "PENDING" | "APPROVED" | "REJECTED";

const AdminApplications = () => {
  const { state } = useAppStore();
  const [searchQuery, setSearchQuery] = useState("");
  const [statusFilter, setStatusFilter] = useState<StatusFilter>("ALL");

  const filteredApplications = useMemo(() => {
    const query = searchQuery.trim().toLowerCase();
    return state.applications.filter((application) => {
      const matchesSearch =
        query === "" ||
        application.name.toLowerCase().includes(query) ||
        application.location.toLowerCase().includes(query) ||
        application.phone.includes(searchQuery);
      const matchesStatus = statusFilter === "ALL" || application.status === statusFilter;
      return matchesSearch && matchesStatus;
    });
  }, [state.applications, searchQuery, statusFilter]);

  const pending = useMemo(
    () => filteredApplications.filter((a) => a.status === "PENDING"),
    [filteredApplications],
  );
  const resolved = useMemo(
    () => filteredApplications.filter((a) => a.status !== "PENDING"),
    [filteredApplications],
  );

  const getCategories = (ids: string[]) => getCategoryNames(state.categories, ids).join(", ") || "Unknown category";

  return (
    <AdminPanelLayout>
      <div className="animate-fade-in space-y-6">
        <section className="space-y-2">
          <h2 className="font-display text-2xl font-bold">Applications</h2>
          <p className="text-sm text-muted-foreground">Review pending requests and track resolved provider applications.</p>
        </section>

        <div className="flex flex-col gap-3 sm:flex-row">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              placeholder="Search by name, location, or phone..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="pl-10 rounded-xl"
            />
          </div>
          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value as StatusFilter)}
            className="rounded-xl border border-border bg-background px-4 py-2 text-sm"
          >
            <option value="ALL">All Statuses</option>
            <option value="PENDING">Pending</option>
            <option value="APPROVED">Approved</option>
            <option value="REJECTED">Rejected</option>
          </select>
        </div>

        <section className={PANEL_CLASS}>
          <div className="mb-4 flex items-center justify-between gap-2">
            <h3 className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Pending ({pending.length})</h3>
            <Badge className="rounded-full border-0 bg-warning text-warning-foreground text-[10px]">
              Needs review
            </Badge>
          </div>

          {pending.length === 0 ? (
            <div className="rounded-2xl border border-dashed bg-secondary/30 p-8 text-center">
              <p className="text-sm text-muted-foreground">No pending applications.</p>
            </div>
          ) : (
            <div className="space-y-3">
              {pending.map((application) => (
                <div key={application.id} className="rounded-2xl border border-border/60 bg-background/40 p-5">
                  <h4 className="text-sm font-semibold">{application.name}</h4>
                  <p className="mt-1 text-xs leading-relaxed text-muted-foreground">{application.description}</p>
                  <div className="mt-2 text-[11px] text-muted-foreground">
                    {getCategories(application.categoryIds)} - {application.location} - {application.phone}
                  </div>
                  <div className="mt-3 flex gap-2">
                    <Link to={`/admin/applications/${application.id}`}>
                      <Button size="sm" variant="outline" className="h-8 gap-1 rounded-full px-4 text-xs">
                        <Info className="h-3 w-3" /> Info
                      </Button>
                    </Link>
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>

        {resolved.length > 0 && (
          <section className={PANEL_CLASS}>
            <h3 className="mb-4 text-sm font-semibold uppercase tracking-wider text-muted-foreground">Resolved ({resolved.length})</h3>
            <div className="space-y-2">
              {resolved.map((application) => (
                <div key={application.id} className="rounded-2xl border border-border/60 bg-background/40 px-4 py-3">
                  <div className="flex items-center justify-between gap-2">
                    <span className="text-sm font-medium">{application.name}</span>
                    <Badge
                      className={cn(
                        "rounded-full px-2 text-[10px]",
                        application.status === "APPROVED"
                          ? "border-0 bg-success text-success-foreground"
                          : "border-0 bg-destructive text-destructive-foreground",
                      )}
                    >
                      {application.status}
                    </Badge>
                  </div>
                  {application.status === "REJECTED" && application.rejectReason && (
                    <p className="mt-2 text-xs text-destructive">Reason: {application.rejectReason}</p>
                  )}
                </div>
              ))}
            </div>
          </section>
        )}
      </div>
    </AdminPanelLayout>
  );
};

export default AdminApplications;

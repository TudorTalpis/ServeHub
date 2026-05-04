import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAppStore } from "@/store/AppContext";
import { useI18n } from "@/store/useI18n";
import { LANGUAGE_OPTIONS } from "@/store/language-options";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { toast } from "@/hooks/use-toast";
import { usersApi } from "@/api";
import { Globe, Save, Lock, Briefcase } from "lucide-react";

const SettingsPage = () => {
  const navigate = useNavigate();
  const { state, dispatch, currentUser } = useAppStore();
  const { t, lang, setLang } = useI18n();

  const [name, setName] = useState(currentUser?.name ?? "");
  const [email, setEmail] = useState(currentUser?.email ?? "");
  const [phone, setPhone] = useState(currentUser?.phone ?? "");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");

  if (!currentUser) {
    return (
      <div className="mx-auto max-w-6xl px-4 py-20 text-center">
        <p className="text-muted-foreground text-sm">Please sign in to access settings.</p>
      </div>
    );
  }

  const handleSave = async () => {
    if (!name.trim() || !email.trim()) {
      toast({ title: "Please complete name and email." });
      return;
    }

    const updated = await usersApi.update(currentUser.id, { name: name.trim(), email: email.trim() });
    const updatedUsers = state.users.map((u) =>
      u.id === currentUser.id ? { ...u, ...updated } : u,
    );
    dispatch({ type: "SET_STATE", payload: { ...state, users: updatedUsers } });

    toast({ title: t("settings.saved") });
  };

  const handleChangePassword = async () => {
    if (!currentPassword || !newPassword) {
      toast({ title: "Please fill in both password fields." });
      return;
    }
    if (newPassword.length < 6) {
      toast({ title: "New password must be at least 6 characters." });
      return;
    }
    try {
      await usersApi.changePassword(currentUser.id, { currentPassword, newPassword });
      setCurrentPassword("");
      setNewPassword("");
      toast({ title: "Password updated successfully." });
    } catch {
      toast({ title: "Current password is incorrect.", variant: "destructive" });
    }
  };

  return (
    <div className="mx-auto max-w-2xl px-4 py-8 animate-fade-in">
      <h1 className="font-display text-2xl font-bold mb-8">{t("settings.title")}</h1>

      {/* Language section */}
      <div className="rounded-2xl border bg-card p-6 shadow-card mb-6">
        <h2 className="font-semibold text-sm flex items-center gap-2 mb-4">
          <Globe className="h-4 w-4 text-primary" /> {t("settings.language")}
        </h2>
        <div className="flex gap-2">
          {LANGUAGE_OPTIONS.map((opt) => (
            <button
              key={opt.code}
              onClick={() => setLang(opt.code)}
              className={`rounded-xl border px-4 py-2.5 text-sm font-medium transition-all duration-200 ${
                lang === opt.code
                  ? "border-primary bg-primary/10 text-primary ring-1 ring-primary/20"
                  : "bg-secondary/30 text-foreground hover:border-primary/40 hover:bg-primary/5"
              }`}
            >
              {opt.label}
            </button>
          ))}
        </div>
      </div>

      {/* Profile section */}
      <div className="rounded-2xl border bg-card p-6 shadow-card mb-6">
        <h2 className="font-semibold text-sm mb-4">{t("settings.profile")}</h2>
        <div className="space-y-4">
          <div>
            <Label className="text-xs text-muted-foreground mb-1.5 block">{t("settings.name")}</Label>
            <Input value={name} onChange={(e) => setName(e.target.value)} className="max-w-md" />
          </div>
          <div>
            <Label className="text-xs text-muted-foreground mb-1.5 block">{t("settings.email")}</Label>
            <Input type="email" value={email} onChange={(e) => setEmail(e.target.value)} className="max-w-md" />
          </div>
          <div>
            <Label className="text-xs text-muted-foreground mb-1.5 block">{t("settings.phone")}</Label>
            <Input
              type="tel"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              placeholder="+373 ..."
              disabled
              className="max-w-md opacity-60"
            />
          </div>
          <Button onClick={handleSave} className="gap-1.5 rounded-xl">
            <Save className="h-3.5 w-3.5" /> {t("settings.save")}
          </Button>
        </div>
      </div>

      {/* Change Password section */}
      <div className="rounded-2xl border bg-card p-6 shadow-card mb-6">
        <h2 className="font-semibold text-sm flex items-center gap-2 mb-4">
          <Lock className="h-4 w-4 text-primary" /> {t("settings.password")}
        </h2>
        <div className="space-y-4">
          <div>
            <Label className="text-xs text-muted-foreground mb-1.5 block">Current password</Label>
            <Input
              type="password"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              placeholder="••••••••"
              className="max-w-md"
            />
          </div>
          <div>
            <Label className="text-xs text-muted-foreground mb-1.5 block">New password</Label>
            <Input
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              placeholder="••••••••"
              className="max-w-md"
            />
          </div>
          <Button onClick={handleChangePassword} className="gap-1.5 rounded-xl">
            <Lock className="h-3.5 w-3.5" /> Update password
          </Button>
        </div>
      </div>

      {/* Become Provider section */}
      <div className="rounded-2xl border bg-card p-6">
        <h2 className="font-semibold text-sm flex items-center gap-2 mb-4">
          <Briefcase className="h-4 w-4 text-primary" /> {t("settings.becomeProvider")}
        </h2>
        <p className="text-xs text-muted-foreground mb-4">{t("home.becomeProvider")}</p>
        <Button
          onClick={() => navigate("/become-provider")}
          className="gap-1.5 rounded-xl bg-primary hover:bg-primary/90"
        >
          <Briefcase className="h-3.5 w-3.5" /> {t("settings.becomeProvider")}
        </Button>
      </div>
    </div>
  );
};

export default SettingsPage;

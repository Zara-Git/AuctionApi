import { useState } from "react";
import { api } from "../api";
import { useNavigate } from "react-router-dom";
import "./ChangePasswordPage.css";

export default function ChangePasswordPage() {
  const navigate = useNavigate();

  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (newPassword.length < 6) {
      setError("New password must be at least 6 characters.");
      return;
    }

    setLoading(true);

    try {
      await api("/api/auth/password", {
        method: "PUT",
        body: JSON.stringify({
          currentPassword,
          newPassword,
        }),
      });

      setSuccess("Password changed successfully.");
      setCurrentPassword("");
      setNewPassword("");
    } catch (err) {
      setError(err.message || "Could not change password.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="change-password-page">
      <div className="change-password-card">
        <button
          className="back-btn"
          type="button"
          onClick={() => navigate("/auctions")}
        >
          ← Back to auctions
        </button>

        <h1>Change Password</h1>
        <p className="subtitle">
          Update your password securely. Your username cannot be changed.
        </p>

        <form className="change-password-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Current password</label>
            <input
              type="password"
              placeholder="Enter your current password"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label>New password</label>
            <input
              type="password"
              placeholder="Enter your new password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              required
            />
          </div>

          {error && <div className="message error-message">{error}</div>}
          {success && <div className="message success-message">{success}</div>}

          <button className="save-btn" type="submit" disabled={loading}>
            {loading ? "Saving..." : "Save new password"}
          </button>
        </form>
      </div>
    </div>
  );
}

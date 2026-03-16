import { useState } from "react";
import { api } from "../api";
import { useNavigate } from "react-router-dom";
import "./AdminPage.css";

export default function AdminPage() {
  const navigate = useNavigate();

  const [auctionId, setAuctionId] = useState("");
  const [userId, setUserId] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const clearMessages = () => {
    setError("");
    setSuccess("");
  };

  const handleDisableAuction = async () => {
    clearMessages();
    try {
      await api(`/api/admin/auctions/${auctionId}/disable`, {
        method: "PUT",
      });
      setSuccess("Auction disabled successfully.");
      setAuctionId("");
    } catch (e) {
      setError(e.message);
    }
  };

  const handleEnableAuction = async () => {
    clearMessages();
    try {
      await api(`/api/admin/auctions/${auctionId}/enable`, {
        method: "PUT",
      });
      setSuccess("Auction enabled successfully.");
      setAuctionId("");
    } catch (e) {
      setError(e.message);
    }
  };

  const handleDisableUser = async () => {
    clearMessages();
    try {
      await api(`/api/admin/users/${userId}/disable`, {
        method: "PUT",
      });
      setSuccess("User disabled successfully.");
      setUserId("");
    } catch (e) {
      setError(e.message);
    }
  };

  const handleEnableUser = async () => {
    clearMessages();
    try {
      await api(`/api/admin/users/${userId}/enable`, {
        method: "PUT",
      });
      setSuccess("User enabled successfully.");
      setUserId("");
    } catch (e) {
      setError(e.message);
    }
  };

  return (
    <div className="admin-page">
      <div className="admin-card">
        <button className="back-btn" onClick={() => navigate("/auctions")}>
          ← Back to auctions
        </button>

        <h1>Admin Panel</h1>
        <p className="admin-subtitle">Manage auctions and user accounts.</p>

        {error && <div className="message error-message">{error}</div>}
        {success && <div className="message success-message">{success}</div>}

        <div className="admin-section">
          <h2>Auction Management</h2>
          <input
            type="number"
            placeholder="Enter auction id"
            value={auctionId}
            onChange={(e) => setAuctionId(e.target.value)}
          />
          <div className="admin-actions">
            <button onClick={handleDisableAuction}>Disable Auction</button>
            <button onClick={handleEnableAuction}>Enable Auction</button>
          </div>
        </div>

        <div className="admin-section">
          <h2>User Management</h2>
          <input
            type="number"
            placeholder="Enter user id"
            value={userId}
            onChange={(e) => setUserId(e.target.value)}
          />
          <div className="admin-actions">
            <button onClick={handleDisableUser}>Disable User</button>
            <button onClick={handleEnableUser}>Enable User</button>
          </div>
        </div>
      </div>
    </div>
  );
}

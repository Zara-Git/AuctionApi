import { useEffect, useState } from "react";
import { api } from "../api";
import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";
import "./AuctionsPage.css";

export default function AuctionsPage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const [searchTitle, setSearchTitle] = useState("");
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState("");
  const [success, setSuccess] = useState("");
  const [showCreateForm, setShowCreateForm] = useState(false);

  const [form, setForm] = useState({
    title: "",
    description: "",
    startingPrice: "",
    startDate: "",
    endDate: "",
  });

  const loadAuctions = async () => {
    setLoading(true);
    setErr("");

    try {
      const data = await api(
        `/api/auctions/search?title=${encodeURIComponent(searchTitle)}`,
      );
      setItems(data);
    } catch (e) {
      setErr(e.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAuctions();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleSearch = async () => {
    setSuccess("");
    await loadAuctions();
  };

  const handleChange = (e) => {
    setForm((prev) => ({
      ...prev,
      [e.target.name]: e.target.value,
    }));
  };

  const resetForm = () => {
    setForm({
      title: "",
      description: "",
      startingPrice: "",
      startDate: "",
      endDate: "",
    });
  };

  const handleCreateAuction = async (e) => {
    e.preventDefault();
    setErr("");
    setSuccess("");

    try {
      await api("/api/auctions", {
        method: "POST",
        body: JSON.stringify({
          title: form.title,
          description: form.description,
          startingPrice: Number(form.startingPrice),
          startDate: form.startDate,
          endDate: form.endDate,
        }),
      });

      setSuccess("Auction created successfully.");
      resetForm();
      setShowCreateForm(false);
      await loadAuctions();
    } catch (e) {
      setErr(e.message);
    }
  };

  const handleLogout = () => {
    logout();
    navigate("/");
  };

  return (
    <div className="auctions-page">
      <div className="auctions-wrapper">
        <div className="auctions-card">
          <div className="auctions-header">
            <div>
              <h2>Auctions</h2>
              <p>
                Search open auctions and create a new auction if you are logged
                in.
              </p>
            </div>

            <div className="header-actions">
              {user && (
                <>
                  <button
                    className="primary-btn create-btn"
                    onClick={() => {
                      setShowCreateForm((prev) => !prev);
                      setErr("");
                      setSuccess("");
                    }}
                  >
                    {showCreateForm ? "Close form" : "Create Auction"}
                  </button>

                  <button className="secondary-btn" onClick={handleLogout}>
                    Logout
                  </button>
                  <button
                    className="secondary-btn"
                    onClick={() => navigate("/change-password")}
                  >
                    Change Password
                  </button>
                  <button
                    className="secondary-btn"
                    onClick={() => navigate("/admin")}
                  >
                    Admin Panel
                  </button>
                </>
              )}
            </div>
          </div>

          <div className="search-row">
            <input
              className="auction-input"
              placeholder="Search by title..."
              value={searchTitle}
              onChange={(e) => setSearchTitle(e.target.value)}
            />

            <button className="dark-btn" onClick={handleSearch}>
              Search
            </button>
          </div>

          {!user && (
            <div className="info-message">
              You can view auctions, but you need to log in to create an auction
              or place bids.
            </div>
          )}

          {err && <div className="error-message">{err}</div>}
          {success && <div className="success-message">{success}</div>}

          {showCreateForm && user && (
            <form
              className="create-auction-form"
              onSubmit={handleCreateAuction}
            >
              <h3>Create New Auction</h3>

              <input
                className="auction-input"
                name="title"
                placeholder="Auction title"
                value={form.title}
                onChange={handleChange}
                required
              />

              <textarea
                className="auction-input auction-textarea"
                name="description"
                placeholder="Description"
                value={form.description}
                onChange={handleChange}
                required
                rows={4}
              />

              <input
                className="auction-input"
                name="startingPrice"
                type="number"
                placeholder="Starting price"
                value={form.startingPrice}
                onChange={handleChange}
                required
                min="1"
              />

              <div className="date-grid">
                <div>
                  <label>Start date</label>
                  <input
                    className="auction-input"
                    name="startDate"
                    type="datetime-local"
                    value={form.startDate}
                    onChange={handleChange}
                    required
                  />
                </div>

                <div>
                  <label>End date</label>
                  <input
                    className="auction-input"
                    name="endDate"
                    type="datetime-local"
                    value={form.endDate}
                    onChange={handleChange}
                    required
                  />
                </div>
              </div>

              <div className="form-actions">
                <button type="submit" className="success-btn">
                  Save Auction
                </button>

                <button
                  type="button"
                  className="secondary-btn"
                  onClick={() => {
                    resetForm();
                    setShowCreateForm(false);
                  }}
                >
                  Cancel
                </button>
              </div>
            </form>
          )}

          {loading ? (
            <p className="loading-text">Loading auctions...</p>
          ) : items.length === 0 ? (
            <div className="empty-state">No auctions found.</div>
          ) : (
            <div className="auctions-grid">
              {items.map((a) => {
                const isOpen = new Date(a.endDate) > new Date();

                return (
                  <div key={a.id} className="auction-item-card">
                    <div className="auction-item-top">
                      <div className="auction-title">{a.title}</div>

                      <span
                        className={
                          isOpen ? "status-badge open" : "status-badge closed"
                        }
                      >
                        {isOpen ? "Open" : "Closed"}
                      </span>
                    </div>

                    <div className="seller-name">Seller: {a.sellerName}</div>

                    {a.description && (
                      <div className="auction-description">{a.description}</div>
                    )}

                    <div className="auction-meta">
                      <div>
                        <span>Starting price:</span> {a.startingPrice}
                      </div>
                      <div>
                        <span>Start:</span>{" "}
                        {a.startDate
                          ? new Date(a.startDate).toLocaleString()
                          : "-"}
                      </div>
                      <div>
                        <span>Ends:</span>{" "}
                        {new Date(a.endDate).toLocaleString()}
                      </div>
                    </div>

                    <button
                      className="dark-btn full-width-btn"
                      onClick={() => navigate(`/auctions/${a.id}`)}
                    >
                      View details
                    </button>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

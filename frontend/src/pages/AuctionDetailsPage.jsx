import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../api";
import { useAuth } from "../context/AuthContext";
import "./AuctionDetailsPage.css";

export default function AuctionDetailsPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();

  const [auction, setAuction] = useState(null);
  const [bids, setBids] = useState([]);
  const [amount, setAmount] = useState("");
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState("");
  const [success, setSuccess] = useState("");

  const [isEditing, setIsEditing] = useState(false);
  const [editForm, setEditForm] = useState({
    title: "",
    description: "",
    startingPrice: "",
    startDate: "",
    endDate: "",
  });

  const loadData = async () => {
    setLoading(true);
    setErr("");

    try {
      const auctionData = await api(`/api/auctions/${id}`);
      setAuction(auctionData);

      setEditForm({
        title: auctionData.title || "",
        description: auctionData.description || "",
        startingPrice: auctionData.startingPrice || "",
        startDate: auctionData.startDate
          ? auctionData.startDate.slice(0, 16)
          : "",
        endDate: auctionData.endDate ? auctionData.endDate.slice(0, 16) : "",
      });

      const auctionIsOpen = new Date(auctionData.endDate) > new Date();

      if (auctionIsOpen) {
        const bidsData = await api(`/api/auctions/${id}/bids`);
        setBids(bidsData);
      } else {
        setBids([]);
      }
    } catch (e) {
      setErr(e.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [id]);

  const highestBid =
    bids.length > 0
      ? Math.max(...bids.map((b) => b.amount))
      : (auction?.startingPrice ?? 0);

  const isOpen = auction ? new Date(auction.endDate) > new Date() : false;

  const isOwner =
    user &&
    auction &&
    (user.id === auction.userId ||
      user.id === auction.sellerId ||
      user.name === auction.sellerName);

  const handlePlaceBid = async (e) => {
    e.preventDefault();
    setErr("");
    setSuccess("");

    const numericAmount = Number(amount);

    if (numericAmount <= highestBid) {
      setErr(`Your bid must be higher than ${highestBid}.`);
      return;
    }

    try {
      await api("/api/bids", {
        method: "POST",
        body: JSON.stringify({
          auctionId: Number(id),
          amount: numericAmount,
        }),
      });

      setSuccess("Bid placed successfully.");
      setAmount("");
      await loadData();
    } catch (e) {
      setErr(e.message);
    }
  };

  const handleEditChange = (e) => {
    setEditForm((prev) => ({
      ...prev,
      [e.target.name]: e.target.value,
    }));
  };

  const handleUpdateAuction = async (e) => {
    e.preventDefault();
    setErr("");
    setSuccess("");

    try {
      await api(`/api/auctions/${id}`, {
        method: "PUT",
        body: JSON.stringify({
          title: editForm.title,
          description: editForm.description,
          startingPrice: Number(editForm.startingPrice),
          startDate: editForm.startDate,
          endDate: editForm.endDate,
        }),
      });

      setSuccess("Auction updated successfully.");
      setIsEditing(false);
      await loadData();
    } catch (e) {
      setErr(e.message);
    }
  };

  if (loading) {
    return <div className="auction-details-page">Loading...</div>;
  }

  if (!auction) {
    return <div className="auction-details-page">Auction not found.</div>;
  }

  return (
    <div className="auction-details-page">
      <div className="auction-details-wrapper">
        <button className="back-btn" onClick={() => navigate("/auctions")}>
          ← Back to auctions
        </button>

        {err && <div className="error-message">{err}</div>}
        {success && <div className="success-message">{success}</div>}

        <div className="auction-details-card">
          <div className="auction-details-header">
            <div>
              <h1>{auction.title}</h1>
              <p>{auction.description}</p>
            </div>

            <span
              className={isOpen ? "status-badge open" : "status-badge closed"}
            >
              {isOpen ? "Open" : "Closed"}
            </span>
          </div>

          <div className="auction-info-grid">
            <div>
              <strong>Seller:</strong> {auction.sellerName}
            </div>
            <div>
              <strong>Starting price:</strong> {auction.startingPrice}
            </div>
            <div>
              <strong>Start:</strong>{" "}
              {auction.startDate
                ? new Date(auction.startDate).toLocaleString()
                : "-"}
            </div>
            <div>
              <strong>Ends:</strong>{" "}
              {new Date(auction.endDate).toLocaleString()}
            </div>
            <div>
              <strong>Highest bid:</strong> {highestBid}
            </div>
          </div>

          {user && isOwner && isOpen && (
            <div style={{ marginTop: "16px" }}>
              <button
                className="dark-btn"
                type="button"
                onClick={() => setIsEditing((prev) => !prev)}
              >
                {isEditing ? "Cancel Edit" : "Edit Auction"}
              </button>
            </div>
          )}
        </div>

        {user && isOwner && isOpen && isEditing && (
          <form className="place-bid-form" onSubmit={handleUpdateAuction}>
            <h2>Edit Auction</h2>

            <input
              className="bid-input"
              name="title"
              value={editForm.title}
              onChange={handleEditChange}
              placeholder="Title"
              required
            />

            <textarea
              className="bid-input"
              name="description"
              value={editForm.description}
              onChange={handleEditChange}
              placeholder="Description"
              rows={4}
              required
            />

            <input
              className="bid-input"
              type="number"
              name="startingPrice"
              value={editForm.startingPrice}
              onChange={handleEditChange}
              placeholder="Starting price"
              required
              disabled={bids.length > 0}
            />

            <input
              className="bid-input"
              type="datetime-local"
              name="startDate"
              value={editForm.startDate}
              onChange={handleEditChange}
              required
            />

            <input
              className="bid-input"
              type="datetime-local"
              name="endDate"
              value={editForm.endDate}
              onChange={handleEditChange}
              required
            />

            {bids.length > 0 && (
              <div className="info-message">
                Starting price cannot be changed because this auction already
                has bids.
              </div>
            )}

            <button className="dark-btn" type="submit">
              Save Changes
            </button>
          </form>
        )}

        {isOpen ? (
          <div className="auction-bids-card">
            <h2>Bid history</h2>

            {bids.length === 0 ? (
              <p className="muted-text">No bids yet.</p>
            ) : (
              <div className="bid-list">
                {bids.map((bid) => (
                  <div key={bid.id} className="bid-item">
                    <div>
                      <strong>Amount:</strong> {bid.amount}
                    </div>
                    <div>
                      <strong>Bidder:</strong> {bid.bidderName}
                    </div>
                    <div>
                      <strong>Date:</strong>{" "}
                      {new Date(bid.bidDate || bid.createdAt).toLocaleString()}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        ) : (
          <div className="auction-bids-card">
            <h2>Auction result</h2>
            <p className="muted-text">
              This auction is closed. Only the winning bid is shown.
            </p>
            <div className="bid-item">
              <div>
                <strong>Winning bid:</strong> {highestBid}
              </div>
            </div>
          </div>
        )}

        {user && isOpen && !isOwner && (
          <form className="place-bid-form" onSubmit={handlePlaceBid}>
            <h2>Place a bid</h2>
            <p className="muted-text">
              Your bid must be higher than {highestBid}.
            </p>

            <input
              className="bid-input"
              type="number"
              min={highestBid + 1}
              value={amount}
              onChange={(e) => setAmount(e.target.value)}
              placeholder="Enter your bid amount"
              required
            />

            <button className="dark-btn" type="submit">
              Place Bid
            </button>
          </form>
        )}

        {user && isOwner && isOpen && !isEditing && (
          <div className="info-message">
            You cannot place a bid on your own auction.
          </div>
        )}

        {!isOpen && (
          <div className="info-message">
            This auction is closed. You cannot place a bid.
          </div>
        )}
      </div>
    </div>
  );
}

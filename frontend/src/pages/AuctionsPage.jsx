import { useEffect, useState } from "react";
import { api } from "../api";

export default function AuctionsPage() {
    const [title, setTitle] = useState("");
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(false);
    const [err, setErr] = useState("");

    const search = async () => {
        setLoading(true);
        setErr("");
        try {
            const data = await api(`/api/auctions/search?title=${encodeURIComponent(title)}`);
            setItems(data);
        } catch (e) {
            setErr(e.message);
        } finally {
            setLoading(false);
        }
    };

    // load once on page open
    useEffect(() => {
        search();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    return (
        <div style={{ padding: 20, border: "1px solid #ddd", borderRadius: 10 }}>
            <h2>Auctions</h2>

            <div style={{ display: "flex", gap: 10, marginBottom: 12 }}>
                <input
                    placeholder="Search by title..."
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    style={{ flex: 1, padding: 12, borderRadius: 10, border: "1px solid #ddd" }}
                />
                <button
                    onClick={search}
                    style={{
                        padding: "12px 16px",
                        borderRadius: 10,
                        border: "none",
                        background: "#111",
                        color: "white",
                        fontWeight: 700,
                        cursor: "pointer",
                    }}
                >
                    Search
                </button>
            </div>

            {loading && <p>Loading...</p>}
            {err && <p style={{ color: "red" }}>{err}</p>}

            {items.length === 0 && !loading ? (
                <p style={{ color: "#666" }}>No auctions found.</p>
            ) : (
                <div style={{ display: "grid", gap: 10 }}>
                    {items.map((a) => (
                        <div
                            key={a.id}
                            style={{
                                border: "1px solid #eee",
                                borderRadius: 12,
                                padding: 12,
                                background: "#fff",
                            }}
                        >
                            <div style={{ fontWeight: 800, fontSize: 16 }}>{a.title}</div>
                            <div style={{ color: "#666", marginTop: 4 }}>Seller: {a.sellerName}</div>
                            <div style={{ marginTop: 6 }}>
                                <span style={{ fontWeight: 700 }}>Starting:</span> {a.startingPrice}
                            </div>
                            <div style={{ marginTop: 6 }}>
                                <span style={{ fontWeight: 700 }}>Ends:</span> {new Date(a.endDate).toLocaleString()}
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}
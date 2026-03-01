import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import AuctionsPage from "./pages/AuctionsPage";
import { useAuth } from "./context/AuthContext";

export default function App() {
    const { user, logout } = useAuth();

    return (
        <div
            style={{
                minHeight: "100vh",
                display: "flex",
                justifyContent: "center",
                alignItems: "flex-start",
                padding: 20,
                background: "#f6f7fb",
            }}
        >
            <div style={{ width: "100%", maxWidth: 520 }}>
                {/* Header */}
                <div
                    style={{
                        background: "white",
                        borderRadius: 14,
                        padding: 16,
                        boxShadow: "0 10px 30px rgba(0,0,0,0.08)",
                        border: "1px solid #e9e9ee",
                        marginBottom: 16,
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "space-between",
                    }}
                >
                    <div>
                        <div style={{ fontSize: 14, color: "#666" }}>Status</div>
                        <div style={{ fontSize: 16, fontWeight: 600 }}>
                            {user ? `Logged in as ${user.email}` : "Not logged in"}
                        </div>
                    </div>

                    {user && (
                        <button
                            onClick={logout}
                            style={{
                                padding: "10px 14px",
                                borderRadius: 10,
                                border: "1px solid #ddd",
                                background: "white",
                                cursor: "pointer",
                                fontWeight: 600,
                            }}
                        >
                            Logout
                        </button>
                    )}
                </div>
                <h1 style={{ color: "red" }}>APP UPDATED</h1>

                <div
                    style={{
                        background: "white",
                        borderRadius: 14,
                        padding: 18,
                        boxShadow: "0 10px 30px rgba(0,0,0,0.08)",
                        border: "1px solid #e9e9ee",
                        marginBottom: 16,
                    }}
                >
                    <AuctionsPage />
                </div>

               
                {!user && (
                    <div style={{ display: "grid", gap: 16 }}>
                        <div
                            style={{
                                background: "white",
                                borderRadius: 14,
                                padding: 18,
                                boxShadow: "0 10px 30px rgba(0,0,0,0.08)",
                                border: "1px solid #e9e9ee",
                            }}
                        >
                            <RegisterPage />
                        </div>

                        <div
                            style={{
                                background: "white",
                                borderRadius: 14,
                                padding: 18,
                                boxShadow: "0 10px 30px rgba(0,0,0,0.08)",
                                border: "1px solid #e9e9ee",
                            }}
                        >
                            <LoginPage />
                           
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}
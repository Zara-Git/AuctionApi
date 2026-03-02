import { Routes, Route, Navigate } from "react-router-dom";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import AuctionsPage from "./pages/AuctionsPage";
import { useAuth } from "./context/AuthContext";

export default function App() {
  const { user, logout } = useAuth();

  return (
    <div style={{ minHeight: "100vh", padding: 20, background: "#f6f7fb" }}>
      <div style={{ width: "100%", maxWidth: 520, margin: "0 auto" }}>
        {/* Header */}
        <div style={{ background: "white", borderRadius: 14, padding: 16, marginBottom: 16 }}>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
            <div>
              <div style={{ fontSize: 14, color: "#666" }}>Status</div>
              <div style={{ fontSize: 16, fontWeight: 600 }}>
                {user ? `Logged in as ${user.email}` : "Not logged in"}
              </div>
            </div>
            {user && <button onClick={logout}>Logout</button>}
          </div>
        </div>

        <Routes>
          <Route path="/" element={<AuctionsPage />} />
          <Route path="/login" element={user ? <Navigate to="/" /> : <LoginPage />} />
          <Route path="/register" element={user ? <Navigate to="/" /> : <RegisterPage />} />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </div>
    </div>
  );
}
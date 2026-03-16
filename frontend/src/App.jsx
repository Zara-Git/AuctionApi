import { Routes, Route, Navigate } from "react-router-dom";
import AuthPage from "./pages/AuthPage";
import AuctionsPage from "./pages/AuctionsPage";
import { useAuth } from "./context/AuthContext";
import AuctionDetailsPage from "./pages/AuctionDetailsPage";
import ChangePasswordPage from "./pages/ChangePasswordPage";
import AdminPage from "./pages/AdminPage";

function App() {
  const { user } = useAuth();

  return (
    <Routes>
      <Route path="/" element={<AuthPage />} />
      <Route
        path="/change-password"
        element={user ? <ChangePasswordPage /> : <Navigate to="/" />}
      />

      <Route
        path="/auctions"
        element={user ? <AuctionsPage /> : <Navigate to="/" />}
      />
      <Route
        path="/auctions/:id"
        element={user ? <AuctionDetailsPage /> : <Navigate to="/" />}
      />
      <Route
        path="/admin"
        element={user ? <AdminPage /> : <Navigate to="/" />}
      />

      <Route path="*" element={<Navigate to="/" />} />
    </Routes>
  );
}

export default App;

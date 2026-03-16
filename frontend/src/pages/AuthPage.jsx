import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import "./AuthPage.css";

export default function AuthPage() {
  const { login, register } = useAuth();
  const navigate = useNavigate();

  const [isLogin, setIsLogin] = useState(true);

  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const switchToLogin = () => {
    setIsLogin(true);
    setError("");
  };

  const switchToRegister = () => {
    setIsLogin(false);
    setError("");
    setSuccess("");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    setLoading(true);

    try {
      if (isLogin) {
        await login(email, password);
        navigate("/auctions");
      } else {
        await register(name, email, password);

        setSuccess("Registration successful. Please log in.");
        setIsLogin(true);

        setName("");
        setEmail("");
        setPassword("");
      }
    } catch (err) {
      setError(err.message || "Something went wrong");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-header">
          <h1>{isLogin ? "Welcome Back" : "Create Account"}</h1>
          <p>
            {isLogin
              ? "Log in to manage your auctions and bids."
              : "Create your account to start bidding."}
          </p>
        </div>

        <div className="auth-tabs">
          <button
            type="button"
            className={isLogin ? "tab active" : "tab"}
            onClick={switchToLogin}
          >
            Login
          </button>
          <button
            type="button"
            className={!isLogin ? "tab active" : "tab"}
            onClick={switchToRegister}
          >
            Register
          </button>
        </div>

        <form className="auth-form" onSubmit={handleSubmit}>
          {!isLogin && (
            <div className="form-group">
              <label>Name</label>
              <input
                type="text"
                placeholder="Enter your name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required={!isLogin}
              />
            </div>
          )}

          <div className="form-group">
            <label>Email</label>
            <input
              type="email"
              placeholder="Enter your email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label>Password</label>
            <input
              type="password"
              placeholder="Enter your password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          {error && <div className="message error">{error}</div>}
          {success && <div className="message success">{success}</div>}

          <button className="submit-btn" type="submit" disabled={loading}>
            {loading
              ? isLogin
                ? "Logging in..."
                : "Registering..."
              : isLogin
                ? "Login"
                : "Create Account"}
          </button>
        </form>

        <div className="auth-footer">
          {isLogin ? (
            <p>
              Don’t have an account?{" "}
              <span onClick={switchToRegister}>Register here</span>
            </p>
          ) : (
            <p>
              Already have an account?{" "}
              <span onClick={switchToLogin}>Login here</span>
            </p>
          )}
        </div>
      </div>
    </div>
  );
}

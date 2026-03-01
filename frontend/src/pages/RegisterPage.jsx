import { useState } from "react";
import { useAuth } from "../context/AuthContext";

export default function RegisterPage() {
    const { register } = useAuth();
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [err, setErr] = useState("");

    const onSubmit = async (e) => {
        e.preventDefault();
        setErr("");
        try {
            await register(name, email, password);
        } catch (e) {
            setErr(e.message);
        }
    };

    return (
        <div style={{ padding: 20, border: "1px solid #ddd", borderRadius: 10 }}>
            <h2>Register</h2>

            <form onSubmit={onSubmit}>
                <input
                    placeholder="Name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    style={{ width: "100%", padding: 10, marginBottom: 10 }}
                />
                <input
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    style={{ width: "100%", padding: 10, marginBottom: 10 }}
                />
                <input
                    placeholder="Password"
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    style={{ width: "100%", padding: 10, marginBottom: 10 }}
                />
                <button style={{ width: "100%", padding: 10 }}>Register</button>
            </form>

            {err && <p style={{ color: "red" }}>{err}</p>}
        </div>
    );
}

const BASE_URL = "https://localhost:7041";

export async function api(path, options = {}) {
    const token = JSON.parse(localStorage.getItem("user") || "null")?.token;

    const res = await fetch(`${BASE_URL}${path}`, {
        ...options,
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
            ...(options.headers || {}),
        },
    });

    const text = await res.text();
    let data = null;

    try {
        data = text ? JSON.parse(text) : null;
    } catch {
        data = text;
    }

    if (!res.ok) throw new Error(data?.message || data || "Request failed");
    return data;
}
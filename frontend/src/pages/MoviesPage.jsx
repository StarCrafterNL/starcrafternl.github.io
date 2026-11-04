import { useEffect, useState } from "react";
import MovieCard from "../components/MovieCard.jsx";
export default function MoviesPage() {
    const [movies, setMovies] = useState([]);
    const [status, setStatus] = useState("loading");
    useEffect(() => {
        const c = new AbortController();
        async function load() {
        try {
        const response = await fetch("/movies.json", {
        signal: c.signal
        });
        if (!response.ok) throw new Error("Mislukt");
        const data = await response.json();
        if (c.signal.aborted) return;
        setMovies(data);
        setStatus(data.length ? "success" : "empty");
        } catch {
        if (!c.signal.aborted) setStatus("error");
        }
        }
        load();
        return () => c.abort();
}, []);
if (status === "loading")
    return <p role="status">Films laden…</p>;
if (status === "error")
    return <p role="alert">Ophalen mislukt.</p>;
if (status === "empty")
    return <p>Er zijn nog geen films.</p>;
return (
    <section className="movies" aria-label="Films">
        <h2>Films</h2>
        {movies.map(movie => (
            <MovieCard key={movie.id} movie={movie} />
        ))}
    </section>
    );
}

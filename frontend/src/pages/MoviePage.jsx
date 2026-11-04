import { useEffect, useState } from "react";
import { Link, useParams } from "react-router";
export default function MoviePage() {
const { movieId } = useParams();
const [movie, setMovie] = useState(null);
const [status, setStatus] = useState("loading");
useEffect(() => {
const c = new AbortController();
async function load() {
setStatus("loading");
try {
const response = await fetch("/movies.json", {
signal: c.signal
});
if (!response.ok) throw new Error("Mislukt");
const data = await response.json();
if (c.signal.aborted) return;
const gevonden = data.find(
film => film.id === Number(movieId)
);
setMovie(gevonden ?? null);
setStatus(gevonden ? "success" : "notfound");
} catch {
if (!c.signal.aborted) setStatus("error");
}
}
load();
return () => c.abort();
}, [movieId]);
if (status === "loading")
return <p role="status">Film laden…</p>;
if (status === "error")
return <p role="alert">Ophalen mislukt.</p>;
if (status === "notfound")
return <p>Film niet gevonden.</p>;
return (
<article>
<h2>{movie.title}</h2>
<p>Jaar: {movie.year}</p>
<Link to="/movies">Terug naar films</Link>
</article>
);
}
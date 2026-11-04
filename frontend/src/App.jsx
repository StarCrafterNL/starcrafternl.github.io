import { Link, Route, Routes } from "react-router";
import MoviesPage from"./pages/MoviesPage.jsx";
import MoviePage from "./pages/MoviePage.jsx";
export default function App() {
return <>
<header><h1>MovieLab</h1></header>
<nav><Link to="/movies">Films</Link></nav>
<main className="filmpagina"
>
<Routes>
<Route path="/" element={<MoviesPage />} />
<Route path="/movies" element={<MoviesPage />} />
<Route path="/movies/:movieId" element={<MoviePage />} />
<Route path="*" element={<p>Niet gevonden.</p>} />
</Routes>
</main>
<footer>WPFW</footer>
</>;
}
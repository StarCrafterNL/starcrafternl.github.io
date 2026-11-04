import { Link } from "react-router";
export default function MovieCard({ movie }) {
return (
<article className="kaart">
<h2>{movie.title}</h2>
<p>Jaar: {movie.year}</p>
<Link to={`/movies/${movie.id}`}>
Bekijk film
</Link>
</article>
);
}

namespace WEBAPI.Features.Blogposts;

public class UpdateBlogpostDto
{
    public string Titel { get; set; } = string.Empty;
    public string Inhoud { get; set; } = string.Empty;
    public DateTime Publicatiedatum { get; set; }
}
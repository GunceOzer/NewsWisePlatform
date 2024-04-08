namespace NewsAggregationApplication.Data.Entities;

public class Source
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; } // RSS Feed URL

   
    public ICollection<Article> Articles { get; set; }
}
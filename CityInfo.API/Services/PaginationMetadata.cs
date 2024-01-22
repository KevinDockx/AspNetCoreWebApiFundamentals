namespace CityInfo.API.Services;

public class PaginationMetadata(int totalItemCount, int pageSize, int currentPage)
{
    public int TotalItemCount { get; set; } = totalItemCount;
    public int TotalPageCount { get; set; } = (int)Math.Ceiling(totalItemCount / (double)pageSize);
    public int PageSize { get; set; } = pageSize;
    public int CurrentPage { get; set; } = currentPage;
}

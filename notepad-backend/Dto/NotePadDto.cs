namespace notepad_backend.Dto;


public class NotedPadsRequestDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
    public int Sorting { get; set; }
    public string? Filter { get; set; }
}

public class NotedPadsPagedResponceDto
{
    public IEnumerable<NotedPadListResponceDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
}

public class NotedPadListResponceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


public class NotedPadResponceDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public bool IsPinned { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public NotedPadDetailResponceDto Detail { get; set; }
}

public class NotedPadDetailResponceDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
}


public class NotedPadsCreateRequestDto
{
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public bool IsPinned { get; set; }
    public NotedPadsCreateDetailRequestDto Detail { get; set; }
}

public class NotedPadsCreateDetailRequestDto
{
    public string Content { get; set; } = string.Empty;
}

namespace QueryContracts.SampleApi;

public static class ProductContract
{
    public static readonly QueryContract<Product, ProductQuery> Instance =
        QueryContract.For<Product, ProductQuery>()
            .Filter(q => q.Search, p => p.Name).Contains()
            .Filter(q => q.Category, p => p.Category).Equals()
            .Filter(q => q.MinPrice, p => p.Price).GreaterThanOrEqual()
            .Filter(q => q.MaxPrice, p => p.Price).LessThanOrEqual()
            .Filter(q => q.Available, p => p.IsAvailable).Equals()
            .Sort(q => q.Sort)
                .Allow("name", p => p.Name)
                .Allow("price", p => p.Price)
                .Allow("created", p => p.CreatedAt)
                .Default("created", descending: true)
            .Page(q => q.Page, q => q.PageSize, maxSize: 100)
            .Build();
}

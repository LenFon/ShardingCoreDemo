namespace ShardingCore.Domain;

public class Account
{

    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
}

public class Buyer : Account { }
public class Seller : Account { }

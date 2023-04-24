using Len.StronglyTypedId;

namespace ShardingCore.Domain;

[StronglyTypedId]
public partial record struct AccountId(Guid Value);

public class Account
{

    public AccountId Id { get; set; }

    public string Name { get; set; } = default!;
}

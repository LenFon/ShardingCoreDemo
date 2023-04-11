using Len.StronglyTypedId;

namespace ShardingCore.Domain;

public record struct AccountId(Guid Value) : IStronglyTypedId<Guid>
{
    public static IStronglyTypedId<Guid> Create(Guid value) => new AccountId(value);
}

public class Account
{

    public AccountId Id { get; set; }

    public string Name { get; set; } = default!;
}

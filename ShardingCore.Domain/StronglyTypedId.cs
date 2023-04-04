namespace ShardingCore.Domain;

/// <summary>
/// 强类型Id接口
/// </summary>
/// <typeparam name="TStronglyTypedId"></typeparam>
/// <typeparam name="TPrimitiveId"></typeparam>
public interface IStronglyTypedId<TStronglyTypedId, TPrimitiveId>
    where TStronglyTypedId : IStronglyTypedId<TStronglyTypedId, TPrimitiveId>
    where TPrimitiveId : struct, IComparable, IComparable<TPrimitiveId>, IEquatable<TPrimitiveId>, ISpanParsable<TPrimitiveId>

{
    TPrimitiveId Value { get; }

    abstract static TStronglyTypedId Create(TPrimitiveId value);
}

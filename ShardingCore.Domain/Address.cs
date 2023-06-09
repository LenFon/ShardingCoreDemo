﻿namespace ShardingCore.Domain;

public class Address
{
    public Guid Id { get; set; }

    /// <summary>
    /// 省
    /// </summary>
    public string Province { get; set; } = default!;

    /// <summary>
    /// 市
    /// </summary>
    public string City { get; set; } = default!;

    /// <summary>
    /// 区
    /// </summary>
    public string Area { get; set; } = default!;

    /// <summary>
    /// 街道
    /// </summary>
    public string Street { get; set; } = default!;

    /// <summary>
    /// 其他部分
    /// </summary>
    public string Other { get; set; } = default!;
}

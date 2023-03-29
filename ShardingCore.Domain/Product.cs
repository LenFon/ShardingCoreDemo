using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShardingCore.Domain;

public class Product
{
    /// <summary>
    /// 商品Id
    /// </summary>
    public Guid Key { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 单价(元)
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    public string Unit { get; set; } = default!;
}

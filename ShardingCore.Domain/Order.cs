using System;

namespace ShardingCore.Domain;

public class Order
{
    /// <summary>
    /// 买家信息
    /// </summary>
    public Buyer Buyer { get; set; } = default!;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 发货地址信息
    /// </summary>
    public Address? DeliveryAddress { get; set; }

    /// <summary>
    /// 订单Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus OrderStatus { get; set; }

    /// <summary>
    /// 支付状态
    /// </summary>
    public PayStatus PayStatus { get; set; }

    /// <summary>
    /// 购买商品
    /// </summary>
    public HashSet<Product> Products { get; set; } = new HashSet<Product>();

    /// <summary>
    /// 收货地址信息
    /// </summary>
    public Address ReceiverAddress { get; set; } = default!;

    /// <summary>
    /// 卖家信息
    /// </summary>
    public Seller Seller { get; set; } = default!;
}

public enum OrderStatus
{
    /// <summary>
    /// 已创建，待提交
    /// </summary>
    Created = 0,

    /// <summary>
    /// 处理中，商家开始处理订单
    /// </summary>
    Processing = 2,

    /// <summary>
    /// 交易完成
    /// </summary>
    Complated = 3,

    /// <summary>
    /// 交易取消
    /// </summary>
    Cancelled = 4,
}


public enum PayStatus
{
    /// <summary>
    /// 未支付
    /// </summary>
    NoPay = 1,

    /// <summary>
    /// 支付中
    /// </summary>
    Paying = 2,

    /// <summary>
    /// 已支付
    /// </summary>
    Payed = 3,

    /// <summary>
    /// 支付失败
    /// </summary>
    PayFail = 4
}
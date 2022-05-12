using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FakeXieCheng.API.Models
{
    public class Order
    {
        [Key] public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> OrderItems { get; set; }
        public OrderStateEnum State { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string TransactionMetadata { get; set; }
    }

    // 有限状态机-现态
    public enum OrderStateEnum
    {
        Pending, // 订单已生成
        Processing, // 支付处理中
        Completed, // 交易成功
        Declined, // 交易失败
        Cancelled, // 订单取消
        Refund // 已退款
    }

    // 有限状态机-动作
    public enum OrderStateTriggerEnum
    {
        PlaceOrder, // 支付
        Approve, // 支付成功
        Reject, //  支付失败
        Cancel , // 取消
        Return // 退货
    }
}
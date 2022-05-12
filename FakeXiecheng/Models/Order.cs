using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Stateless;

namespace FakeXieCheng.API.Models
{
    public class Order
    {
        public Order()
        {
            StateMachineInit();
        }
        [Key] public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> OrderItems { get; set; }
        public OrderStateEnum State { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public string TransactionMetadata { get; set; }
        private StateMachine<OrderStateEnum, OrderStateTriggerEnum> _machine;

        private void StateMachineInit()
        {
            // 订单初始状态
            _machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>(OrderStateEnum.Pending);
            
            // 订单现态："已生成"
            // 动作1： 点击"支付" ，订单状态更新为 "支付处理中"
            // 动作2： 点击"取消" ，订单状态更新为 "订单取消"
            _machine.Configure(OrderStateEnum.Pending)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Cancel, OrderStateEnum.Cancelled);

            // 订单现态："支付处理中"
            // 动作1： 第三方支付回调成功 ， 订单状态更新为 "交易成功"
            // 动作2： 第三方支付失败 ， 订单状态更新为 "交易失败"
            _machine.Configure(OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Approve, OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Reject, OrderStateEnum.Declined);

            // 订单现态： "交易失败"
            // 动作1： 用户重新发起支付 ， 订单状态更新为 "支付处理中"
            _machine.Configure(OrderStateEnum.Declined)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing);

            // 订单现态： "交易成功"
            // 动作1： 用户发起退款， 订单现态更新为 "已退款"
            _machine.Configure(OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Return, OrderStateEnum.Refund);
        }
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
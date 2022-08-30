using FluentValidation;
using Kindy.DDDTemplate.Application.Commands.Order.Dtos;

namespace Kindy.DDDTemplate.Application.Commands.Order.Validations
{
    /// <summary>
    /// 创建订单模型验证
    /// </summary>
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public CreateOrderCommandValidator()
        {
            RuleFor(command => command.OrderCode).NotEmpty().WithMessage("名称不能为空");
            RuleFor(command => command.OrderAmount).NotEqual(0).WithMessage("数量不能为0");
        }
    }
}

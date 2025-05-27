using MediatR;

namespace Physical.WriteApi.Commands
{
    public class CreateOrderCommand : IRequest<int>
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}

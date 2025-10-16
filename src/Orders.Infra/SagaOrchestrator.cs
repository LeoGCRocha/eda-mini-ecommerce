using System.Text.Json;
using EdaMicroEcommerce.Domain.BuildingBlocks;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orders.Application.IntegrationEvents;
using Orders.Application.IntegrationEvents.Products;
using Orders.Application.Saga;
using Orders.Application.Saga.Entity;
using Orders.Domain.Entities.Events;

namespace Orders.Infra;

public class SagaOrchestrator : ISagaOrchestrator
{
    // TODO: De que maneira seria possivel lidar com um pedido muito tempo como PENDING....
    // todo mensagem precisa vir com um ORDER-ID associado
    // 1. pega o estado atual
        // se for recebido duas mensagens por algum motivo ao mesmo tempo de order-created, como garantir que não seja feita uma 
        // reserva dupla pra um PEDIDO ???
        // esse controle deve ser feito no inventario de que maneira
        // 2. se for order_created
        // pegar a mensagem do tipo order_created_event
            // 3. envia para o outbox e atualiza o estado para pending_reservation
            
    private readonly OrderContext _orderContext;
    private readonly ILogger<SagaOrchestrator> _logger;

    public SagaOrchestrator(OrderContext orderContext, ILogger<SagaOrchestrator> logger)
    {
        _orderContext = orderContext;
        _logger = logger;
    }

    // TODO: Passar usar cancelation tokens
    public async Task ExecuteAsync<T>(OrderId orderId, T @event)
    {
        // 1. Pegar ultimo estado do SAGA
        // Toda order é unica, essa abordagem aqui talvez tenha problemas quando precisarmos lidar com um reprocessamento de eventos
        var sagaState = await _orderContext.Saga.FirstOrDefaultAsync(s => s.OrderId == orderId);

        // TODO: Por isso em outro lugar para respeitar o SRP 
        // TODO: Mudar pra uma service pra trazer mais reaproveitamente.
        if (@event is OrderCreatedEvent orderCreatedEvent)
        {
            // TODO: Se uma reserva já deu problema não precisa mais ficar adiciona logica pras demais....
            if (sagaState is null) // indepotemcia
            {
                // TODO: Cuidar no STATE BASE quantos já foram processados...
                // Quando chegar no numero esperado de reservado finalizar, se não fica pendendo, se tiver algum com falha, finaliza...
                
                // 2. Primeira vez recebida do evento, caminho esperado
                // 2.1. Criar o novo objeto saga para persistir no banco de dados
                var sagaEntity = new SagaEntity(orderId, null, SagaStatus.ORDER_CREATED);
                sagaEntity.StateData = JsonSerializer.Serialize(new StateData()
                {
                    ExpectedReservations = orderCreatedEvent.ProductOrderInfos.Count,
                    FailedReservations = 0,
                    CurrentReservations = 0
                });
                await _orderContext.Saga.AddAsync(sagaEntity);
                
                // 3. Criar os eventos de reserva pra cada produto associado
                foreach (var productReservation in orderCreatedEvent.ProductOrderInfos)
                {
                    var objectEvent = new ProductReservationEvent(orderId, productReservation.ProductId, productReservation.Quantity);
                    var outboxIntegration = new ProductReservationIntegration(EventType.ProductReservation, JsonSerializer.Serialize(objectEvent));
                    // ... O outbox ira consumir isso e enviar pro respectivo topico product-reservation....
                    _orderContext.OutboxIntegrationEvents.Add(outboxIntegration);
                }
                await _orderContext.SaveChangesAsync();
                return;
            }

            _logger.LogWarning("Foi detectado um evento de criação repetido para {OrderId}", orderId.Value.ToString());
            return;
        }

        throw new GenericException("O evento {EventType}, não possui uma implementação no SAGA.");
    }
    
    // TODO: Move this to another CLASS
    private class StateData
    {
        public int ExpectedReservations { get; set; }
        public int CurrentReservations { get; set; }
        public int FailedReservations { get; set; }
    }
}
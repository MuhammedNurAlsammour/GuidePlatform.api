using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.Payments; namespace
GuidePlatform.Application.Features.Queries.Payments.GetPaymentsById
{ public class
GetPaymentsByIdQueryResponse { public int TotalCount { get; set; } public
PaymentsDTO
payments
{ get; set; } = new(); } }
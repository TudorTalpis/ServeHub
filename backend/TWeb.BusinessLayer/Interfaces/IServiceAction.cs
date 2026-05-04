using TWeb.Domain.Models;

namespace TWeb.BusinessLayer.Interfaces;

public interface IServiceAction
{
    List<ServiceDto> GetAllServiceAction();
    List<ServiceDto> GetByProviderIdServiceAction(string providerId);
    ServiceDto? GetByIdServiceAction(string id);
    ServiceDto CreateServiceAction(CreateServiceDto dto);
    ServiceDto? UpdateServiceAction(string id, UpdateServiceDto dto);
    bool DeleteServiceAction(string id);
}


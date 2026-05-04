using TWeb.Domain.Models;

namespace TWeb.BusinessLayer.Interfaces;

public interface ICategoryAction
{
    List<CategoryDto> GetAllCategoryAction();
    CategoryDto? GetByIdCategoryAction(string id);
    CategoryDto CreateCategoryAction(CreateCategoryDto dto);
    CategoryDto? UpdateCategoryAction(string id, UpdateCategoryDto dto);
    bool DeleteCategoryAction(string id);
}

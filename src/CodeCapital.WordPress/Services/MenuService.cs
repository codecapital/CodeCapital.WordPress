using CodeCapital.WordPress.Core.Models;
using CodeCapital.WordPress.Core.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Services
{
    public class MenuService : IMenuService
    {
        private readonly MenuRepository _menuRepository;

        public MenuService(MenuRepository menuRepository) => _menuRepository = menuRepository;

        public virtual async Task<List<MenuItem>> GetMenuAsync(int menuId) => await _menuRepository.GetMenuAsync(menuId);
    }
}

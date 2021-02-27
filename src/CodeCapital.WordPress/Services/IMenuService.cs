using System.Collections.Generic;
using System.Threading.Tasks;
using CodeCapital.WordPress.Core.Models;

namespace CodeCapital.WordPress.Services
{
    public interface IMenuService
    {
        Task<List<MenuItem>> GetMenuAsync(int menuId);
    }
}
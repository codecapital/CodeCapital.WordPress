using CodeCapital.WordPress.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CodeCapital.WordPress.Mvc.ViewModels
{
    public class DebuggerViewModel
    {
        public ConcurrentDictionary<string, Post> Posts { get; set; } = new ConcurrentDictionary<string, Post>();
        public ConcurrentDictionary<int, List<MenuItem>> Menus { get; set; } = new ConcurrentDictionary<int, List<MenuItem>>();
        public List<Term> Terms { get; set; } = new List<Term>();
    }
}

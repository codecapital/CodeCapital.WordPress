﻿using CodeCapital.WordPress.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCapital.WordPress.Core
{
    public class PaginatedList
    {
        // or CurrentPage?
        public int PageIndex { get; }

        public int TotalPages { get; }

        // or TotalItems?
        public int TotalCount { get; }

        // Do we need this?
        //public int PageSize { get; }

        public List<Post> Items { get; set; } = new List<Post>();

        public PaginatedList(List<Post> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            Items = items;
        }

        public PaginatedList()
        {

        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList> CreateAsync(IQueryable<Post> source, int pageIndex, int pageSize)
        {
            var count = await source.AsNoTracking().CountAsync();

            if (pageSize == 0)
            {
                return new PaginatedList(await source.ToListAsync(), count, pageIndex, pageSize);
            }

            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();

            return new PaginatedList(items, count, pageIndex, pageSize);
        }
    }

    //public class PaginatedList<T> : List<T>
    //{
    //    public int PageIndex { get; }

    //    public int TotalPages { get; }

    //    public int TotalCount { get; }

    //    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    //    {
    //        PageIndex = pageIndex;
    //        TotalCount = count;
    //        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

    //        AddRange(items);
    //    }

    //    public bool HasPreviousPage => PageIndex > 1;

    //    public bool HasNextPage => PageIndex < TotalPages;

    //    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    //    {
    //        var count = await source.CountAsync();

    //        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

    //        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    //    }
    //}
}

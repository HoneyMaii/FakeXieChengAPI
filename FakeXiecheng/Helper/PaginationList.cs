using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FakeXieCheng.API.Helper
{
    public class PaginationList<T> : List<T>
    {
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PaginationList(int totalCount, int currentPage, int pageSize, List<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            AddRange(items);
            TotalCount = totalCount;
            TotalPages = (int) Math.Ceiling(totalCount / (double) pageSize);
        }

        // 工厂模式
        public static async Task<PaginationList<T>> CreateAsync(int currentPage, int pageSize, IQueryable<T> result)
        {
            var totalCount = await result.CountAsync(); // 访问数据库获取数据总量
            // pagination
            // skip
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip).Take(pageSize);

            // Eager Load 立即加载 （include vs join ）
            var items = await result.ToListAsync(); // 访问数据库获取数据list
            return new PaginationList<T>(totalCount, currentPage, pageSize, items);
        }
    }
}
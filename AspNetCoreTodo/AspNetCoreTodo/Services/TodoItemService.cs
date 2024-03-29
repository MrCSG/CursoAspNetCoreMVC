using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreTodo.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly ApplicationDbContext _context;
        public TodoItemService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<TodoItem[]> GetIncompleteItemsAsync(ApplicationUser user)
        {
            return await _context.Items.Where(x => x.IsDone == false && x.UserId == user.Id).ToArrayAsync();
        }
        public async Task<Boolean> AddItemAsync(TodoItem newItem, ApplicationUser user)
        {
            newItem.Id = Guid.NewGuid();
            newItem.IsDone = false;
            newItem.UserId = user.Id;

            if(newItem.DueAt == null)
            {
                newItem.DueAt = DateTimeOffset.Now.AddDays(3);
            }

            _context.Items.Add(newItem);
            //SaveChangesAsync() devuelve la cant de items que se cambiaron en la base de datos.
            var saveResult = await _context.SaveChangesAsync();

            return saveResult == 1;
        }
        public async Task<Boolean> MarkDoneAsync(Guid id, ApplicationUser user)
        {
            var item = await _context.Items.Where(x => x.Id == id && x.UserId == user.Id).SingleOrDefaultAsync();
            if(item == null)
            {
                return false;
            }

            item.IsDone = true;
            var saveResult = await _context.SaveChangesAsync();

            return saveResult == 1;
        }
    }
}
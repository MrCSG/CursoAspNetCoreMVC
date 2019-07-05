using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;

namespace AspNetCoreTodo.Controllers
{
    public class TodoController : Controller
    {
        private readonly ITodoItemService _todoItemService;
        public TodoController(ITodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }
        public async Task<IActionResult> Index()
        {
            //Get To-Do items from database
            var items = await _todoItemService.GetIncompleteItemsAsync();

            //Put items into a Model
            var model = new TodoViewModel()
            {
                Items = items
            };

            //Render view using the Model
            return View(model);
        }
    }
}
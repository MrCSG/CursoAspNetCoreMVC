using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;

namespace AspNetCoreTodo.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ITodoItemService _todoItemService;
        private readonly UserManager<IdentityUser> _userManager;
        public TodoController(ITodoItemService todoItemService, UserManager<IdentityUser> userManager)
        {
            _todoItemService = todoItemService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            //Obtener usuario actual, si no existe devuelve un error
            var currentUser = await _userManager.GetUserAsync(User);
            if(currentUser == null)
            {
                return Challenge();
            }
            //Get To-Do items from database
            var items = await _todoItemService.GetIncompleteItemsAsync(currentUser);
            //Put items into a Model
            var model = new TodoViewModel()
            {
                Items = items
            };
            //Render view using the Model
            return View(model);
        }
        //Esta función es la que recibe la petición de agregar un ítem, luego de verificar que no se mezclara call.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(TodoItem newItem)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            //Obtener usuario actual, si no existe devuelve un error
            var currentUser = await _userManager.GetUserAsync(User);
            if(currentUser == null)
            {
                return Challenge();
            }
            //Agrega 1 nuevo item a la base de datos, nos avisa si se pudo o no guardar;
            var successful = await _todoItemService.AddItemAsync(newItem, currentUser);
            if(!successful)
            {
                return BadRequest("Could not add item.");
            }
            return RedirectToAction("Index");
        }
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDone(Guid id)
        {
            if(id == Guid.Empty)
            {
                return RedirectToAction("Index");
            }
            //Obtener usuario actual, si no existe devuelve un error
            var currentUser = await _userManager.GetUserAsync(User);
            if(currentUser == null)
            {
                return Challenge();
            }
            //Marca como verdadero/finalizadas el items que indique el usuario
            var successful = await _todoItemService.MarkDoneAsync(id,currentUser);
            if(!successful)
            {
                return BadRequest("Could not mark item as done.");
            }
            return RedirectToAction("Index");
        }
    }
}
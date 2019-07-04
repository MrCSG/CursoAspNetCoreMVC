using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using AspNetCoreTodo.Models;

namespace AspNetCoreTodo.Controllers
{
    public class TodoController : Controller
    {
        public IActionResult Index()
        {
            //Get To-Do items from database
            TodoViewModel itemList = new TodoViewModel();
            itemList.Items = new TodoItem[3];

            //Put items into a Model
            itemList.Items[0] = new TodoItem();
            itemList.Items[1] = new TodoItem();
            itemList.Items[2] = new TodoItem();

            //Render view using the Model
            return View(itemList);
        }
    }
}
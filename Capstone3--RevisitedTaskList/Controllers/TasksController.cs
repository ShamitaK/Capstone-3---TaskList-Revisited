using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Capstone3__RevisitedTaskList.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone3__RevisitedTaskList.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        //accessing the database
        private readonly IdentityTaskListDbContext _context;

        public TasksController(IdentityTaskListDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier).Value; //link between the databases... primary key id connection
            List<Tasks> thisUsersTasks = _context.Tasks.Where(x => x.OwnerId == id).ToList();
            return View(thisUsersTasks);
        }

        [HttpGet]
        public IActionResult AddTasks()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddTasks(Tasks newTask)
        {
            newTask.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier).Value; //finds the taskownerid that way you can assign the task to the owner id.
            if (ModelState.IsValid)
            {
                _context.Tasks.Add(newTask);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult EditTasks(int id)
        {
            Tasks foundTasks = _context.Tasks.Find(id);
            if (foundTasks != null)
            {
                return View(foundTasks);
            }
            return View();
        }
        [HttpPost]

        public IActionResult EditTasks(Tasks updateTask)
        {
            Tasks dbTasks = _context.Tasks.Find(updateTask.Id);

            if (ModelState.IsValid)
            {
                dbTasks.TaskDescription = updateTask.TaskDescription;
                dbTasks.DueDate = updateTask.DueDate;
                dbTasks.Completed = updateTask.Completed;

                _context.Entry(dbTasks).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.Update(dbTasks);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeleteTasks(int id)
        {
            Tasks dbTasks = _context.Tasks.Find(id);
            if (ModelState.IsValid)
            {
                _context.Tasks.Remove(dbTasks);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult SortTasks()
        {

            return View();
        }
        [HttpPost]
        public IActionResult SortTasks(int id)
        {
            string id2 = User.FindFirst(ClaimTypes.NameIdentifier).Value; 
            List<Tasks> thisUsersTasks = _context.Tasks.Where(x => x.OwnerId == id2).ToList();

            if (id == 1)
            {
                List<Tasks> sortedList = thisUsersTasks.OrderBy(task => task.DueDate).ToList();
                return View("Index", sortedList);
            }
            else if(id == 2)
            {
                List<Tasks> sortedList = thisUsersTasks.OrderBy(task => task.Completed).ToList();
                return View("Index", sortedList);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult FindTasks()
        {
            return View();
        }
        [HttpPost]

        public IActionResult FindTasks(string word)
        {
            string id2 = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Tasks> thisUsersTasks = _context.Tasks.Where(x => x.OwnerId == id2).ToList();

            if (word != null)
            {
                List<Tasks> foundTasks = thisUsersTasks.Where(x => x.TaskDescription.ToLower().Contains(word.ToLower())).ToList();
                return View("Index", foundTasks);
            }
            else 
            {
                return RedirectToAction("Index");
            }
            
        }
    }
}
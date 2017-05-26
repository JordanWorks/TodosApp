using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ToDoApp.Models;
using ToDoApp.Services;

namespace ToDoAPI.Controllers
{
    public class TodosController : ApiController
    {
        private ToDoAppContext db = new ToDoAppContext();

        // GET: api/Todos
        public IQueryable<Todo> GetTodos()
        {
            return db.Todos;
        }

        // GET: api/Todos/GetFinishedTodos
        [Route("api/todos/GetFinishedTodos")]
        public IQueryable<Todo> GetFinishedTodos()
        {
            return db.Todos.Where(t => t.Visibility == false);
        }

        // GET: api/Todos/GetCurrentTodos
        [Route("api/todos/GetCurrentTodos")]
        public IQueryable<Todo> GetCurrentTodos()
        {
            return db.Todos.Where(t => t.Visibility == true);
        }

        // GET: api/Todos/5
        [ResponseType(typeof(Todo))]
        public async Task<IHttpActionResult> GetTodo(int id)
        {
            Todo todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);
        }

        // GET: api/Todos/getCountFinishedTodos
        [Route("api/todos/getCountFinishedTodos")]
        public int GetCountFinishedTodos()
        {
            int finishedTodos = db.Todos.Where(t => t.Visibility == false).Count() > 0 ? db.Todos.Where(t => t.Visibility == false).Count() : 0;
            int allTodos = db.Todos.Count() > 0 ? db.Todos.Count() : 1;

            int res = (finishedTodos * 100) / allTodos;

            if (res > 0)
                return res;
            else
                return 0;
        }

        // PUT: api/Todos/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTodo(int id, JObject jsonData)
        {
            String visibility = TodoServices.parseJson(jsonData, "Visibility");
            String name = TodoServices.parseJson(jsonData, "Name");
            String action = TodoServices.parseJson(jsonData, "Action");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Todo todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            if (id != todo.Id)
            {
                return BadRequest();
            }

            //If we want to hide the todo
            if (visibility != null && visibility.Equals("False"))
            {
                todo.Visibility = false;
            }

            //If we want to change the name of todo
            if (action != null && action.Equals("updateToDo"))
            {
                if (name != null)
                {
                    todo.Name = name;
                }
            }

            db.Entry(todo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Todos
        [ResponseType(typeof(Todo))]
        public async Task<IHttpActionResult> PostTodo(Todo todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Todos.Add(todo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = todo.Id }, todo);
        }

        // DELETE: api/Todos/5
        [ResponseType(typeof(Todo))]
        public async Task<IHttpActionResult> DeleteTodo(int id)
        {
            Todo todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();

            return Ok(todo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TodoExists(int id)
        {
            return db.Todos.Count(e => e.Id == id) > 0;
        }
    }
}
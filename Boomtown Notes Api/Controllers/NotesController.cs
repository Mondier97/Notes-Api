using Boomtown_Notes_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boomtown_Notes_Api.Controllers
{
    /// <summary>
    /// Controller that handles all of the endpoints for notes. 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        private readonly NotesContext context;

        /// <summary>
        /// Initializes the controller with a connection to the database.
        /// </summary>
        /// <param name="context">Context to interact with the database through entity framework</param>
        public NotesController(NotesContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Returns either a single note by id or all of the notes. Return format is json.
        /// </summary>
        /// <param name="id">optional note id. if one is not given, then all of the notes are returned</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<object> Get(int? id)
        {
            if (id.HasValue)
            {
                Note note = context.Notes.Find(id);

                // we cant find the note so we need to let them know with a 404
                if (note == null)
                    return new NotFoundResult();

                var noteResult = new OkObjectResult(note);
                noteResult.DeclaredType = typeof(Note);

                return noteResult;
            }
            else
            {
                List<Note> notes = new(context.Notes);
                var notesResult = new OkObjectResult(notes);
                notesResult.DeclaredType = typeof(List<Note>);
                
                return notesResult;
            }
        }

        /// <summary>
        /// Creates a new note and Returns the user the new note upon creation.
        /// </summary>
        /// <param name="body">Text for the new note.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<object> Create([FromBody] CreateParams createParams)
        {
            if (createParams.Body == null)
                return new BadRequestResult();

            var note = context.Notes.Add(new Note
            {
                Body = createParams.Body
            });
            context.SaveChanges();
            
            return new OkObjectResult(note.Entity);
        }

        /// <summary>
        /// Changes the text for a given note
        /// </summary>
        /// <param name="newNote">New copy of the note</param>
        /// <returns></returns>
        [HttpPut]
        public ActionResult<object> Edit([FromBody] Note newNote)
        {
            if (newNote.Body == null)
                return new BadRequestResult();

            var note = context.Notes.Find(newNote.Id);

            // they didnt give a valid id
            if (note == null)
                return new NotFoundResult();

            note.Body = newNote.Body;
            context.SaveChanges();
            
            return new OkResult();
        }

        /// <summary>
        /// Deletes a note by id.
        /// </summary>
        /// <param name="id">Id for note to be deleted</param>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult<object> Delete(int id)
        {
            if (context.Notes.Find(id) == null)
                return new NotFoundResult();

            context.Notes.Remove(context.Notes.Find(id));
            context.SaveChanges();
            
            return new OkResult();
        }

        /// <summary>
        /// Class to allow us to get json data through the create method
        /// </summary>
        public class CreateParams
        {
            [BindRequired]
            public string Body { get; set; }
        }
    }
}

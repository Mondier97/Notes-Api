using Boomtown_Notes_Api;
using Boomtown_Notes_Api.Controllers;
using Boomtown_Notes_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Boomtown_Notes_Unit_Tests
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void SetUp()
        {
            /* 
             * I've read you dont need to clean up all of your contexts but oh well. im paranoid
             */
            using (NotesContext context = new("data source=notes.test.sqlite"))
            {
                NotesController controller = new(context);

                // empties table. almost certainly terrible for performance, but sufficient for us!
                context.Notes.RemoveRange(context.Notes);

                // initialize some dummy data
                context.Notes.AddRange(new[]
                    {
                    new Note
                    {
                        Body = "our note!"
                    },
                    new Note
                    {
                        Body = "our next note"
                    },
                    new Note
                    {
                        Body = "our final note"
                    }
                });

                context.SaveChanges();
            }
        }

        [Test]
        public void TestNotesGET()
        {
            using (NotesContext context = new("data source=notes.test.sqlite"))
            {
                NotesController controller = new(context);

                /*
                 * case for no id
                 */
                var actionResult = controller.Get(null);
                var result = actionResult.Result as OkObjectResult;

                // did we actually get a list of notes?
                Assert.IsInstanceOf(typeof(List<Note>), result.Value);

                List<Note> notes = (List<Note>)result.Value;

                // did we get the 3 notes like expected?
                Assert.IsTrue(notes.Count == 3);

                /*
                 * case for existing id
                 */
                Note searchedNote = notes[1];
                actionResult = controller.Get(searchedNote.Id);
                result = actionResult.Result as OkObjectResult;

                // is this a note like we expect?
                Assert.IsInstanceOf(typeof(Note), result.Value);

                Note note = (Note)result.Value;

                // did we get the note we wanted?
                Assert.IsTrue(note.Id == searchedNote.Id && note.Body == searchedNote.Body);

                /*
                 * case for non existing id
                 */
                actionResult = controller.Get(-1);

                // is this a 404?
                Assert.IsInstanceOf(typeof(NotFoundResult), actionResult.Result);
            }
        }

        [Test]
        public void TestNotesPOST()
        {
            using (NotesContext context = new("data source=notes.test.sqlite"))
            {
                NotesController controller = new(context);

                /*
                 * Case for valid body
                 */
                var actionResult = controller.Create(new NotesController.CreateParams
                {
                    Body = "hello world"
                });
                var result = actionResult.Result as OkObjectResult;

                Assert.IsInstanceOf(typeof(OkObjectResult), result);

                Note note = (Note)result.Value;

                // did we get back the same note?
                Assert.IsTrue(note.Body == "hello world");

                /*
                 * Case for invalid body (body doesnt exist)
                 */
                actionResult = controller.Create(new NotesController.CreateParams());

                // im not sure if dotnetcore will allow for invalid bodies on these params but wanted to test it
                Assert.IsInstanceOf(typeof(BadRequestResult), actionResult.Result);
            }
        }

        [Test]
        public void TestNotesPUT()
        {
            using (NotesContext context = new("data source=notes.test.sqlite"))
            {
                NotesController controller = new(context);
                /*
                 * Case for valid id
                 */
                var actionResult = controller.Get(null);
                var result = actionResult.Result as OkObjectResult;
                List<Note> notes = (List<Note>)result.Value;

                actionResult = controller.Edit(new Note
                {
                    Id = notes[1].Id,
                    Body = "note 2"
                });

                Assert.IsInstanceOf(typeof(OkResult), actionResult.Result);

                var editedNote = new List<Note>(context.Notes)[1];
                Assert.IsTrue(editedNote.Id == notes[1].Id && editedNote.Body == "note 2");

                /*
                 * Case for invalid id
                 */
                actionResult = controller.Edit(new Note
                {
                    Id = -1,
                    Body = "note 2"
                });
                Assert.IsInstanceOf(typeof(NotFoundResult), actionResult.Result);

                /*
                 * Case for invalid body
                 */
                actionResult = controller.Edit(new Note
                {
                    Id = notes[1].Id
                });

                Assert.IsInstanceOf(typeof(BadRequestResult), actionResult.Result);
            }
        }

        [Test]
        public void TestNotesDELETE()
        {
            using (NotesContext context = new("data source=notes.test.sqlite"))
            {
                NotesController controller = new(context);

                /*
                 * Case for valid id
                 */
                var actionResult = controller.Create(new NotesController.CreateParams
                {
                    Body = "hello world"
                });
                var result = actionResult.Result as OkObjectResult;
                Note newNote = (Note)result.Value;

                actionResult = controller.Delete(newNote.Id);
                Assert.IsInstanceOf(typeof(OkResult), actionResult.Result);

                var getResult = controller.Get(newNote.Id);
                Assert.IsInstanceOf(typeof(NotFoundResult), getResult.Result);

                /*
                 * Case for invalid id
                 */
                actionResult = controller.Delete(-1);
                Assert.IsInstanceOf(typeof(NotFoundResult), actionResult.Result);
            }
        }
    }
}
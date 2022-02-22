using SC.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SC.RestWebAPI.Controllers
{
    public class ClassController : ApiController
    {
        private StudentEntities _db = new StudentEntities();


        [ResponseType(typeof(IEnumerable<Class>))]
        [Route("api/class")]
        [HttpGet]
        public HttpResponseMessage GetClasses(int studentid)
        {
            try
            {
                var _classList = _db.Classes.Where(x => _db.StudentsClasses.Where(z => z.StudentID == studentid).Select(e => e.ClassID).ToList().Contains(x.ClassID));

                
                if (_classList != null && _classList.ToList().Count > 0)
                {
                    return Request.CreateResponse<IQueryable<Class>>(HttpStatusCode.OK, _classList);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Classes not assigned to student");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while fetching classes based on studentid");
            }

        }

        [Route("api/class")]
        [ResponseType(typeof(Class))]
        public HttpResponseMessage PostClass(Class _class)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                if (_class.ClassID == 0)
                {
                    _db.Classes.Add(_class);
                }
                else
                {
                    _db.Entry(_class).State = EntityState.Modified;
                }

                _db.SaveChanges();
                return Request.CreateResponse<string>(HttpStatusCode.OK, "Class saved successfully(classid:" + _class.ClassID.ToString() + ")");

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while saving class");
            }
        }


        [Route("api/class")]
        public HttpResponseMessage PutClass(Class _class)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                if (_class.ClassID == 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid request details: classid not provided");
                }

                _db.Entry(_class).State = EntityState.Modified;

                _db.SaveChanges();

                return Request.CreateResponse<string>(HttpStatusCode.OK, "Class updated successfully(classid:" + _class.ClassID.ToString() + ")");

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(_class.ClassID))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Class not found based on the id");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while updating class");
            }
        }

        [Route("api/ClassExists")]
        private bool ClassExists(long id)
        {
            return _db.Classes.Count(e => e.ClassID == id) > 0;
        }

        [Route("api/class")]
        public HttpResponseMessage DeleteClass(long id)
        {
            try
            {
                Class _class = _db.Classes.Find(id);
                if (_class == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Class not found based on the id");
                }
                _db.Classes.Remove(_class);
                _db.SaveChanges();

                return Request.CreateResponse<string>(HttpStatusCode.OK, "Class deleted successfully(studentid:" + _class.ClassID.ToString() + ")");
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while deleting student");
            }
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using SC.DataLayer.Models;
using SC.RestWebAPI.Model;
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
    public class StudentController : ApiController
    {
        private StudentEntities _db = new StudentEntities();


        [ResponseType(typeof(IEnumerable<Student>))]
        [Route("api/student")]
        public HttpResponseMessage GetStudents()
        {
            try
            {
                var _studentsList = _db.Students;
                if (_studentsList != null)
                {
                    return Request.CreateResponse<IQueryable<Student>>(HttpStatusCode.OK, _studentsList);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student records not found");
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while fetching students"); 
            }
            
        }

        [ResponseType(typeof(Student))]
        [Route("api/student")]
        public HttpResponseMessage GetStudent(int id)
        {
            try
            {
                Student _student = _db.Students.Find(id);
                if (_student == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student not found based on the id");
                }
                else
                {
                    return Request.CreateResponse<Student>(HttpStatusCode.OK, _student);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while fetching student based on id");
            }
        }

        [Route("api/student")]
        [ResponseType(typeof(Student))]
        public HttpResponseMessage PostEmployee(Student student)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                if (student.StudentID == 0)
                {
                    _db.Students.Add(student);
                }
                else
                {
                    _db.Entry(student).State = EntityState.Modified;
                }

                _db.SaveChanges();
                return Request.CreateResponse<string>(HttpStatusCode.OK, "Student saved successfully(studentid:"+student.StudentID.ToString()+")");
                
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while saving student");
            }
        }


        [Route("api/student")]
        public HttpResponseMessage PutEmployee(Student student)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                if (student.StudentID == 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid request details: studentid not provided");
                }

                _db.Entry(student).State = EntityState.Modified;

                _db.SaveChanges();

                return Request.CreateResponse<string>(HttpStatusCode.OK, "Student updated successfully(studentid:" + student.StudentID.ToString() + ")");

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.StudentID))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student not found based on the id");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while updating student");
            }
        }

        [Route("api/StudentExists")]
        private bool StudentExists(long id)
        {
            return _db.Students.Count(e => e.StudentID == id) > 0;
        }

        [Route("api/student")]
        [ResponseType(typeof(Student))]
        public HttpResponseMessage DeleteEmployee(long id)
        {
            try
            {
                Student student = _db.Students.Find(id);
                if (student == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student not found based on the id");
                }
                _db.Students.Remove(student);
                _db.SaveChanges();

                return Request.CreateResponse<string>(HttpStatusCode.OK, "Student deleted successfully(studentid:" + student.StudentID.ToString() + ")");
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while deleting student");
            }
        }

        [Route("api/assignclasses")]
        [ResponseType(typeof(Student))]
        public HttpResponseMessage AssignClasses(List<StudentClassEntity> studentClasses)
        {
            try
            {
                if (studentClasses != null && studentClasses.Count > 0)
                {
                    string _errorMessage = "";
                    string _successMessage = "";
                    string _notFoundMessage = "";
                    foreach(var item in studentClasses)
                    {
                        if(_db.StudentsClasses.Count(e => e.StudentID == item.StudentID && e.ClassID == item.ClassID) > 0)
                        {
                            //record exist
                            if(_errorMessage == "")
                            {
                                _errorMessage = "StudentID:" + item.StudentID.ToString() + " to ClassID:" + item.ClassID.ToString();
                            }
                            else
                            {
                                _errorMessage = _errorMessage + ", " + "StudentID:" + item.StudentID.ToString() + " to ClassID:" + item.ClassID.ToString();
                            }
                            
                        }
                        else
                        {
                            //not exist


                            //student id is valid or not
                            //class id is valid or not
                            string _currentNotFoundMessage = "";
                            if(_db.Students.Count(e => e.StudentID == item.StudentID) <= 0)
                            {
                                _currentNotFoundMessage = "Student(studentid:" + item.StudentID.ToString() + ")";
                            }

                            if (_db.Classes.Count(e => e.ClassID == item.ClassID) <= 0)
                            {
                                if (!string.IsNullOrEmpty(_currentNotFoundMessage))
                                {
                                    _currentNotFoundMessage = _currentNotFoundMessage + " and Class(classid:" + item.ClassID.ToString() + ")";
                                }
                                else
                                {
                                    _currentNotFoundMessage = "Class(classid: " + item.ClassID.ToString() + ")";
                                }
                            }

                            if (!string.IsNullOrEmpty(_currentNotFoundMessage))
                            {
                                if (_notFoundMessage == "")
                                {
                                    _notFoundMessage = _currentNotFoundMessage;
                                }
                                else
                                {
                                    _notFoundMessage = _notFoundMessage + ", " + _currentNotFoundMessage;
                                }
                            }
                            else
                            {
                                StudentsClass _newSClass = new StudentsClass()
                                {
                                    StudentID = item.StudentID,
                                    ClassID = item.ClassID
                                };

                                _db.StudentsClasses.Add(_newSClass);
                                _db.SaveChanges();

                                if (_successMessage == "")
                                {
                                    _successMessage = "StudentID:" + item.StudentID.ToString() + " to ClassID:" + item.ClassID.ToString();
                                }
                                else
                                {
                                    _successMessage = _successMessage + ", " + "StudentID:" + item.StudentID.ToString() + " to ClassID:" + item.ClassID.ToString();
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(_successMessage))
                    {
                        _successMessage = "Successfully assigned classes to students: " + _successMessage;

                        if (!string.IsNullOrEmpty(_errorMessage))
                        {
                            _errorMessage = "Students already assigned to selected classes: "+ _errorMessage;

                            _successMessage = _successMessage + " - " + _errorMessage;
                        }

                        if (!string.IsNullOrEmpty(_notFoundMessage))
                        {
                            _notFoundMessage = _notFoundMessage  +" not found";

                            _successMessage = _successMessage + " - " + _notFoundMessage;
                        }

                        return Request.CreateResponse<string>(HttpStatusCode.OK, _successMessage);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(_errorMessage))
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Students already assigned to selected classes");
                        }
                        else if (!string.IsNullOrEmpty(_notFoundMessage))
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Students and classes not found");
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid request details: studentid and classid invalid");
                        }
                       
                    }
                    
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid request details: studentid and classid required");
                }

                

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while assigning classes to students");
            }
        }

        [Route("api/removeclasses")]
        [ResponseType(typeof(Student))]
        public HttpResponseMessage DeleteClasses(List<StudentClassEntity> studentClasses)
        {
            try
            {
                if (studentClasses != null && studentClasses.Count > 0)
                {
                    string _errorMessage = "";
                    string _successMessage = "";
                    foreach (var item in studentClasses)
                    {
                        var _dbStudentClass = _db.StudentsClasses.FirstOrDefault(e => e.StudentID == item.StudentID && e.ClassID == item.ClassID);
                        if (_dbStudentClass !=null)
                        {
                            //record exist
                            _db.StudentsClasses.Remove(_dbStudentClass);
                            _db.SaveChanges();

                            if (_successMessage == "")
                            {
                                _successMessage = "StudentID:" + item.StudentID.ToString() + " from ClassID:" + item.ClassID.ToString();
                            }
                            else
                            {
                                _successMessage = _successMessage + ", " + "StudentID:" + item.StudentID.ToString() + " from ClassID:" + item.ClassID.ToString();
                            }                           

                        }
                        else
                        {
                            //not exist

                            if (_errorMessage == "")
                            {
                                _errorMessage = "StudentID:" + item.StudentID.ToString() + " from ClassID:" + item.ClassID.ToString();
                            }
                            else
                            {
                                _errorMessage = _errorMessage + ", " + "StudentID:" + item.StudentID.ToString() + " from ClassID:" + item.ClassID.ToString();
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(_successMessage))
                    {
                        _successMessage = "Successfully deleted classes from students: " + _successMessage;

                        if (!string.IsNullOrEmpty(_errorMessage))
                        {
                            _errorMessage = "Students and classes not found: " + _errorMessage;

                            _successMessage = _successMessage + " - " + _errorMessage;
                        }

                        return Request.CreateResponse<string>(HttpStatusCode.OK, _successMessage);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(_errorMessage))
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Students and classes not found");
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid request details: studentid and classid invalid");
                        }

                    }

                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid request details: studentid and classid required");
                }



            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while deleting classes from students");
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

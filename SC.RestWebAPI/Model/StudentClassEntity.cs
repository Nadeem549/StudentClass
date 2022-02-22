using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SC.RestWebAPI.Model
{
    public class StudentClassEntity
    {
        [Required(ErrorMessage = "StudentID is required")]
        public int StudentID { get; set; }
        [Required(ErrorMessage = "ClassID is required")]
        public int ClassID { get; set; }
    }
}
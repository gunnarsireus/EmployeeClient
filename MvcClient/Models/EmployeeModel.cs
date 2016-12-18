using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcClient.Models
{
    public class EmployeeModel
    {
        public List<EmployeeService.Employee> Employees { get; set; }
    }
}
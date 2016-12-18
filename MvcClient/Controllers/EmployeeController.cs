using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Web;
using System.Web.Mvc;

namespace MvcClient.Controllers
{
    public class EmployeeController : Controller
    {
        public static string GetServerName()
        {
            string serverName = "Unknown";

            ClientSection clientSection =
                ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;

            ChannelEndpointElementCollection endpointCollection =
                clientSection.ElementInformation.Properties[string.Empty].Value as ChannelEndpointElementCollection;
            List<string> endpointNames = new List<string>();
            foreach (ChannelEndpointElement endpointElement in endpointCollection)
            {
                if (endpointElement.Contract == "EmployeeService.IEmployeeService")
                {
                    serverName = endpointElement.Address.AbsoluteUri;
                }
            }

            return serverName;
        }
        // GET: Employee
        public ActionResult Index()
        {
            ViewBag.Connection = GetServerName();
            EmployeeService.EmployeeServiceClient client = new EmployeeService.EmployeeServiceClient();
            var employees = client.GetEmployees().OrderBy(e => e.Id).ToList();
            return View(employees);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IList<EmployeeService.Employee> employees)
        {
            ViewBag.Connection = GetServerName();
            if (employees == null)
            {
                return RedirectToAction("Index");
            }
            EmployeeService.EmployeeServiceClient client = new EmployeeService.EmployeeServiceClient();

            if (ModelState.IsValid)
            {
                client.DeleteEmployee(0);
                foreach (var employee in employees)
                {
                    if (employee.Id == 0)
                    {
                        continue;  //If employee with Id=0 has been entered in the view, ignore he/she, will not be stored in db and will disapear in next page load
                    }
                    if (client.GetEmployee(employee.Id) == null)
                    {
                        client.SaveEmployee(employee);   //employee doesn't exist in Db, Create new
                    }
                    else
                    {
                        client.UpdateEmployee(employee);   //employee do exist in Db, Update     
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            try
            {
                EmployeeService.EmployeeServiceClient client = new EmployeeService.EmployeeServiceClient();
                client.AddEmployee();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                EmployeeService.EmployeeServiceClient client = new EmployeeService.EmployeeServiceClient();
                client.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

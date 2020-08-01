using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DenemeIdentity.Models;
using DenemeIdentity.Security;
using DenemeIdentity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DenemeIdentity.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        private IHostingEnvironment _hostingEnvironment;
        private readonly AppDbContext _context;
        private readonly IDataProtector _protector;

        public HomeController(IEmployeeRepository employeeRepository,
                                IHostingEnvironment hostingEnvironment,
                                AppDbContext context,
                                IDataProtectionProvider dataProtectionProvider,
                                DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _employeeRepository = employeeRepository;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
        }
        //hostingenvironment wwwroot yolunu tutar. Fotoğrafları burdaki images klasörüne kaydetmek istediğimiz için bunu kullanırız.

        //[Route("")]
        //[Route("Home")]
        //[Route("Home/Index")]

        [AllowAnonymous]
        public ViewResult Index()
        {

            var model = _employeeRepository.GetAllEmployees()
                .Select(e =>
                {
                    e.EncryptedId = _protector.Protect(e.Id.ToString());
                    return e;
                });

            return View(model);
        }

        //[Route("Home/Details/{id}")]
        [AllowAnonymous]
        public ViewResult Details(string id)
        {
            //string decryptedId = _protector.Unprotect(id);
            int employeeeId = Convert.ToInt32(_protector.Unprotect(id));

            Employee employee = _employeeRepository.GetEmployee(employeeeId);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", employeeeId);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(employeeeId ),
                PageTitle = "Employee Details"
            };

            return View(homeDetailsViewModel);
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName 
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel editViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(editViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if (model.Photos != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                _employeeRepository.Update(employee);
                return RedirectToAction("Index");
            }

            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photos != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photos.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photos.CopyTo(fileStream);
                }

            }

            return uniqueFileName;
        }

        
    }


}

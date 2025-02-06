using Microsoft.AspNetCore.Mvc;
using Business.Services;
using Business.DTOs;
using Presentation_UI_.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation_UI_.Controllers
{
    public class AdminPageController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IServiceService _serviceService;
        private readonly IProjectLeaderService _projectLeaderService;

        public AdminPageController(ICustomerService customerService, IServiceService serviceService, IProjectLeaderService projectLeaderService)
        {
            _customerService = customerService;
            _serviceService = serviceService;
            _projectLeaderService = projectLeaderService;
        }

        // ✅ Ladda AdminPage med alla kunder, tjänster och projektledare
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var services = await _serviceService.GetAllServicesAsync();
            var projectLeaders = await _projectLeaderService.GetAllProjectLeadersAsync();

            var model = new AdminPageViewModel
            {
                Customers = customers.Select(c => new CustomerViewModel
                {
                    CustomerID = c.CustomerID,
                    CustomerName = c.CustomerName,
                    OrganizationNumber = c.OrganizationNumber,
                    Address = c.Address,
                    Discount = c.Discount ?? 0
                }).ToList(),

                Services = services.Select(s => new ServiceViewModel
                {
                    ServiceID = s.ServiceID,
                    ServiceName = s.ServiceName
                }).ToList(),

                ProjectLeaders = projectLeaders.Select(pl => new ProjectLeaderViewModel
                {
                    ProjectLeaderID = pl.ProjectLeaderID,
                    Name = pl.Name,
                    Email = pl.Email,
                    Phone = pl.Phone,
                    Department = pl.Department
                }).ToList()
            };

            return View(model);
        }


        // 🔵 Lägg till Projektledare (Vänta på databas innan sidan returneras)
        [HttpPost]
        public async Task<IActionResult> AddProjectLeader(ProjectLeaderViewModel projectLeader)
        {
            if (projectLeader == null || string.IsNullOrWhiteSpace(projectLeader.Name) || string.IsNullOrWhiteSpace(projectLeader.Email))
            {
                TempData["Error"] = "Alla fält måste fyllas i korrekt.";
                return RedirectToAction("Index");
            }

            // Skapa en ProjectLeaderDTO från ProjectLeaderViewModel
            var projectLeaderDto = new ProjectLeaderDTO
            {
                Name = projectLeader.Name,
                Email = projectLeader.Email,
                Phone = projectLeader.Phone,
                Department = projectLeader.Department
            };

            // 🟢 Vänta på att projektledaren läggs till i databasen
            await _projectLeaderService.CreateProjectLeaderAsync(projectLeaderDto);

            TempData["Success"] = "Projektledare har lagts till!";
            return RedirectToAction("Index");
        }




        // 🔵 Ta bort Projektledare (Vänta på att databasen uppdateras)
        [HttpPost]
        public async Task<IActionResult> DeleteProjectLeader(int id)
        {
            // 🟢 Vänta på att projektledaren tas bort från databasen
            await _projectLeaderService.DeleteProjectLeaderAsync(id);

            TempData["Success"] = "Projektledare har tagits bort!";
            return RedirectToAction("Index");
        }


        // 🔵 Lägg till Kund
        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerViewModel customerViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Alla fält måste fyllas i korrekt.";
                return RedirectToAction("Index");
            }

            // 🟢 Mappa ViewModel till DTO
            var customerDto = new CustomerDTO
            {
                CustomerID = customerViewModel.CustomerID,
                CustomerName = customerViewModel.CustomerName,
                OrganizationNumber = customerViewModel.OrganizationNumber,
                Address = customerViewModel.Address,
                Discount = customerViewModel.Discount
            };

            await _customerService.CreateCustomerAsync(customerDto);

            TempData["Success"] = "Kund har lagts till!";
            return RedirectToAction("Index");
        }


        // 🔵 Ta bort Kund
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            // 🟢 Vänta på att kunden tas bort från databasen
            await _customerService.DeleteCustomerAsync(id);

            TempData["Success"] = "Kund har tagits bort!";
            return RedirectToAction("Index");
        }

        // 🔵 Lägg till Tjänst
        [HttpPost]
        public async Task<IActionResult> AddService(ServiceViewModel serviceViewModel)
        {
            if (string.IsNullOrWhiteSpace(serviceViewModel.ServiceName))
            {
                TempData["Error"] = "Tjänstens namn får inte vara tomt.";
                return RedirectToAction("Index");
            }

            // 🟢 Omvandla ViewModel till DTO
            var serviceDto = new ServiceDTO
            {
                ServiceName = serviceViewModel.ServiceName
            };

            await _serviceService.CreateServiceAsync(serviceDto);

            TempData["Success"] = "Tjänst har lagts till!";
            return RedirectToAction("Index");
        }


        // 🔵 Ta bort Tjänst
        [HttpPost]
        public async Task<IActionResult> DeleteService(int serviceID)
        {
            await _serviceService.DeleteServiceAsync(serviceID);
            TempData["Success"] = "Tjänst har tagits bort!";
            return RedirectToAction("Index");
        }

    }
}
 
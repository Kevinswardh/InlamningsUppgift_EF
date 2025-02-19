using Microsoft.AspNetCore.Mvc;
using Business.Services;
using Business.DTOs;
using Presentation_UI_.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation_UI_.Controllers
{
    /// <summary>
    /// Handles administrative operations for managing customers, services, and project leaders in the admin page.
    /// </summary>
    public class AdminPageController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IServiceService _serviceService;
        private readonly IProjectLeaderService _projectLeaderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminPageController"/> with the required services.
        /// </summary>
        /// <param name="customerService">Service for customer operations.</param>
        /// <param name="serviceService">Service for service-related operations.</param>
        /// <param name="projectLeaderService">Service for project leader management.</param>
        public AdminPageController(ICustomerService customerService, IServiceService serviceService, IProjectLeaderService projectLeaderService)
        {
            _customerService = customerService;
            _serviceService = serviceService;
            _projectLeaderService = projectLeaderService;
        }


        // ✅ Ladda AdminPage med alla kunder, tjänster och projektledare
        /// <summary>
        /// Loads the admin page with lists of customers, services, and active project leaders.
        /// Filters out deleted project leaders or those with a <c>ProjectLeaderID</c> of -1.
        /// </summary>
        /// <returns>The admin page view populated with the necessary data.</returns>
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var services = await _serviceService.GetAllServicesAsync();
            var projectLeaders = await _projectLeaderService.GetAllProjectLeadersAsync();

            // Filtrera bort de projektledare som är markerade som borttagna eller har ProjectLeaderID == -1
            var activeProjectLeaders = projectLeaders
                .Where(pl => pl.IsDeleted == 0 && pl.ProjectLeaderID != -1)  // Lägg till kontroll för ProjectLeaderID != -1
                .ToList();

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

                ProjectLeaders = activeProjectLeaders.Select(pl => new ProjectLeaderViewModel
                {
                    ProjectLeaderID = pl.ProjectLeaderID,
                    FirstName = pl.FirstName,
                    LastName = pl.LastName,
                    Email = pl.Email,
                    Phone = pl.Phone,
                    Department = pl.Department
                }).ToList()
            };

            return View(model);
        }




        // 🔵 Lägg till Projektledare (Vänta på databas innan sidan returneras)
        /// <summary>
        /// Adds a new project leader to the database based on the provided view model.
        /// </summary>
        /// <param name="projectLeader">The project leader view model containing the details.</param>
        /// <returns>Redirects to the <c>Index</c> action with a success or error message.</returns>

        [HttpPost]
        public async Task<IActionResult> AddProjectLeader(ProjectLeaderViewModel projectLeader)
        {
            if (projectLeader == null || string.IsNullOrWhiteSpace(projectLeader.FirstName) || string.IsNullOrWhiteSpace(projectLeader.LastName) || string.IsNullOrWhiteSpace(projectLeader.Email))
            {
                TempData["Error"] = "Alla fält måste fyllas i korrekt.";
                return RedirectToAction("Index");
            }

            // Skapa en ProjectLeaderDTO från ProjectLeaderViewModel
            var projectLeaderDto = new ProjectLeaderDTO
            {
                FirstName = projectLeader.FirstName, // ✅ Ny kod
                LastName = projectLeader.LastName,   // ✅ Ny kod
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
        /// <summary>
        /// Deletes a project leader by their ID and updates the database.
        /// </summary>
        /// <param name="id">The unique identifier of the project leader to delete.</param>
        /// <returns>Redirects to the <c>Index</c> action with a success message.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteProjectLeader(int id)
        {
            // 🟢 Vänta på att projektledaren tas bort från databasen
            await _projectLeaderService.DeleteProjectLeaderAsync(id);

            TempData["Success"] = "Projektledare har tagits bort!";
            return RedirectToAction("Index");
        }


        // 🔵 Lägg till Kund
        /// <summary>
        /// Adds a new customer to the database using the provided customer view model.
        /// </summary>
        /// <param name="customerViewModel">The customer view model containing the customer information.</param>
        /// <returns>Redirects to the <c>Index</c> action with a success or error message.</returns>
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
        /// <summary>
        /// Deletes a customer from the database based on their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the customer to delete.</param>
        /// <returns>Redirects to the <c>Index</c> action with a success message.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            // 🟢 Vänta på att kunden tas bort från databasen
            await _customerService.DeleteCustomerAsync(id);

            TempData["Success"] = "Kund har tagits bort!";
            return RedirectToAction("Index");
        }

        // 🔵 Lägg till Tjänst
        /// <summary>
        /// Adds a new service to the database using the provided service view model.
        /// </summary>
        /// <param name="serviceViewModel">The service view model containing service details.</param>
        /// <returns>Redirects to the <c>Index</c> action with a success or error message.</returns>
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
        /// <summary>
        /// Deletes a service from the database based on the service ID.
        /// </summary>
        /// <param name="serviceID">The unique identifier of the service to delete.</param>
        /// <returns>Redirects to the <c>Index</c> action with a success message.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteService(int serviceID)
        {
            await _serviceService.DeleteServiceAsync(serviceID);
            TempData["Success"] = "Tjänst har tagits bort!";
            return RedirectToAction("Index");
        }

    }
}
 
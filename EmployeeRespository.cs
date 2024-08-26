using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;
using System.Threading;


namespace challenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext ?? throw new ArgumentNullException(nameof(employeeContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public Employee Add(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));
            employee.EmployeeId = Guid.NewGuid().ToString();

            _employeeContext.Employees.Add(employee);
            _logger.LogInformation($"Added new employee with ID {employee.EmployeeId}");
            return employee;
        }

        public Employee GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("employee ID cannot be null or NoWhiteSpace", nameof(id));
            var employee = _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
            if (employee == null)
            {
                _logger.LogWarning($"Employee with ID {id} not found");
            }
            LoadDirectReports(employee);
            return employee;
        }
        private void LoadDirectReports(Employee employee)
        {
            if (employee != null)
            {
                _employeeContext.Entry(employee).Collection(e => e.DirectReports).Load();

                foreach (var directReport in employee.DirectReports)
                {
                    LoadDirectReports(directReport);
                }
            }
        }

        public Task SaveAsync()
        {
            _logger.LogInformation("saving employee changes to the employee database");

            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));
            _employeeContext.Employees.Remove(employee);
            _logger.LogInformation($"Removed employee with ID {employee.EmployeeId}");
            return employee;
        }

        public async Task<Employee> UpdatedirectReports(string employeeId, List<Employee> newdirectReports)
        {
            if (string.IsNullOrWhiteSpace(employeeId)) throw new ArgumentNullException(nameof(employeeId));
            var employee = await _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefaultAsync(e => e.EmployeeId == employeeId);
            employee.DirectReports = newdirectReports;
            _employeeContext.SaveChangesAsync();
            return employee;
        }

    }
    }


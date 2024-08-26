using challenge.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        Employee Add(Employee employee);
        Task SaveAsync();

        Employee Remove(Employee employee);

        Task<Employee> UpdatedirectReports(string employeeId, List<Employee> newdirectReports);


    }
}
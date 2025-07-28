using combined;
using GrpcServer.Model;

namespace GrpcServer.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> _employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "John Doe" },
            new Employee { Id = 2, Name = "Jane Smith" },
            new Employee { Id = 3, Name = "Alice Johnson" }
        };
        public EmployeeRepository() { }

        public Employee GetById(int id)
        {
            return _employees.FirstOrDefault(e => e.Id == id);
        }

        public EmployeeSummary AddEmployees(List<Employee> employees)
        {
            if (employees == null || !employees.Any())
            {
                return new EmployeeSummary { Total = 0 };
            }
            _employees.AddRange(employees);
            return new EmployeeSummary { Total = _employees.Count };
        }

        public List<Employee> GetAllEmployees()
        {
            return _employees;
        }
    }
}

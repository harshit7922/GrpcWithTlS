using combined;
using GrpcServer.Model;

namespace GrpcServer.Repository
{
    public interface IEmployeeRepository
    {
       Employee GetById(Int32 Id);
       EmployeeSummary AddEmployees(List<Employee> employees);
       List<Employee> GetAllEmployees();        
    }
}

using combined;
using GrpcServer.Model;
using AutoMapper;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GrpcServer
{
    public class GrpcMappingProfile : Profile
    {
        public GrpcMappingProfile()
        {
            CreateMap<Employee, EmployeeReply>();
            CreateMap<EmployeeRequest, Employee>();
            CreateMap<EmployeeSummaryModel, EmployeeSummary>();
        }
    }
}

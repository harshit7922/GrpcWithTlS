using AutoMapper;
using clientstreaming;
using combined;
using Grpc.Core;
using GrpcServer.Model;
using GrpcServer.Repository;
using EmployeeReply = combined.EmployeeReply;
using EmployeeRequest = combined.EmployeeRequest;

namespace GrpcServer.Services
{
    public class FullEmployeeServiceImpl : FullEmployeeService.FullEmployeeServiceBase
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        public FullEmployeeServiceImpl(IMapper mapper, IEmployeeRepository repo)
        {
            _mapper = mapper;
            _employeeRepository = repo;
        }
        public override Task<EmployeeReply> GetEmployee(EmployeeRequest request, ServerCallContext context)
        {
            var model = _employeeRepository.GetById(request.Id);
            var reply = _mapper.Map<EmployeeReply>(model);
            return Task.FromResult(reply);
        }
        public override async Task<EmployeeSummary> AddEmployees(IAsyncStreamReader<EmployeeRequest> requestStream, ServerCallContext context)
        {
            var employeeList = new List<Employee>();

            await foreach (var req in requestStream.ReadAllAsync())
            {
                employeeList.Add(_mapper.Map<Employee>(req));
            }

            var result = _employeeRepository.AddEmployees(employeeList);
            return _mapper.Map<EmployeeSummary>(result);
        }

        // Replace the incorrect override with the correct signature using EmployeeReply
        //public override async Task StreamAllEmployees(Empty request, IServerStreamWriter<EmployeeReply> responseStream, ServerCallContext context)
        //{
        //    var employees = _employeeRepository.GetAllEmployees();
        //    foreach (var employee in employees)
        //    {
        //        var reply = _mapper.Map<EmployeeReply>(employee);
        //        await responseStream.WriteAsync(reply);
        //    }
        //}

        public override async Task StreamAllEmployees(Empty request, IServerStreamWriter<EmployeeReply> responseStream, ServerCallContext context)
        {
            try
            {
                var employees = _employeeRepository.GetAllEmployees() ?? new List<Employee>();
                foreach (var employee in employees)
                {
                    if (employee == null) continue;
                    var reply = _mapper.Map<EmployeeReply>(employee);
                    if (reply != null)
                        await responseStream.WriteAsync(reply);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use your logger here)
                Console.WriteLine($"Exception in StreamAllEmployees: {ex}");
                throw new RpcException(new Status(StatusCode.Internal, "Server error in StreamAllEmployees"));
            }
        }

        public override async Task Chat(IAsyncStreamReader<chatMessage> request, IServerStreamWriter<chatMessage> serverStreamWriter, ServerCallContext context)
        {
            await foreach (var message in request.ReadAllAsync(context.CancellationToken))
            {
                // Optionally, you can process or log the message here
               message.Message += "hello";
                // Echo the message back to the client
                await serverStreamWriter.WriteAsync(message);
            }
        }
    }
}

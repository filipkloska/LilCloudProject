using CliWrap;
using CliWrap.Buffered;
using Grpc.Core;
using LilCloudServerConsole.Classes;
using LilCloudServerConsole.Database;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
namespace LilCloudServerConsole.Services
{
    public class LilCloudService : LilCloud.LilCloudBase
    {
        private readonly ILogger<LilCloudService> _logger;
        private readonly IConfiguration _config;
        private readonly JwtTokenService _jwt;
        private readonly DbHandler _dbHandler;

        public LilCloudService(
            ILogger<LilCloudService> logger,
            IConfiguration config,
            JwtTokenService jwtTokenService,
            DbHandler dbHandler)
        {
            _logger = logger;
            _config = config;
            _jwt = jwtTokenService;
            _dbHandler = dbHandler;
        }
        public override Task<RegisterReply> RegisterAccount(RegisterRequest request, ServerCallContext context)
        {
            return Task.FromResult(new RegisterReply
            {
                Status = GenericStatus.GsOk,
            });
        }

        public override async Task<AccessReply> AccessAccount(AccessRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Accessing account");
            var user = await _dbHandler.GetUser(request.Login, request.Password);
            if (user == null)
            {
                return await Task.FromResult(new AccessReply
                {
                    Status = GenericStatus.GsError,
                });
            }
            else
            {
                return await Task.FromResult(new AccessReply
                {
                    Status = GenericStatus.GsOk,
                    AuthToken = _jwt.GenerateToken(user)
                });
            }
        }

        public override async Task<ConnectionReply> EstablishConnection(ConnectionRequest request, ServerCallContext context)
        {
            var os = RuntimeInformation.OSDescription;
            string cpuName = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                foreach (ManagementObject mo in mos.Get())
                {
                    _logger.LogInformation(mo["Name"].ToString());
                    cpuName += mo["Name"].ToString();
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var result = await Cli
                  .Wrap("bash")
                  .WithArguments(["-c", "lscpu | sed -nr '/Model name/ s/.*:\\s*(.*) @ .*/\\1/p'"])
                  .ExecuteBufferedAsync();
                cpuName += result.StandardOutput.Trim();
            }

            return await Task.FromResult(new ConnectionReply
            {
                OSName = os.ToString(),
                CPUName = cpuName,
                CPULimit = _config.GetValue<uint>("HostData:CpuLimit"),
                TcpPort = _config.GetValue<int>("HostData:TcpPort"),
                //AuthToken = _jwt.GenerateToken()
            });

        }

        public override async Task UploadFile(IAsyncStreamReader<FileChunk> requestStream, IServerStreamWriter<UploadFileReply> responseStream, ServerCallContext context)
        {
            var fileName = context.RequestHeaders.GetValue("filename");
            var username = context.UserState["Username"].ToString();

            var appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads");

            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }
            var dirPath = Path.Combine(appPath, username);
            _logger.LogInformation($"Upload file: {fileName} at {dirPath}");
            
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var fullPath = Path.Combine(dirPath, fileName);
            _logger.LogInformation($"Full path: {fullPath}");

            //try catch here
            await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);

            await foreach (var chunk in requestStream.ReadAllAsync())
            {
                if (chunk.Data.Length > 0)
                {
                    await fs.WriteAsync(chunk.Data.Memory);
                }
            }

            await fs.FlushAsync();

            await responseStream.WriteAsync(new UploadFileReply
            {
                Completion = 100
            });
            _logger.LogInformation($"Upload complete: {fullPath}");
            var id = (int)context.UserState["UserId"];
            var user = await _dbHandler.GetUser(id);
            await _dbHandler.AddFile(new FileData
            {
                FileName = fileName,
                FileSavePath = fullPath,
                UserId = user.Id,
                Owner = user
            }, user);
        }
        
        public override async Task<AvailabilityStatusReply> CheckAvailability(EmptyRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new AvailabilityStatusReply
            {
                Status = AvailabilityStatus.StatusAvailable
            });
        }
    }
}

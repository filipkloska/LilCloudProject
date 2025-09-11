using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace LilCloudServerConsole.Database
{
    public class DbHandler
    {
        private CloudContext _db;
        private readonly ILogger<DbHandler> _logger;

        //functions to create: cleanEntireDb (with all the files!!!) [admin]
        //clean userdbfiles (with real files) [admin for different users/only for yourself]
        //return an IEnumerable with filedata for a user
        //return a file with a specified name belonging for a user - admin can do that for everyone
        public DbHandler(CloudContext db, ILogger<DbHandler> logger) 
        {
            _db = db;
            _logger = logger;
        }

        public async Task AddUser(string login, string password)
        {
            _db.Users.Add(new User
            {
                Name = login,
                Password = password, // HASH
                IsAdmin = false
            });
            await _db.SaveChangesAsync();
        }
        public async Task AddAdmin(string login, string password)
        {
            _db.Users.Add(new User
            {
                Name = login,
                Password = password, // HASH
                IsAdmin = true
            });
            await _db.SaveChangesAsync();
        }
        public async Task<User?> GetUser(string login, string password)
        {
            return await _db.Users
                    .FirstOrDefaultAsync(u => u.Name == login && u.Password == password);
        }
        public async Task<User?> GetUser(int id)
        {
            return await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task AddFile(FileData fileData, User user)
        {
            _logger.LogInformation("Adding a file");
            _db.Files.Add(new FileData
            {
                FileName = fileData.FileName,
                UserId = user.Id,
                FileSavePath = fileData.FileSavePath,
                Owner = user //this is ass, duplicate data
            });
            await _db.SaveChangesAsync();
        }
        //removeFile [User]
        //getFiles
    }
}

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using EDP_Uplay.Models;
using Dapper;
// RUIDONG Redemption
namespace EDP_Uplay.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RedemptionsController : ControllerBase
    {
        public static string ConnectionString
        {
            get
            {
                #region ConnectionString
                // 创建 ConfigurationBuilder 对象
                var builder = new ConfigurationBuilder()
                    // 指定 appsettings.json 文件路径（如果不在当前目录）
                    .SetBasePath(Directory.GetCurrentDirectory())
                    // 添加 JSON 格式的配置源
                    .AddJsonFile("appsettings.json");
                // 构建 Configuration 对象
                IConfiguration configuration = builder.Build();
                // 使用 GetConnectionString 方法获取数据库连接字符串
                string connectionString = configuration.GetConnectionString("DefaultConnection");
                return connectionString;
                #endregion
            }
        }

        [HttpGet]
        public List<Redemptions> GetRedemptionsList()
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                // 查询数据
                var result = connection.Query<Redemptions>("select * from Redemptions").ToList();
                return result;
            }
        }

        [HttpGet]
        public Redemptions Get(int id = 0)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                return connection.QuerySingleOrDefault<Redemptions>($"select * from Redemptions where id={id}");
            }
        }

        [HttpDelete]
        public MessageModel<string> Delete(int id = 0)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute($"DELETE FROM Redemptions WHERE Id ={id}");
                return new MessageModel<string>()
                {
                    msg = "保存成功",
                    success = true,
                    response = "保存成功"
                };
            }
        }

        [HttpPost]
        public MessageModel<string> Create([FromForm] RedemptionsRequest request)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string filePath = string.Empty;
                if (request.file != null && request.file.Length > 0)
                {
                    // 指定要保存文件的路径  
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", request.file.FileName);

                    // 创建文件流以保存文件  
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        request.file.CopyToAsync(stream);
                    }
                }
                //connection.Execute($@"INSERT INTO Redemptions(title, image,needPoints,kind,createdAt,updatedAt)  VALUES('{request.title}','/uploads/{request.file.FileName}',{request.needPoints},'{request.kind}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                //return new MessageModel<string>()
                //{
                //    msg = "保存成功",
                //    success = true,
                //    response = "保存成功"
                // Use parameters instead of string interpolation to avoid SQL syntax errors
                string sql = "INSERT INTO Redemptions (title, image, needPoints, kind, createdAt, updatedAt) VALUES (@Title, @Image, @NeedPoints, @Kind, @CreatedAt, @UpdatedAt)";

                var parameters = new
                {
                    Title = request.title,
                    Image = $"/uploads/{request.file.FileName}",
                    NeedPoints = request.needPoints,
                    Kind = request.kind,
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                connection.Execute(sql, parameters);
                return new MessageModel<string>()
                {
                    msg = "保存成功", // "Saved successfully"
                    success = true,
                    response = "保存成功" // "Saved successfully"
                };
            }
        }

        [HttpPut]
        public MessageModel<string> Edit([FromForm] RedemptionsRequest request)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string filePath = string.Empty;
                if (request.file != null && request.file.Length > 0)
                {
                    // 指定要保存文件的路径  
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", request.file.FileName);

                    // 创建文件流以保存文件  
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        request.file.CopyToAsync(stream);
                    }
                }
                //connection.Execute($@"update Redemptions set title='{request.title}',image='/uploads/{request.file.FileName}',kind='{request.kind}',needPoints={request.needPoints} where id={request.Id}");
                //return new MessageModel<string>()
                // {
                //  msg = "保存成功",
                //  success = true,
                //  response = "保存成功"
                string sql = "UPDATE Redemptions SET title=@Title, image=@Image, kind=@Kind, needPoints=@NeedPoints WHERE id=@Id";

                var parameters = new
                {
                    Title = request.title,
                    Image = $"/uploads/{request.file.FileName}",
                    Kind = request.kind,
                    NeedPoints = request.needPoints,
                    Id = request.Id
                };

                connection.Execute(sql, parameters);
                return new MessageModel<string>()
                {
                    msg = "保存成功", // "Saved successfully"
                    success = true,
                    response = "保存成功" // "Saved successfully"
                };
            }
        }
    }
}


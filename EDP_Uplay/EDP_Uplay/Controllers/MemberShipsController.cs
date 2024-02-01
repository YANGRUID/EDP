using EDP_Uplay.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Xml.Linq;
using Dapper;
namespace EDP_Uplay.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberShipsController : ControllerBase
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
        public List<MemberShips> GetMemberShipsList()
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                // 查询数据
                var result = connection.Query<MemberShips>("select * from MemberShips").ToList();
                return result;
                //// 插入数据
                //var newCustomer = new { CustomerId = 1 Name = John Doe };
                //connection.Execute(INSERT INTO Customers(CustomerId Name) VALUES(@CustomerId @Name) newCustomer);

                //// 更新数据
                //var updatedCustomer = new { CustomerId = 1 Name = Jane Smith };
                //connection.Execute(UPDATE Customers SET Name = @Name WHERE CustomerId = @CustomerId updatedCustomer);

                //// 删除数据
                //var customerIdToDelete = 1;
                //connection.Execute(DELETE FROM Customers WHERE CustomerId = @CustomerId new { CustomerId = customerIdToDelete });

                //return new MessageModel<List<MemberShips>>()
                //{
                //    msg = "获取成功",
                //    success = true,
                //    response = result
                //};
            }
        }

        [HttpGet]
        public MemberShips Get(int id = 0)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                return connection.QuerySingleOrDefault<MemberShips>($"select * from MemberShips where id={id}");
            }
        }

        [HttpDelete]
        public MessageModel<string> Delete(int id = 0)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute($"DELETE FROM MemberShips WHERE Id ={id}");
                return new MessageModel<string>()
                {
                    msg = "保存成功",
                    success = true,
                    response = "保存成功"
                };
            }
        }

        [HttpPost]
        public MessageModel<string> Create(MemberShips request)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute($@"INSERT INTO MemberShips(name, description,needPoints,color,createdAt,updatedAt)  VALUES('{request.name}','{request.description}',{request.needPoints},'{request.color}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                return new MessageModel<string>()
                {
                    msg = "保存成功",
                    success = true,
                    response = "保存成功"
                };
            }
        }

        [HttpPut]
        public MessageModel<string> Edit(MemberShips request)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute($@"update MemberShips set name='{request.name}',description='{request.description}',color='{request.color}',needPoints={request.needPoints} where id={request.Id}");
                return new MessageModel<string>()
                {
                    msg = "保存成功",
                    success = true,
                    response = "保存成功"
                };
            }
        }
    }
}


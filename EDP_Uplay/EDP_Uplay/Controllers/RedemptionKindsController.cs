
using Dapper;
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

namespace EDP_Uplay.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RedemptionKindsController : ControllerBase
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

        #region kind
        [HttpGet]
        public List<RedemptionKinds> GetKindsList()
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                // 查询数据
                var result = connection.Query<RedemptionKinds>("select * from RedemptionKinds").ToList();

                return result;
            }
        }

        [HttpGet]
        public List<RedemptionKindsDTO> GetRedemptionKindsList()
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                List<RedemptionKindsDTO> result = new List<RedemptionKindsDTO>();
                // 查询数据
                var data = connection.Query<RedemptionKinds>("select * from RedemptionKinds").ToList();
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        RedemptionKindsDTO redemptionKindsDTO = new RedemptionKindsDTO();
                        redemptionKindsDTO.kind = item.name;
                        var redemptionsList = connection.Query<Redemptions>($"select * from Redemptions where kind='{item.name}'").ToList();
                        if (redemptionsList != null && redemptionsList.Count > 0)
                        {
                            redemptionKindsDTO.redemptions = redemptionsList;
                        }
                        else
                        {
                            redemptionKindsDTO.redemptions = new List<Redemptions>();
                        }
                        result.Add(redemptionKindsDTO);
                    }
                }
                return result;
            }
        }

        [HttpPost]
        public MessageModel<string> Create(RedemptionKinds request)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute($@"INSERT INTO RedemptionKinds(name, createdAt,updatedAt)  VALUES('{request.name}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')");
                return new MessageModel<string>()
                {
                    msg = "保存成功",
                    success = true,
                    response = "保存成功"
                };
            }
        }
        #endregion 
    }
}


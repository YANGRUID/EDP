using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EDP_Uplay.Models
{
    public class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Apply 方法将循环检查每个模型属性。将Nullable为false的项添加进添加到Required对象列表中
        /// </summary>
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            var additionalRequiredProps = model.Properties
                .Where(x => !x.Value.Nullable && !model.Required.Contains(x.Key))
                .Select(x => x.Key);
            foreach (var propKey in additionalRequiredProps)
            {
                model.Required.Add(propKey);
            }
        }
    }
}


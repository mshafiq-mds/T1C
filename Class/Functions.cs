using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using Prodata.WebForm.Models;

namespace Prodata.WebForm.Class
{
    public class Functions
    {
        public static string GetGeneratedRefNo(string refType, bool isPreview) // false = preview and save / true = preview
        {
            using (var context = new AppDbContext())
            {
                var refTypeParam = new SqlParameter("@RefType", refType);
                var bizAreaParam = new SqlParameter("@BizArea", Auth.User().iPMSBizAreaCode);
                var isPreviewParam = new SqlParameter("@IsPreview", isPreview ? 1 : 0);

                var outputParam = new SqlParameter
                {
                    ParameterName = "@NewRefNo",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 30,
                    Direction = ParameterDirection.Output
                };

                context.Database.ExecuteSqlCommand(
                    "EXEC GenerateNewBudgetRefNo @RefType, @BizArea, @IsPreview, @NewRefNo OUTPUT",
                    refTypeParam, bizAreaParam, isPreviewParam, outputParam
                );

                return outputParam.Value?.ToString();
            }
        }


    }
}
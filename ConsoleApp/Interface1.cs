using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    interface Interface1
    {
        List<ProductInfo> GetProductInfoList(string orgCode, DateTime beginTime, DateTime endTime);
    }
}

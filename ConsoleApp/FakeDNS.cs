using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class FakeDNS
    {
        /// <summary>
        /// 通过设置这个属性，可以在发出连接的时候绑定客户端发出连接所使用的IP地址。 
        /// </summary>
        /// <param name="servicePoint"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        public static IPEndPoint BindIPEndPointCallback(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount)
        {
            return new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);//端口号
        }
        /// <summary>
        /// 一个服务器上面配置多个IP 固定出网IP
        /// </summary>
        public static void MakeRequest()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.baidu.com");
                //设置本地的出口ip和端口
                //request.ServicePoint.BindIPEndPointDelegate += BindIPEndPointCallback; //new BindIPEndPoint(BindIPEndPointCallback);
                //if (ServicePointManager.DefaultConnectionLimit < 10)
                //{
                //    ServicePointManager.DefaultConnectionLimit = 10;
                //}
                //req.ServicePoint.ConnectionLimit=int.Max;  //允许最大连接数 

                System.Net.WebProxy proxy = new WebProxy("127.0.0.1", 80);
                request.Proxy = proxy;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader;
                myStreamReader = new StreamReader(myResponseStream, System.Text.Encoding.GetEncoding("utf-8"));
                //post返回的数据
                string receiveData = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                Console.WriteLine(receiveData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

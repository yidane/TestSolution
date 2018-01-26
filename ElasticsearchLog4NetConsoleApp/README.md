#使用Elasticsearch保存log4net日志
##1.elasticsearch环境搭建
###1.1.使用docker搭建elasticsearch环境
~~~ docker
sudo docker pull elasticsearch
sudo docker pull kibana
sudo docker run --name es -it -d -p 9200:9200 -p 9300:9300 elasticsearch
sudo docker run --name kibana -e ELASTICSEARCH_URL=http://172.17.0.2:9200 -p 5601:5601 -d kibana
~~~
此处ELASTICSEARCH_URL为elasticsearch容器的ip，在对外映射了端口情况下，可以直接使用本机ip，但注意要保证hibana所在docker能连接该地址。
配置完毕之后，打开浏览器测试

链接 | 测试
- | -
 http://127.0.0.1/9200 | 测试elasticsearch是否运行正常 
 http://127.0.0.1/5601 | 测试kibana是否运行正常 
 	
##1.2 创建日志索引
在elasticsearch中创建名为log的索引
索引的json格式（index.json） 如下
~~~ index
{
    "settings": {
        "index": {
            "number_of_shards": 1,
            "number_of_replicas": 0
        }
    },
    "mappings": {
        "LogEvent": {
            "properties": {
                "timeStamp": {
                    "type": "date",
                    "format": "dateOptionalTime"
                },
                "message": {
                    "type": "string"
                },
                "messageObject": {
                    "type": "object"
                },
                "exception": {
                    "type": "object"
                },
                "loggerName": {
                    "type": "string"
                },
                "domain": {
                    "type": "string"
                },
                "identity": {
                    "type": "string"
                },
                "level": {
                    "type": "string"
                },
                "className": {
                    "type": "string"
                },
                "fileName": {
                    "type": "string"
                },
                "name": {
                    "type": "string"
                },
                "fullInfo": {
                    "type": "string"
                },
                "methodName": {
                    "type": "string"
                },
                "fix": {
                    "type": "string"
                },
                "properties": {
                    "type": "string"
                },
                "userName": {
                    "type": "string"
                },
                "threadName": {
                    "type": "string"
                },
                "hostName": {
                    "type": "string"
                }
            }
        }
    }
}
~~~
创建索引脚本（init.ssh）
~~~ init
echo "Attempting to delete log index (OK if not found)"
curl -X DELETE http://localhost:9200/log
echo ""
echo "Attempting to create log index"
curl -X PUT http://localhost:9200/log -d @index.json
echo ""
~~~
##2.log4net测试
###2.1 使用nuget安装第三方动态库
~~~ nuget
Install-Package log4net -Version 2.0.8
Install-Package log4net.ElasticSearch -Version 2.3.5
~~~
###2.2 配置log4net
####2.2.1 修改app.config配置文件
~~~ appconfig
<configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
</configSections>
<log4net>
    <!-- Default style appender. You can change the name but make sure you reference it corectly. Change the type if you're using .NET 4.0 -->
    <appender name="ElasticSearchAppender" type="log4net.ElasticSearch.ElasticSearchAppender, log4net.ElasticSearch">
      <layout type="log4net.Layout.PatternLayout,log4net">
        <param name="ConversionPattern" value="%d{ABSOLUTE} %-5p %c{1}:%L - %m%n" />
      </layout>

      <!-- You can also use scheme, user, password here if you need https and http authentication, see the docs -->
      <connectionString value="Server=10.100.14.152;Index=log;Port=9200;rolling=false"/>

      <!-- false means do not eject older messages when the buffer is full, try to keep them all. -->
      <lossy value="false" />

      <!-- Any buffer > 1 will use the Elasticsearch _bulk API. Tune as needed depending on how high volume your app is and how fast you want messages to show up in ES -->
      <bufferSize value="100" />

      <!-- When an event comes in that matches or is above the treshold, the buffer will flush. I don't reccommend setting to "ALL" or it will defeat the purpose of having a buffer -->
      <evaluator type="log4net.Core.LevelEvaluator">
        <threshold value="ERROR"/>
      </evaluator>
    </appender>

    <!-- Common to all log4net configs. See log4net docs for more details  -->
    <root>
      <level value="ALL"/>
      <appender-ref ref="ElasticSearchAppender" />
    </root>
  </log4net>
~~~
其中connectionString为elasticsearch发布出来的url,参数rolling表示是否每天自动创建新索引。
####2.2.2 在Assembly.cs中注册log4net
~~~ Assembly
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
~~~
如果log4net配置文件是单独文件，注册方式如下
~~~ Assembly
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "configuration.xml", Watch = true)]
~~~
####2.2.3 编写测试代码
~~~ csharp
class Program
    { 
        // Create a new logging instance
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for(int i = 0; i < 100000; i++)
            {
                // Most basic logging example.
                _log.ErrorFormat("System Error {0}", "Divide by zero error.");
                _log.ErrorFormat("System Error {0}", "Divide by zero error.");
                _log.ErrorFormat("System Error {0}", "Divide by zero error.");
                _log.ErrorFormat("System Error {0}", "Divide by zero error.");
                _log.ErrorFormat("System Error {0}", "Divide by zero error.");
                _log.ErrorFormat("System Error {0}", "Divide by zero error.");
                _log.ErrorFormat("System Error {0}", "Divide by zero error.");
                _log.ErrorFormat("System Error {0}", "Divide by zero error.");
             
                // Log a message with an exeption object
                _log.Error("System Error", new Exception("Something terrible happened."));

                // Log an exception object with custom fields in the Data property
                Exception newException = new Exception("There was a system error");
                newException.Data.Add("CustomProperty", "CustomPropertyValue");
                newException.Data.Add("SystemUserID", "User43");

                _log.Error("Something broke.", newException);
            }
            watch.Stop();

            Console.WriteLine(watch.ElapsedMilliseconds);

            Console.ReadKey();
        }
    }
~~~
##3 测试kibana检索数据
打开http://127.0.0.1/5601，配置查询索引为log执行查询，即可查询到实时log4net日志。
还可以配置图形化工具，展示除饼图、条形图等
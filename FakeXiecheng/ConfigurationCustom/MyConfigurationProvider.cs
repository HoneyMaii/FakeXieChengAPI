using System;
using System.Timers;
using Microsoft.Extensions.Configuration;

namespace FakeXieCheng.API.ConfigurationCustom
{
    class MyConfigurationProvider: ConfigurationProvider // ConfigurationProvider  继承了 IConfigurationProvider
    {
        private Timer timer;
        public MyConfigurationProvider():base()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 3000; // ms
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Load(true);
        }

        public override void Load()
        {
            // 加载数据
            Load(false);
        }

        void Load(bool reload)
        {
            // 这里可以远程调用获取配置，比如 阿波罗配置中心
            // 结合 配置文件 做出配置的调整
            
            Data["lastTime"] = DateTime.Now.ToString(); // Data：ConfigurationProvider 里的数据承载集合
            if(reload) OnReload();
        }
    }
}
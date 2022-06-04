using System;
using Snowflake.Core;

namespace FakeXieCheng.API.Helper
{
    // 获取 ❄️雪花算法 worker 单例
    public class IdGenerator
    {
        private static volatile IdWorker _idWorker;
        private static readonly object obj = new object();

        private IdGenerator()
        {
        }

        public static IdWorker GetWorker()
        {
            if (_idWorker == null)
            {
                lock (obj)  
                {
                    if (_idWorker == null)
                    {
                        _idWorker = new IdWorker(1, 1);
                    }
                }
            }

            return _idWorker;
        }

        // 获取下一个ID
        public static long GetNextId()
        {
            var worker = GetWorker();
            return worker.NextId();
        }
    }
}
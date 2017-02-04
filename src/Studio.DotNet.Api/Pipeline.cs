using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Studio.DotNet.Api
{
    /// <summary>
    /// 双向管道实现
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class Pipeline<TContext>  : IPipeline<TContext>
    {
        private readonly IList<Func<Func<TContext, Task<TContext>>, Func<TContext, Task<TContext>>>> _middlewares = new List<Func<Func<TContext, Task<TContext>>, Func<TContext, Task<TContext>>>>();

        /// <summary>
        /// 创建双向管道并执行管道
        /// </summary>
        /// <param name="context">中间件间的上下文</param>
        /// <returns>管道处理以后的上下文</returns>
        public Task<TContext> ProcessAsync(TContext context)
        {
            if (!_middlewares.Any())
            {
                return Task.FromResult(context);
            }
            var firstProcessLogic = _middlewares
                .Reverse()  // 让最后一个添加的中间件调用结束中间件
                .Aggregate((Func<TContext, Task<TContext>>)Task.FromResult, (c, i) => i(c));
            return firstProcessLogic(context);   // 执行添加的第一个中间件
        }

        public IPipeline<TContext> Add(Func<Func<TContext, Task<TContext>>, Func<TContext, Task<TContext>>> middleware)
        {
            _middlewares.Add(middleware);
            return this;
        }
    }

    public interface IPipeline<TContext>
    {
		IPipeline<TContext> Add(Func<Func<TContext, Task<TContext>>, Func<TContext, Task<TContext>>> middleware);
        Task<TContext> ProcessAsync(TContext context);
    }
}

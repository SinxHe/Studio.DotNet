using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#pragma warning disable 1570

namespace Studio.DotNet.Api
{
    /// <summary>
    /// 双向管道实现
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class Pipeline<TContext>  : IPipeline<TContext>
    {
        private readonly IList<Func<Func<TContext, Task>, Func<TContext, Task>>> _middlewares = new List<Func<Func<TContext, Task>, Func<TContext, Task>>>();

		/// <summary>
		/// 创建双向管道并执行管道
		/// </summary>
		/// <param name="context">中间件间的上下文</param>
		/// <returns>管道处理以后的上下文</returns>
		/// <remarks>
		/// 返回值不要弄成 Task<TContext>, 因为 中间件的链式很少, 写上下文委托逻辑的时候还不得不写 return 语句倒是麻烦了不少
		/// </remarks>
		public Task ProcessAsync(TContext context)
		{
            var firstProcessLogic = _middlewares
                .Reverse()  // 让最后一个添加的中间件调用结束中间件
                .Aggregate(ctx => Task.CompletedTask, (c, i) => i(c));
            return firstProcessLogic(context);   // 执行添加的第一个中间件
        }

        public IPipeline<TContext> Add(Func<Func<TContext, Task>, Func<TContext, Task>> middleware)
        {
            _middlewares.Add(middleware);
            return this;
        }
    }

    public interface IPipeline<TContext>
    {
		IPipeline<TContext> Add(Func<Func<TContext, Task>, Func<TContext, Task>> middleware);
		Task ProcessAsync(TContext context);
    }
}

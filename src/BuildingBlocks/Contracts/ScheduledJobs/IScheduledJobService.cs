using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.ScheduledJobs
{
    public interface IScheduledJobService
    {
        #region Fire And Forget
        string Enqueue(Expression<Action> functionCall);
        string Enqueue<T>(Expression<Action<T>> functionCall);
        #endregion


        #region Delay 
        string Schedule(Expression<Action> functionCall, TimeSpan delay);
        string Schedule<T>(Expression<Action<T>> functionCAll, TimeSpan delay);
        string Schedule(Expression<Action> functionCall, DateTimeOffset enqueueAt);
        //string Schedule<T>(Expression<Action<T>> functionCall, DateTimeOffset enqueueAt);
        #endregion


        #region Continuos jobs
        string ContinueQueueWith(string parentJobId, Expression<Action> functionCall);
        #endregion
        bool Delete(string jobId);
        bool Requeue(string jobId);
    }
}

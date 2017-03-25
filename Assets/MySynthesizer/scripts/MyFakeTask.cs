//#define DEBUG_TASK
#define DISPOSABLE

using System;
using System.Collections.Generic;

#if !WINDOWS_UWP
namespace MySpace
{
    namespace Tasks
    {
        public class Task
#if DISPOSABLE
            : IDisposable
#endif
        {
            private class Worker
            {
                private const int maxThreads = 3;
                private const int maxJobs = 256;
                private readonly System.Threading.AutoResetEvent jobEvent;
                private readonly Task[] tasks;
                private volatile bool exit;
                private volatile int rpos;
                private volatile int wpos;
                public Worker()
                {
                    //UnityEngine.Debug.Log("worker start");
                    tasks = new Task[maxJobs];
                    rpos = 0;
                    wpos = 0;
                    exit = false;
#if DEBUG_TASK
                    jobEvent = null;
                    threadList = null;
#else
                    jobEvent = new System.Threading.AutoResetEvent(false);
                    for (int i = 0; i < maxThreads; i++)
                    {
                        System.Threading.Thread thread = new System.Threading.Thread(threadMain);
                        thread.Priority = System.Threading.ThreadPriority.Normal;
                        thread.IsBackground = true;
                        thread.Start();
                    }
#endif
                }
                public void Terminate()
                {
                    exit = true;
                }

                private void threadMain()
                {
                    //UnityEngine.Debug.Log("threadMain start");
                    while (!exit)
                    {
                        jobEvent.WaitOne(200);
                        while (rpos != wpos)
                        {
                            pumpJob();
                        }
                    }
                    //UnityEngine.Debug.Log("threadMain exit");
                }
                private void pumpJob()
                {
                    Task task = null;
                    int cpos = rpos;
                    int npos = (cpos + 1) % maxJobs;
                    lock (jobEvent)
                    {
                        int tpos = rpos;
                        if (cpos == tpos)
                        {
                            task = tasks[cpos];
                            tasks[cpos] = null;
                            rpos = npos;
                            cpos = npos;
                        }
                        else
                        {
                            cpos = tpos;
                        }
                    }
                    if ((task != null) && !exit)
                    {
                        try
                        {
                            task.state = TaskState.Running;
                            task.job.Invoke();
                            task.SetResult(TaskState.RanToCompletion);
                        }
                        catch (System.Exception e)
                        {
                            task.exception = e;
                            task.SetResult(TaskState.Faulted);
                            UnityEngine.Debug.LogError(e);
                        }
                    }
                    task = null;
                }
                public void Delegate(Task task)
                {
                    lock (tasks)
                    {
#if DEBUG_TASK
                        try
                        {
                            task.state = TaskState.Running;
                            task.job.Invoke();
                            task.state = TaskState.RanToCompletion;
                        }
                        catch (System.Exception e)
                        {
                            UnityEngine.Debug.LogError(e);
                            task.exception = e;
                            task.state = TaskState.Faulted;
                        }
#else
                        int cpos = wpos;
                        int npos = (cpos + 1) % maxJobs;
                        while (npos == rpos)
                        {
                            UnityEngine.Debug.LogWarning("Too mutch jobs. Please tune maxJobs and numThreads.");
                            if (exit)
                            {
                                return;
                            }
                            System.Threading.Thread.Sleep(1);
                        }
                        tasks[cpos] = task;
                        wpos = npos;
                        jobEvent.Set();
#endif
                    }
                }
            }

            public enum TaskState
            {
                Canceled,
                Created,
                Faulted,
                RanToCompletion,
                Running,
                WaitingForActivation,
                WaitingForChildrenToComplete,
                WaitingToRun,
            }
            private volatile TaskState state;
            private System.Exception exception;
            private Action job;
            private System.Threading.ManualResetEvent complete;

#if DISPOSABLE
            private static Object lockObject = new Object();
            private static int taskCount;
            private static Worker worker;
            private bool disposed = false;
#else
            private static Worker worker = new Worker();
#endif

            public Task(Action Job)
            {
                //UnityEngine.Debug.Log("Task()");
#if DISPOSABLE
                lock (lockObject)
                {
                    if (worker == null)
                    {
                        worker = new Worker();
                    }
                    taskCount++;
                }
#endif
                state = TaskState.Created;
                job = Job;
            }
#if DISPOSABLE
            ~Task()
            {
                //UnityEngine.Debug.Log("~Task()");
                //UnityEngine.Debug.Log(System.Reflection.MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodInfo.GetCurrentMethod().Name);
                Dispose(false);
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposed)
                {
                    try {
                        Worker elder = null;
                        lock (lockObject)
                        {
                            if (--taskCount == 0)
                            {
                                elder = worker;
                                worker = null;
                            }
                        }
                        if (elder != null)
                        {
                            elder.Terminate();
                        }
                    }
                    catch (Exception e)
                    {
                        if (disposing)
                        {
                            throw e;
                        }
                    }
                }
            }
#endif
            private void SetResult(TaskState State)
            {
                lock (job)
                {
                    state = State;
                    if (complete != null)
                    {
                        complete.Set();
                    }
                }
            }

            public TaskState Status
            {
                get
                {
                    return state;
                }
            }
            public System.Exception Exception
            {
                get
                {
                    return exception;
                }
            }
            public bool IsCompleted
            {
                get
                {
                    TaskState st = state;
                    return (st == TaskState.Canceled) || (st == TaskState.Faulted) || (st == TaskState.RanToCompletion);
                }
            }
            public static Task Run(Action action)
            {
                Task t = new Task(action);
                t.Start();
                return t;
            }
            public void Start()
            {
                if (state != TaskState.Created)
                {
                    throw new System.InvalidOperationException();
                }
                state = TaskState.WaitingForActivation;
                worker.Delegate(this);
            }
            public void Wait()
            {
                if (IsCompleted)
                {
                    return;
                }
                lock (job)
                {
                    if (IsCompleted)
                    {
                        return;
                    }
                    complete = new System.Threading.ManualResetEvent(false);
                }
                complete.WaitOne();
                complete.Close();
                complete = null;
            }
        }
    }
}
#endif

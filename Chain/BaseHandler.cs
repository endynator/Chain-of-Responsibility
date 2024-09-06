using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet.Chain
{
    [NeedInBackupValidator(-1)]
    public abstract class BaseHandler<T> : 
        IHandler<T>, IRollbackable<T, BaseHandler<T>> where T : IRollbackable<T, T>
    {
        protected NeedInBackupValidatorAttribute? ValidatorAttribute;
        protected int ProcessCounter = 0;
        protected BaseHandler<T>? Next { get; set; }
        public Action<T>? Action { set; protected get; }
        public bool HasAction() => Action is not null;

        protected virtual Exception? TryAction(T data)
        {
            try
            {
                Action!(data);
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }

        protected virtual bool IsReadyToGo()
        {
            if (Next is null) { ValidatorAttribute!.ProcessCounter = 0; return false; }
            return true;
        }
        protected virtual void Terminate<Ex>(T Data, Action<T> tryAction, Exception e) where Ex : Exception
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("\nProcessing will be stopped.\nData will be rolled back in a state before the error had occured...");
            try
            {
                tryAction(Data);
            }
            catch (Ex e1)
            {
                Console.WriteLine($"Failed to rollback: {e1.Message}");
            }
            finally
            {
                Console.WriteLine("Stopping Handler...");
                ValidatorAttribute!.ProcessCounter = 0;
            }
        }
        protected abstract Exception? TryBackup(T data);
        public abstract T Process(T Data);
        public abstract void Backup(T Data);
        public abstract BaseHandler<T>? RollBack(T Data);
    }

    [NeedInBackupValidator(-1)]
    public abstract class BaseHandler : IHandler, IRollbackable
    {
        protected NeedInBackupValidatorAttribute? ValidatorAttribute;
        protected int ProcessCounter = 0;
        protected BaseHandler? Next { get; set; }
        public Action? Action { set; protected get; }
        public bool HasAction() => Action is not null;

        protected virtual Exception? TryAction()
        {
            try
            {
                Action!();
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }

        protected virtual bool IsReadyToGo()
        {
            if (Next is null) { ValidatorAttribute!.ProcessCounter = 0; return false; }
            return true;
        }
        protected virtual void Terminate<Ex>(Action tryAction, Exception e) where Ex : Exception
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("\nProcessing will be stopped.\nData will be rolled back in a state before the error had occured...");
            try
            {
                tryAction();
            }
            catch (Ex e1)
            {
                Console.WriteLine($"Failed to rollback: {e1.Message}");
            }
            finally
            {
                Console.WriteLine("Stopping Handler...");
                ValidatorAttribute!.ProcessCounter = 0;
            }
        }
        public abstract void Process();
        public abstract void Backup();
        public abstract void RollBack();
    }
}

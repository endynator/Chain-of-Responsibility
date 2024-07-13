using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoR
{
    [NeedInBackupValidator(-1)]
    public abstract class BaseHandler<T> :
        IHandler<T>, IRollbackable<T>
        where T : IRollbackable<T>
    {
        public NeedInBackupValidatorAttribute? ValidatorAttribute { get; set; }
        public Action<T>? Action { set; protected get; }
        public IHandlerInterface? Next { get; set; }
        public bool IsGeneric { get; set; }

        public IHandlerInterface This { get; set; }

        protected internal BaseHandler()
        {
            This ??= this;
            if (ValidatorAttribute is null)
            {
                Type type = This.GetType();
                ValidatorAttribute =
                    Attribute.GetCustomAttribute(type, typeof(NeedInBackupValidatorAttribute)) as
                    NeedInBackupValidatorAttribute ?? throw new NullReferenceException("Attribute wasn't found");
            }
        }

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

        protected virtual bool IsNextNotNull()
        {
            if (Next is null) { ValidatorAttribute!.ProcessCounter = 0; return false; }
            return true;
        }

        /// <summary>
        /// Handles exceptions by attempting a specified action and providing a mechanism to process the exception.
        /// <list type="table">
        /// <item>
        /// <term><typeparamref name="Ex"/></term>
        /// <description>The type of <see cref="System.Exception"/> that is expected to be caught when invoking <paramref name="tryAction"/>.</description>
        /// </item>
        /// <item>
        /// <term>Purpose</term>
        /// <description>Called whenever we need to handle the specified exception in a specific way.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="Ex">The type of exception that is thrown to terminate the process.</typeparam>
        /// <param name="tryAction">The action to attempt when terminating the process.</param>
        /// <param name="e">The exception instance that was caught.</param>
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
        public abstract T? Process(T Data);
        public abstract void Backup(T Data);
        public abstract IRollbackable<T>? RollBack(T Data);
    }
}
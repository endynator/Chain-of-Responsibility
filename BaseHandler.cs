using System.Reflection;

namespace CoR
{
    [NeedInBackupValidator(-1)]
    public abstract class BaseHandler : IHandler, IRollbackable
    {
        public NeedInBackupValidatorAttribute? ValidatorAttribute { get; set; }
        public IHandlerInterface? Next { get; set; }
        public bool IsGeneric { get; set; }
        public Action? Action { set; protected internal get; }
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

        protected abstract Exception? TryBackup();
        public abstract void Process();
        public abstract void Backup();
        public abstract void RollBack();
    }
}
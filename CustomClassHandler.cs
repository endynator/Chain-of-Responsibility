using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using System.Xml;

namespace CoR
{
    public class CustomClassHandler<T> : BaseHandler<T>
        where T : IRollbackable<T>
    {

        public CustomClassHandler()
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
        #region Utility Methods
        protected override Exception? TryBackup(T data)
        {
            try
            {
                data.Backup(data);
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }
        #endregion

        public override T? Process(T Data)
        {
            ++ValidatorAttribute!.ProcessCounter;

            Exception? e1 = TryBackup(Data);
            if (e1 is not null)
            {
                Console.WriteLine("Failed to backup...");
                Data.RollBack(Data);
                ValidatorAttribute!.ProcessCounter = 0;
                return Data;
            }

            if (!HasAction())
            {
                if (!IsNextNotNull()) { return Data; }

                var temp = This.CallNext(Data);
            }
            Exception? e2 = TryAction(Data);
            if (e2 is not null)
            {
                Terminate<Exception>(Data, (Data) => { var rb = Data.RollBack(Data); }, e2);
                ValidatorAttribute!.ProcessCounter = 0;
                return Data;
            }
            return (T?)This.CallNext(Data);
        }

        public override void Backup(T Data)
        {
            if (!ValidatorAttribute!.IsReadyForBackup()) return;
            Data.Backup(Data);
        }

        public override CustomClassHandler<T>? RollBack(T Data)
        {
            try
            {
                Data.RollBack(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Called during execution of ");
                Console.WriteLine(e.Message);
            }
            return this;
        }
    }

}
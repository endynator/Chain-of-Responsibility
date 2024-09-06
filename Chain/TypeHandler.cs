using System.Reflection.Metadata;

namespace dotnet.Chain
{
    public class TypeHandler<T> : BaseHandler<T> where T : IRollbackable<T, T>
    {
        public TypeHandler(TypeHandler<T>? handler = null)
        {
            Next = handler;
            Type type = this.GetType();
            ValidatorAttribute =
                Attribute.GetCustomAttribute(type, typeof(NeedInBackupValidatorAttribute)) as
                NeedInBackupValidatorAttribute ?? throw new NullReferenceException("Attribute wasn't found");
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

        public virtual TypeHandler<T> SetNext(TypeHandler<T> handler, Action<T>? action = null)
        {
            if (action is not null) Action = action;
            Next = handler;
            return handler;
        }

        public override T Process(T Data) {
            ++ValidatorAttribute!.ProcessCounter;

            Exception? e1 = TryBackup(Data);
            if (e1 is not null)
            {
                Console.WriteLine("Failed to backup...");
                Data.RollBack(Data);
                ValidatorAttribute!.ProcessCounter = 0;
                return Data;
            }

            if (!HasAction()) return IsReadyToGo() ? Next!.Process(Data) : Data;

            Exception? e2 = TryAction(Data);
            if (e2 is not null)
            {
                Terminate<Exception>(Data, (Data) => { var rb = Data.RollBack(Data); }, e2);
                ValidatorAttribute!.ProcessCounter = 0;
                return Data;
            }
            return IsReadyToGo() ? Next!.Process(Data) : Data;
        }
        

        public override void Backup(T Data)
        {
            if (!ValidatorAttribute!.IsReadyForBackup()) return;
            Data.Backup(Data);
        }

        public override BaseHandler<T>? RollBack(T Data)
        {
            try
            {
                Data.RollBack(Data);
            }catch(Exception e)
            {
                Console.WriteLine($"Called during execution of ");
                Console.WriteLine(e.Message);
            }
            return this;
        }
    }

    
}
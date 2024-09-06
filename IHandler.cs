using System.Diagnostics;

namespace CoR
{
    [NeedInBackupValidator(-1)]
    public interface IHandlerInterface
    {
        public IHandlerInterface This { get; protected internal set; }
        protected internal NeedInBackupValidatorAttribute? ValidatorAttribute { get; set; }
        protected internal abstract IHandlerInterface? Next { get; set; }
        public bool IsGeneric { get; set; }
        public virtual void SetNext(params IHandlerInterface[] handler)
        {
            IHandlerInterface present = this;
            foreach (var item in handler)
            {
                present.Next = item;
                var t = item.GetType();
                if (t.IsGenericType & t == typeof(IHandler<>))
                {
                    present.Next.IsGeneric = true;
                    present = present.Next;
                    continue;
                }
                else if (item is IHandler)
                {
                    present.Next.IsGeneric = false;
                    present = present.Next;
                    continue;
                }
                throw new InvalidCastException($"Object {item} is not Handler");
            }
        }
        public virtual IHandlerInterface SetNext(IHandlerInterface handler)
        {
            Next = handler;
            var t = handler.GetType();
            if (t.IsGenericType & t == typeof(IHandler<>)) Next.IsGeneric = true;
            else if (handler is IHandler) Next.IsGeneric = false;
            throw new InvalidCastException($"Object {handler} is not Handler");
        }
        public virtual bool IsNextNotNull()
        {
            if (Next is null) { ValidatorAttribute!.ProcessCounter = 0; return false; }
            return true;
        }
        public virtual object? CallNext(object? Data)
        {
            if (IsNextNotNull()) return null;
            if (Data is null | !Next!.IsGeneric)
            {
                if (Next is IHandler n) n.Process();
                return null;
            }
            else
            {
                try
                {
                    return Utils.CallGenericMethod(Next, Next.GetType().GenericTypeArguments[0], Data!);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to call generic method");
                    Console.WriteLine(e.Message);
                    return null;
                }

            }
        }
    };

    public interface IHandler : IHandlerInterface
    {
        void Process();
    }

    public interface IHandler<T> : IHandlerInterface
    {
        T? Process(T Data);
    }
}
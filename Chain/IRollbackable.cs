using System.Reflection.Metadata;

namespace dotnet.Chain
{
    // 'R' used for Covariant returns
    public interface IRollbackable<T, R> where R: IRollbackable<T, R>
    {
        public void Backup(T Data);
        R? RollBack(T Data);
    }
    public interface IRollbackable<T> 
    {
        public void Backup(T Data);
        T? RollBack(T Data);
    }
    public interface IRollbackable
    {
        void Backup();
        void RollBack();
    }
}
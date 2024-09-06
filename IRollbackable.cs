using System.Reflection.Metadata;

namespace CoR
{
    /// <summary>
    /// <list type="table">
    /// <item>
    /// <term><typeparamref name="T"/></term>
    /// <description>Type of data used to backup</description>
    /// </item>
    /// <item>
    /// <term><typeparamref name="TRollback"/></term>
    /// <description>Used for covariant return when doing the <see cref="RollBack(T)"/></description>
    /// </item>
    /// </list>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRollback"></typeparam>
    public interface IRollbackable<T>
    {
        public void Backup(T Data);
        IRollbackable<T>? RollBack(T Data);
    }
    public interface IRollbackable
    {
        void Backup();
        void RollBack();
    }
}
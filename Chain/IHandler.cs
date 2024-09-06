using System.Diagnostics;

namespace dotnet.Chain
{
  public interface IHandler
  {
    void Process();
  }

  public interface IHandler<T>
  {
    T Process(T Data);
  }
}
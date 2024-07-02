using System.Diagnostics;
using System.Linq.Expressions;
using dotnet.Chain;

namespace dotnet
{
    file interface IOccupation;
    file class User(int age) : IRollbackable<User>
    {
        public required string Email { init; get; }
        public int Age { get; private set; } = age;
        public IOccupation? Occupation { get; set; }

        private User? backup;
        public User(User user)
            : this(user.Age)
        {
            Occupation = user.Occupation;
            backup = user.backup;
        }

        public void Backup(User Data)
        {
            backup = new(Data)
            {
                Email = Data.Email
            };
        }

        public User? RollBack(User Data) => backup;

    }
    public static class Program
    {


        public static void Main()
        {
            DebugInfo.Start();

            TypeHandler<User> handler1 = new();

            var info = DebugInfo.StopAndReturn();
            DebugInfo.Reset();
            Console.WriteLine($"Elapsed time: {info.Item1}\nMemory usage: {info.Item2}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoR
{
    static class Utils
    {
        public static T? ExecuteAndReturnNull<T>(Action<object[]> action, params object[] parameters)
        { 
            action(parameters);
            return default;
        }
        public static object? CallGenericMethod(IHandlerInterface handlerInterface, Type genericType, object parameter)
        {
            // Get the type of the handlerInterface object
            Type handlerType = handlerInterface.GetType();

            // Find the generic method using reflection
            var methodInfo = handlerType.GetMethod("Process");

            if (methodInfo != null)
            {
                // Make a generic method for the specific type
                var genericMethod = methodInfo.MakeGenericMethod(genericType);

                // Invoke the generic method
                var result = genericMethod.Invoke(handlerInterface, [parameter]);
                Console.WriteLine($"Result from generic method: {result}");
                return result;
            }
            else
            {
                Console.WriteLine("Generic method not found.");
                return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoR
{
    /// <summary>
    /// </summary>
    /// <param name="Interval">Used to define how often the backup is needed where negative value will be considered as a need in backup once</param>
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Interface |
        AttributeTargets.GenericParameter, Inherited = true)]
    public class NeedInBackupValidatorAttribute(int Interval) : Attribute
    {
        public int PostErrorRollbackCount { get; set; } = Interval;
        public int ProcessCounter { get; set; }

        public bool IsReadyForBackup(bool isDataNull = false)
        {
            if (PostErrorRollbackCount <= -1) return isDataNull;
            return PostErrorRollbackCount % ProcessCounter == 0;
        }
    }
}

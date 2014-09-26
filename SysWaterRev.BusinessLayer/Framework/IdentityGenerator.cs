using System;

namespace SysWaterRev.BusinessLayer.Framework
{
    public static class IdentityGenerator
    {
        public static Guid NewSequentialGuid()
        {
            var uid = Guid.NewGuid().ToByteArray();
            var binDate = BitConverter.GetBytes(DateTime.UtcNow.Ticks);

            var sequentialGuid = new byte[uid.Length];

            sequentialGuid[0] = uid[0];
            sequentialGuid[1] = uid[1];
            sequentialGuid[2] = uid[2];
            sequentialGuid[3] = uid[3];
            sequentialGuid[4] = uid[4];
            sequentialGuid[5] = uid[5];
            sequentialGuid[6] = uid[6];

            // Verification - Set to 1100
            sequentialGuid[7] = (byte)(0xc0 | (0xf & uid[7]));

            // Sequential part
            sequentialGuid[9] = binDate[0];
            sequentialGuid[8] = binDate[1];
            sequentialGuid[15] = binDate[2];
            sequentialGuid[14] = binDate[3];
            sequentialGuid[13] = binDate[4];
            sequentialGuid[12] = binDate[5];
            sequentialGuid[11] = binDate[6];
            sequentialGuid[10] = binDate[7];

            return new Guid(sequentialGuid);
        }

        public static string GenerateCustomerNumber()
        {
            var rand = new Random();
            var customerNumber = string.Format("CUST{0}", rand.Next(1000, 100000));
            return customerNumber;
        }

        public static string GenerateEmployeeNumber()
        {
            var rand = new Random();
            var employeeNumber = string.Format("EMP{0}", rand.Next(1000, 100000));
            return employeeNumber;
        }
    }
}
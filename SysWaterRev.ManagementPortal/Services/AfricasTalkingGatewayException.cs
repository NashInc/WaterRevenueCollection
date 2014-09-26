using System;

namespace SimpleRevCollection.Management.Services
{
    public class AfricasTalkingGatewayException : Exception
    {
        public AfricasTalkingGatewayException(string message)
            : base(message) { }
    }
}
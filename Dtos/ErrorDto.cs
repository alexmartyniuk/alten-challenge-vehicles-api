using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehiclesAPI.Dtos
{
    public class ErrorDto
    {
        public string Message { get; private set; }

        public ErrorDto(string message)
        {
            Message = message;
        }
    }
}

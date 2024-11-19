using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CreateContact.Application.DTOs
{
    public class BaseReponse
    {
        [JsonIgnore]
        public int ErrorCode { get; set; }
        [JsonIgnore]
        public string ErrorDescription { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorBoilerplate.Shared.Dto.Logistique
{
    // https://support.map.ir/developers/api/quest/2-0-0/%d9%85%d8%b3%d8%aa%d9%86%d8%af%d8%a7%d8%aa-2/
    public class LocationSearchDto
    {
        public string text { get; set; }

        [JsonProperty("$filter")]
        public string filter
        {
            get
            {
                return "province eq تهران"; // TODO Obviously we support Tehran only.
            }
        }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorBoilerplate.Shared.Dto.Logistique
{
    // https://support.map.ir/developers/api/quest/2-0-0/%d9%85%d8%b3%d8%aa%d9%86%d8%af%d8%a7%d8%aa-2/
    public class LocationSearchResponseDto
    {
        [JsonProperty("odata.count")]
        public int count;

        public List<Value> value;

        public class Value
        {
            public string Province;
            public string County;
            public string District;
            public string City;
            public string Region;
            public string Neighborhood;
            public string Title;
            public string Address;
            public string Type;
            public string Fclass;
            public GeoJson Geom;
        }
    }
}
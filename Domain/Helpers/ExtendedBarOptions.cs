using ChartJs.Blazor.BarChart;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Domain.Helpers
{
    public class ExtendedBarOptions : BarOptions
    {
        [JsonProperty("indexAxis")]
        public string IndexAxis { get; set; } = "x"; // por defecto vertical

        [JsonProperty("scales")]
        public object Scales { get; set; } = new
        {
            x = new
            {
                type = "linear",
                ticks = new { beginAtZero = true, stepSize = 1 }
            },
            y = new
            {
                type = "category"
            }
        };
    }
}

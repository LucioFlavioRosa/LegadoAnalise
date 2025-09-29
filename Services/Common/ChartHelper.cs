using System.Collections.Generic;

namespace Services.Common;

public static class ChartHelper
{
    public static object BuildGaugeChartOptions(double valor, string title = "", double max = 100)
    {
        return new
        {
            title = new { text = title, left = "center" },
            series = new[]
            {
                new {
                    type = "gauge",
                    startAngle = 180,
                    endAngle = 0,
                    progress = new { show = false, width = 10 },
                    axisLine = new {
                        roundCap = true,
                        lineStyle = new {
                            width = 20,
                            color = new List<object> {
                                new object[] { 0.5, "#ca583f" },
                                new object[] { 0.75, "#FDDD60" },
                                new object[] { 1, "#1eaa59" }
                            }
                        }
                    },
                    axisTick = new { show = false },
                    splitLine = new {
                        length = 15,
                        lineStyle = new { width = 0, color = "#999" }
                    },
                    axisLabel = new { show = false, distance = 5, color = "#999", fontSize = 14 },
                    pointer = new {
                        icon = "path://M2090.36389,615.30999 L2090.36389,615.30999 C2091.48372,615.30999 2092.40383,616.194028 2092.44859,617.312956 L2096.90698,728.755929 C2097.05155,732.369577 2094.2393,735.416212 2090.62566,735.56078 C2090.53845,735.564269 2090.45117,735.566014 2090.36389,735.566014 L2090.36389,735.566014 C2086.74736,735.566014 2083.81557,732.63423 2083.81557,729.017692 C2083.81557,728.930412 2083.81732,728.84314 2083.82081,728.755929 L2088.2792,617.312956 C2088.32396,616.194028 2089.24407,615.30999 2090.36389,615.30999 Z",
                        length = "85%",
                        width = 6,
                        offsetCenter = new object[] { 0, "0" }
                    },
                    anchor = new {
                        show = true,
                        showAbove = true,
                        size = 10,
                        itemStyle = new { borderWidth = 3 }
                    },
                    detail = new {
                        show = false,
                        valueAnimation = true,
                        fontSize = 30,
                        offsetCenter = new object[] { 0, "20%" }
                    },
                    data = new[] { new { value = valor > max ? max : valor } }
                }
            }
        };
    }
}

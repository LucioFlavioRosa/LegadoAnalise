using System;

namespace Services.Common;

public static class AccordionHelper
{
    public static string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        if (text.Length > maxLength)
            return text.Substring(0, maxLength) + "...";
        return text;
    }

    public static string GetAccordionButtonText(bool isExpanded, string expandText = "Ver+")
    {
        return isExpanded ? "Ocultar" : expandText;
    }

    public static string GetAccordionCssClass(bool isExpanded)
    {
        return isExpanded ? "accordion-toggle expanded" : "accordion-toggle";
    }

    public static string GetAccordionBodyClass(bool isExpanded)
    {
        return isExpanded ? "accordian-body collapse show" : "accordian-body collapse";
    }

    public static string GetHiddenRowClass()
    {
        return "hiddenRow";
    }

    public static string GetHiddenRowStyle()
    {
        return "padding: 0 !important;";
    }
}

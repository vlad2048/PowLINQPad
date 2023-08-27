using LINQPad.Controls;

namespace PowLINQPad.Controls_.Utils;

static class CtrlUIExt
{
    public static RangeControl SetStep(this RangeControl ctrl, int step)
    {
        ctrl.HtmlElement.InvokeScript(false, "eval", $"targetElement.step = {step}");
        return ctrl;
    }
}